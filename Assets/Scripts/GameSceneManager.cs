using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Photon.Realtime;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	List<string> messageList = new List<string>();

	[SerializeField]
	TextMeshProUGUI messageText;

	private PhotonView _pv;

	public Dictionary<Player, int> playerScores = new Dictionary<Player, int>();

	//seat spawning
	public Transform seat1;
	public Transform seat2;
	public Transform seat3;

	//lane boundaries 
	public float lane1Left = -8f;
	public float lane1Right = -4f;

	public float lane2Left = -2f;
	public float lane2Right = 2f;

	public float lane3Left = 4f;
	public float lane3Right = 8f;

	//lane names
	public TextMeshProUGUI lane1Name;
	public TextMeshProUGUI lane2Name;
	public TextMeshProUGUI lane3Name;

	//lane spawn areas
	public BoxCollider2D lane1SpawnArea;
	public BoxCollider2D lane2SpawnArea;
	public BoxCollider2D lane3SpawnArea;


	//lane scores
	public TextMeshProUGUI lane1Score;
	public TextMeshProUGUI lane2Score;
	public TextMeshProUGUI lane3Score;

	//track balloons
	public int lane1BalloonCount = 0;
	public int lane2BalloonCount = 0;
	public int lane3BalloonCount = 0;

	//for winner
	public GameObject winPanel;
	public TextMeshProUGUI winText;
	public Button returnButton;

	[SerializeField] private AudioSource gameOverSound;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		_pv = this.gameObject.GetComponent<PhotonView>();

		if (PhotonNetwork.CurrentRoom == null)
		{
			SceneManager.LoadScene("LobbyScene");
			return;
		}
		else
		{
			InitGame();

			if (PhotonNetwork.IsMasterClient)

			{
				CallRpcSendMessageToAll("Game Started!");
			}

			if (PhotonNetwork.IsMasterClient)
			{
				for (int i = 0; i < 3; i++)
				{
					SpawnBalloonInLane(1);
					SpawnBalloonInLane(2);
					SpawnBalloonInLane(3);
				}
			}


		}

		returnButton.onClick.AddListener(ReturnToLobby);
	}

	public void InitGame()
	{

		foreach (var player in PhotonNetwork.CurrentRoom.Players.Values) 
		{
			playerScores[player] = 0;
		}
		

		SpawnPlayerBySeat();
	}


	//seat spawning
	private void SpawnPlayerBySeat()
	{
		int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;

		Transform spawnPoint = null;

		if (actorNum == 1)
			spawnPoint = seat1;
		else if (actorNum == 2)
			spawnPoint = seat2;
		else if (actorNum == 3)
			spawnPoint = seat3;
		else
		{
			Debug.Log("Too many players! Only 3 allowed.");
			return;
		}

		
		GameObject playerObj = PhotonNetwork.Instantiate("PhotonPlayer", spawnPoint.position, Quaternion.identity);


		// Assign lane boundaries
		PlayerController pc = playerObj.GetComponent<PlayerController>();

		if (actorNum == 1)
		{
			pc.leftLimit = lane1Left;
			pc.rightLimit = lane1Right;
		}
		else if (actorNum == 2)
		{
			pc.leftLimit = lane2Left;
			pc.rightLimit = lane2Right;
		}
		else if (actorNum == 3)
		{
			pc.leftLimit = lane3Left;
			pc.rightLimit = lane3Right;
		}


		//put names on screen 
		string playerName = PhotonNetwork.LocalPlayer.NickName;
		_pv.RPC("RPC_SetLaneName", RpcTarget.AllBuffered, actorNum, playerName);

	}

	//setting names
	[PunRPC]
	void RPC_SetLaneName(int actorNumber, string playerName)
	{
		if (actorNumber == 1)
			lane1Name.text = playerName;
		else if (actorNumber == 2)
			lane2Name.text = playerName;
		else if (actorNumber == 3)
			lane3Name.text = playerName;
	}

	//spawn balloons
	Vector3 GetRandomPointInArea(BoxCollider2D area)
	{
		Bounds b = area.bounds;

		float x = Random.Range(b.min.x, b.max.x);
		float y = Random.Range(b.min.y, b.max.y);

		return new Vector3(x, y, 0);
	}

	// Spawn balloon in a specific lane and track
	void SpawnBalloonInLane(int lane)
	{
		Vector3 pos = Vector3.zero;

		if (lane == 1)
		{
			pos = GetRandomPointInArea(lane1SpawnArea);
			lane1BalloonCount++;
		}
		else if (lane == 2)
		{
			pos = GetRandomPointInArea(lane2SpawnArea);
			lane2BalloonCount++;
		}
		else if (lane == 3)
		{
			pos = GetRandomPointInArea(lane3SpawnArea);
			lane3BalloonCount++;
		}

		GameObject balloon = PhotonNetwork.Instantiate("PhotonBalloon", pos, Quaternion.identity);

		// Tell balloon which lane it belongs to
		balloon.GetComponent<Balloon>().lane = lane;

	}

	//balloon popped counting tracker
	public void BalloonPopped(int lane)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		if (lane == 1)
		{
			lane1BalloonCount--;
			if (lane1BalloonCount < 3)
				SpawnBalloonInLane(1);
		}
		else if (lane == 2)
		{
			lane2BalloonCount--;
			if (lane2BalloonCount < 3)
				SpawnBalloonInLane(2);
		}
		else if (lane == 3)
		{
			lane3BalloonCount--;
			if (lane3BalloonCount < 3)
				SpawnBalloonInLane(3);
		}
	}


	//scoring
	public void AddScore(Player player, int amount)
	{
		if (!playerScores.ContainsKey(player))
			playerScores[player] = 0;

		playerScores[player] += amount;

		_pv.RPC("RPC_UpdateScoreUI", RpcTarget.All, player.ActorNumber, playerScores[player]);


		//check win
		if (PhotonNetwork.IsMasterClient && playerScores[player] >= 100)
		{
			_pv.RPC("RpcDeclareWinner", RpcTarget.All, player.NickName);
		}

	}

	void UpdateScoreUI(Player player)
	{
		int actorNum = player.ActorNumber;

		if (actorNum == 1)
			lane1Score.text = playerScores[player].ToString();
		else if (actorNum == 2)
			lane2Score.text = playerScores[player].ToString();
		else if (actorNum == 3)
			lane3Score.text = playerScores[player].ToString();
	}

	[PunRPC]
	void RPC_UpdateScoreUI(int actorNumber, int newScore)
	{
		foreach (var kvp in playerScores)
		{
			Player player = kvp.Key;
			if (player.ActorNumber == actorNumber)
			{
				playerScores[player] = newScore;
				UpdateScoreUI(player);
				return;
			}
		}
	}



	[PunRPC]
	void RpcDeclareWinner(string winnerName)
	{
		winText.text = $"100 points!\nWinner: {winnerName}";
		winPanel.SetActive(true);

		// Freeze game
		Time.timeScale = 0f;
	}

	public void ReturnToLobby()
	{
		Time.timeScale = 1f;
		PhotonNetwork.LeaveRoom();
	}



	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		// Only react if the MASTER CLIENT left
		if (otherPlayer.IsMasterClient)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				StartCoroutine(DelayedLeave());
			}
		}
	}
	private IEnumerator DelayedLeave()
	{
		yield return new WaitForSeconds(0.15f);
		PhotonNetwork.LeaveRoom();
	}

	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("LobbyScene");
	}


	// --- Chat Message Functions ---
	public void CallRpcSendMessageToAll(string message)
	{
		_pv.RPC("RpcSendMessage", RpcTarget.All, message);
	}

	[PunRPC]
	void RpcSendMessage(string message, PhotonMessageInfo info)
	{
		if (messageList.Count >= 10)
		{
			messageList.RemoveAt(0);
		}

		// Add the sender's name to the message for clarity.
		messageList.Add($"{message}"); //($"[{info.Sender.Nickname}]: (message)};
		UpdateMessage();
	}

	void UpdateMessage()
	{
		if (messageText != null)
		{
			messageText.text = string.Join("\n", messageList);
		}
	}


	void CallRpcReloadGame()
	{
		_pv.RPC("ReloadGame", RpcTarget.All);

	}

	[PunRPC]
	void ReloadGame(PhotonMessageInfo info)
	{
		//SceneManager.LoadScene("GameScene");
		PhotonNetwork.LoadLevel("GameScene");
	}
}

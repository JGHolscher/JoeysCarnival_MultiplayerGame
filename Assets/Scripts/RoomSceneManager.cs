using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Text;


public class RoomScene : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI textRoomName;

	[SerializeField]
	private TextMeshProUGUI textPlayerList;

	[SerializeField]
	private Button buttonStartGame;

	[SerializeField]
	private Image panelToChangeColor;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		if (PhotonNetwork.CurrentRoom == null)
		{
			SceneManager.LoadScene("LobbyScene");
			return;
		}

		textRoomName.text = PhotonNetwork.CurrentRoom.Name;
		UpdatePlayerList();
	}


	private void UpdatePlayerList()
	{

		StringBuilder sb = new StringBuilder();
		//sb.AppendLine("Players: ");


		foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
		{
			sb.AppendLine("*" + player.NickName + "*");
		}

		textPlayerList.text = sb.ToString();


		if (buttonStartGame != null) 
		{
			buttonStartGame.interactable = PhotonNetwork.IsMasterClient;
		}

	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		UpdatePlayerList();
	}

	public override void OnPlayerLeftRoom(Player newPlayer)
	{
		UpdatePlayerList();
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		UpdatePlayerList();
	}

	public void OnClickStartGame()
	{
		if (!PhotonNetwork.IsMasterClient)
			return;
		SceneManager.LoadScene("GameScene");
	}

	public void OnClickLeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("LobbyScene");
	}

		// Update is called once per frame
		void Update()
    {
        
    }
}

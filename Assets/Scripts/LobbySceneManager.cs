using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Realtime;
using System.Text;

public class LobbySceneManager : MonoBehaviourPunCallbacks
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField]
	public TMP_InputField inputRoomName;

	[SerializeField]
	public TMP_InputField inputPlayerName;

	[SerializeField]
	public TextMeshProUGUI textRoomList;

    void Start()
    {
        if(PhotonNetwork.IsConnected == false)
		{
			SceneManager.LoadScene("StartScene");
		}
		else
		{
			if (PhotonNetwork.CurrentLobby == null)
			{
				PhotonNetwork.JoinLobby();
			}
		}
		
    }

	public override void OnConnectedToMaster()
	{
		print("Connected to Master!");
		PhotonNetwork.JoinLobby();
	}


	// Update is called once per frame
	public override void OnJoinedLobby()
	{
		print("Lobby Joined Successfully");
	}


	public string GetRoomName()
	{
		string roomName = inputRoomName.text;
		return roomName.Trim();
	}

	public string GetPlayerName()
	{
		string playerName = inputPlayerName.text;
		return playerName.Trim();
	}

	public void OnClickCreateRoom()
	{
		//get player name
		string playerName = GetPlayerName();
		if (string.IsNullOrEmpty(playerName))
		{
			Debug.LogError("Player Name is invalid.");
			return;
		}

		PhotonNetwork.LocalPlayer.NickName = playerName;

		//get room name
		string roomName = GetRoomName();
		if (!string.IsNullOrEmpty(roomName)) 
		{ 
			PhotonNetwork.CreateRoom(roomName);
		}

	}

	public void OnClickJoinRoom()
	{
		string playerName = GetPlayerName();
		if (string.IsNullOrEmpty(playerName)) 
		{
			Debug.LogError("Player Name is invalid");
			return;
		}

		PhotonNetwork.LocalPlayer.NickName = playerName;

		//room
		string roomName = GetRoomName();
		if (!string.IsNullOrEmpty(roomName))
		{
			PhotonNetwork.JoinRoom(roomName);
		}
	}

	public override void OnJoinedRoom()
	{
		print("Room Joined!");
		SceneManager.LoadScene("RoomScene");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		StringBuilder sb = new StringBuilder();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.PlayerCount > 0) //if room have people
			{
				sb.AppendLine($"RoomName: {roomInfo.Name} Player Count: {roomInfo.PlayerCount}");
			}
			textRoomList.text = sb.ToString();
		}
	}

	public void BackToStart()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
	}
}

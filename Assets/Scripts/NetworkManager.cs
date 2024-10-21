using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager networkManager;

    public Text status_Text;
    public InputField name_Input;
    public InputField room_Input;

    void Awake() => networkManager = this;
    
    void Update() => status_Text.text = PhotonNetwork.NetworkClientState.ToString();

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        print("Connect");
        PhotonNetwork.LocalPlayer.NickName = name_Input.text;
        UIManager.uiManager.isConnect = true;
        UIManager.uiManager.CloseStatus();
        UIManager.uiManager.ActiveButtons(true);
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnect");
        UIManager.uiManager.CloseStatus();
        UIManager.uiManager.isConnect = false;
    }

    public void JoinServer() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        print("Join server");
        UIManager.uiManager.CloseStatus();
    }

    public void CreateRoom() => PhotonNetwork.CreateRoom(room_Input.text, new RoomOptions { MaxPlayers = 2 });

    public void JoinRoom() => PhotonNetwork.JoinRoom(room_Input.text);

    public void RandomMatch() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnCreatedRoom()
    {
        print("Create Room");
        UIManager.uiManager.CloseStatus();
        UIManager.uiManager.CloseRoom(false);
    }

    public override void OnJoinedRoom()
    {
        print("Join Room");
        UIManager.uiManager.CloseStatus();
        UIManager.uiManager.CloseRoom(false);
        UIManager.uiManager.ShowRoom(PhotonNetwork.PlayerList[0].NickName, PhotonNetwork.PlayerList.Length > 1 ? PhotonNetwork.PlayerList[1].NickName : "");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Failed Create Room");
        UIManager.uiManager.CloseStatus();
        UIManager.uiManager.CreateRoomFailed();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Failed Join Room");
        UIManager.uiManager.CloseStatus();
        UIManager.uiManager.JoinRoomFailed();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Failed Random Match");
        UIManager.uiManager.CloseStatus();
    }
}

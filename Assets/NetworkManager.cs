using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField roomCode; 
    [SerializeField] private TMP_InputField username;

    void Connect(){
        PhotonNetwork.ConnectUsingSettings();   
    }

    public void CreateRoom(){
        if(roomCode.text != null && username.text != null){
            PhotonNetwork.LocalPlayer.NickName = username.text;
            Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(roomCode.text, roomOptions);
        }
        else{
            Debug.LogError("Username and/or room code field is empty");
        }
    }

    public void JoinRoom(){
        if(roomCode != null && username.text != null){
            PhotonNetwork.LocalPlayer.NickName = username.text;
            PhotonNetwork.JoinRoom(roomCode.text);
        }
        else{
            Debug.LogError("Username and/or room code field is empty");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Join failed");
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
        base.OnJoinedRoom();
    }
}

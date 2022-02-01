using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

public class ServerSetup : MonoBehaviourPunCallbacks {

    [SerializeField] private Text logText, usernameText;

    [SerializeField] private InputField usernameInput;
    
    [SerializeField] private LobbyManager lobbyManager;
    

    public void OnClickConnectToMainServer() {
    
        if(usernameInput.text.Length.Equals(0)) {
            
            logText.text = "Input Username";
            return;
        }
        PhotonNetwork.NickName = usernameInput.text.ToUpper();
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        
        logText.text = "Connecting...";
    }
    
    public override void OnConnectedToMaster() {

        usernameText.text = PhotonNetwork.NickName;
        base.OnConnected();
        PhotonNetwork.JoinLobby();
        lobbyManager.lobbyPanel.SetActive(true);
        logText.text = "Connected";
    }

    public override void OnDisconnected(DisconnectCause cause) {

        logText.text = "Disconnected From Server For " + cause.ToString();
    }

    public override void OnJoinedLobby() {
        
        base.OnJoinedLobby();
        logText.text = "Joined Lobby";
    }

    public override void OnLeftRoom() {

        PhotonNetwork.JoinLobby();
    }
    
}

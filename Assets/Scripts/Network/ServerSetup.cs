using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ServerSetup : MonoBehaviourPunCallbacks {

    [SerializeField] private Text _logText, _usernameText;

    [SerializeField] private InputField _usernameInput;
    
    [SerializeField] private LobbyManager _lobbyManager;

    private void Awake() {
        
    }

    public void OnClickConnectToMainServer() {
    
        if(_usernameInput.text.Length.Equals(0)) {
            
            _logText.text = "Input Username";
            return;
        }
        PhotonNetwork.NickName = _usernameInput.text.ToUpper();
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        
        _logText.text = "Connecting...";
    }
    
    public override void OnConnectedToMaster() {

        _usernameText.text = PhotonNetwork.NickName;
        base.OnConnected();
        PhotonNetwork.JoinLobby();
        _lobbyManager._lobbyPanel.SetActive(true);
        _logText.text = "Connected";
    }

    public override void OnDisconnected(DisconnectCause cause) {

        _logText.text = "Disconnected From Server For " + cause.ToString();
    }

    public override void OnJoinedLobby() {
        
        base.OnJoinedLobby();
        _logText.text = "Joined Lobby";
    }

    public override void OnLeftRoom() {

        PhotonNetwork.JoinLobby();
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class LobbyManager : MonoBehaviourPunCallbacks {


    private ExitGames.Client.Photon.Hashtable _properties;
    [SerializeField] private Text logText;
    [SerializeField] private InputField roomNameInputField;
    
    [Header("Lobby Panel")]
    public GameObject lobbyPanel;
    [SerializeField] private Transform roomListContentT;
    [SerializeField] private List<RoomListInfo> roomItemList = new List<RoomListInfo>();
    [SerializeField] private RoomListInfo roomItemPrefab; 
    
    [Header("Room Panel")]
    public GameObject roomPanel;
    [SerializeField] private Transform playersListContentT;
    [SerializeField] private List<PlayerListInfo> playerItemList = new List<PlayerListInfo>();
    [SerializeField] private PlayerListInfo playerItemPrefab;
    [SerializeField] private Text roomName;
    [SerializeField] private Button startRoomButton;
    [SerializeField] private Text readyStateText;
    
    
    
    private void Start() {

        _properties = new ExitGames.Client.Photon.Hashtable();
    }


    public void OnClickCreateRoom() {

        if(roomNameInputField.text.Length.Equals(0)) {

            logText.text = "Enter Room Name";
            return;
        }
        var options = new RoomOptions {
            
            MaxPlayers = 5,
            PlayerTtl = 1
        };
        PhotonNetwork.CreateRoom(roomNameInputField.text, options);
        startRoomButton.interactable = true;
        logText.text = "Creating Room....";
    }


    public override void OnCreateRoomFailed(short returnCode, string message) {

        logText.text = "Creating Room Failed As " + message;
        startRoomButton.interactable = false;
    }

    public override void OnJoinedRoom() {

        logText.text = "Room Joined: " + PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);

        roomName.text = PhotonNetwork.CurrentRoom.Name;

        _properties["s"] = true;
        PhotonNetwork.SetPlayerCustomProperties(_properties);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {

        logText.text = "Room Join Failed as " + message;
        startRoomButton.interactable = false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {

        Debug.Log("Players Joined " + newPlayer.NickName);
    }

   
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        
        var i = playerItemList.FindIndex(x => x.nameText.text.Equals(targetPlayer.NickName));
        if(!i.Equals(-1))
            playerItemList[i].ChangePlayerState(targetPlayer.CustomProperties["s"].Equals(true)? 0: 1);

        foreach (var pair in PhotonNetwork.CurrentRoom.Players) {
            
            var index = playerItemList.FindIndex(x => x.nameText.text.Equals(pair.Value.NickName));
            if(index.Equals(-1)) {

                var playerListInfo = Instantiate(playerItemPrefab, playersListContentT);
                playerListInfo.SetPlayerName(pair.Value);
                playerItemList.Add(playerListInfo);
                Debug.Log("OnPlayerPropertiesUpdate : " + pair.Value.NickName + "::" + pair.Value.CustomProperties["s"].ToString());
                playerListInfo.ChangePlayerState(pair.Value.CustomProperties["s"].Equals(true)? 0: 1);
            }
        }    
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {

        Debug.Log("player left " + otherPlayer.NickName);
        var index = playerItemList.FindIndex(x => x.nameText.text.Equals(otherPlayer.NickName));
        if(!index.Equals(-1)) {

            Destroy(playerItemList[index].gameObject); 
            playerItemList.RemoveAt(index);
        } 
    }

    public void OnClickLeaveRoom() {

        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {

        foreach (var t in roomItemList)
            Destroy(t.gameObject);

        foreach (var t in playerItemList)
            Destroy(t.gameObject);

        roomItemList.Clear();
        playerItemList.Clear();
        roomNameInputField.text = null;

        startRoomButton.interactable = false;

        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    

    public void OnClickChangeState() {
        
        /*foreach (var player in PhotonNetwork.CurrentRoom.Players.Where(player => player.Value.NickName.Equals(PhotonNetwork.NickName))) {
        
            _properties["s"] = !_properties["s"].Equals(true);
            readyStateText.text = _properties["s"].Equals(true)? "Ready": "Idle";
            Debug.Log(_properties["s"] + "::hh");
            PhotonNetwork.SetPlayerCustomProperties(_properties);
        }*/
        foreach (var player in PhotonNetwork.CurrentRoom.Players) {
            
            if(player.Value.NickName.Equals(PhotonNetwork.NickName)) {

                _properties["s"] = !_properties["s"].Equals(true);
                readyStateText.text = _properties["s"].Equals(true)? "Ready": "Idle";
                Debug.Log(_properties["s"] + "::hh");
                PhotonNetwork.SetPlayerCustomProperties(_properties);
            }
        }
    }


    public void OnClickStartRoom() {

        SceneManager.LoadScene(1);
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        
        base.OnRoomListUpdate(roomList);
        foreach (var info in roomList) {
            
            var index = roomItemList.FindIndex(x => x.roomName.text.Equals(info.Name));
            if(info.RemovedFromList || !info.IsVisible) { 

                if(!index.Equals(-1)) {

                    Destroy(roomItemList[index].gameObject);
                    roomItemList.RemoveAt(index);
                }
                return;
            } 
            else if(index.Equals(-1)) {
                
                var item = Instantiate(roomItemPrefab, roomListContentT);
                item.SetRoomName(info);
                roomItemList.Add(item);
                Debug.Log("room added :" + roomItemList.Count);
            }
        }
    }


    public void CountRoom() {

        Debug.Log(PhotonNetwork.CountOfRooms + "Rome list");
    }
    
}

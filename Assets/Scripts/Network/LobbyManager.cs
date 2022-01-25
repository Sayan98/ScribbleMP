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
    [FormerlySerializedAs("_roomNameInputField")] [SerializeField] private InputField roomNameInputField;
    
    [Header("Lobby Panel")]
    public GameObject _lobbyPanel;
    [FormerlySerializedAs("_roomListContentT")] [SerializeField] private Transform roomListContentT;
    [FormerlySerializedAs("_roomItemList")] [SerializeField] private List<RoomListInfo> roomItemList = new List<RoomListInfo>();
    [FormerlySerializedAs("_roomItemPrefab")] [SerializeField] private RoomListInfo roomItemPrefab; 
    
    [Header("Room Panel")]
    public GameObject _roomPanel;
    [SerializeField] private Transform _playersListContentT;
    [SerializeField] private List<PlayerListInfo> _playerItemList = new List<PlayerListInfo>();
    [SerializeField] private PlayerListInfo _playerItemPrefab;
    [SerializeField] private Text _RoomName;
    [SerializeField] private Button _startRoomButton;
    [SerializeField] private Text _readyStateText;
    
    
    
    private void Start() {

        _properties = new ExitGames.Client.Photon.Hashtable();
    }


    public void OnClickCreateRoom() {

        if(roomNameInputField.text.Length.Equals(0)) {

            logText.text = "Enter Room Name";
            return;
        }
        var options = new RoomOptions
        {
            MaxPlayers = 5,
            PlayerTtl = 1
        };
        PhotonNetwork.CreateRoom(roomNameInputField.text, options);
        _startRoomButton.interactable = true;
        logText.text = "Creating Room....";
    }


    public override void OnCreateRoomFailed(short returnCode, string message) {

        logText.text = "Creating Room Failed As " + message.ToString();
        _startRoomButton.interactable = false;
    }

    public override void OnJoinedRoom() {

        logText.text = "Room Joined: " + PhotonNetwork.CurrentRoom.Name;
        _roomPanel.SetActive(true);
        _lobbyPanel.SetActive(false);

        _RoomName.text = PhotonNetwork.CurrentRoom.Name;

        _properties["s"] = true;
        PhotonNetwork.SetPlayerCustomProperties(_properties);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {

        logText.text = "Room Join Failed as " + message;
        _startRoomButton.interactable = false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {

        Debug.Log("Players Joined " + newPlayer.NickName);
    }

   
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        
        var i = _playerItemList.FindIndex(x => x._nameText.text.Equals(targetPlayer.NickName));
        if(!i.Equals(-1))
            _playerItemList[i].ChangePlayerState(targetPlayer.CustomProperties["s"].Equals(true)? 0: 1);

        foreach (KeyValuePair<int, Player>pair in PhotonNetwork.CurrentRoom.Players) {
            
            var index = _playerItemList.FindIndex(x => x._nameText.text.Equals(pair.Value.NickName));
            if(index.Equals(-1)) {

                var playerListInfo = Instantiate(_playerItemPrefab, _playersListContentT);
                playerListInfo.SetPlayerName(pair.Value);
                _playerItemList.Add(playerListInfo);
                Debug.Log("OnPlayerPropertiesUpdate : " + pair.Value.NickName + "::" + pair.Value.CustomProperties["s"].ToString());
                playerListInfo.ChangePlayerState(pair.Value.CustomProperties["s"].Equals(true)? 0: 1);
            }
        }    
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {

        Debug.Log("player left " + otherPlayer.NickName);
        var index = _playerItemList.FindIndex(x => x._nameText.text.Equals(otherPlayer.NickName));
        if(!index.Equals(-1)) {

            Destroy(_playerItemList[index].gameObject);
           _playerItemList.RemoveAt(index);
        } 
    }

    public void OnClickLeaveRoom() {

        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {

        for (var i = 0; i < roomItemList.Count; i++)
            Destroy(roomItemList[i].gameObject);  
        
        for (var i = 0; i < _playerItemList.Count; i++)
            Destroy(_playerItemList[i].gameObject);  

        roomItemList.Clear();
        _playerItemList.Clear();
        roomNameInputField.text = null;

        _startRoomButton.interactable = false;

        _roomPanel.SetActive(false);
        _lobbyPanel.SetActive(true);
    }
    

    public void OnClickChangeState() {

        foreach (KeyValuePair<int, Player>player in PhotonNetwork.CurrentRoom.Players) {
            
            if(player.Value.NickName.Equals(PhotonNetwork.NickName)) {

                _properties["s"] = !_properties["s"].Equals(true);
                _readyStateText.text = _properties["s"].Equals(true)? "Ready": "Idle";
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
            
            var index = roomItemList.FindIndex(x => x._roomName.text.Equals(info.Name));
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


    public void countroom() {

        Debug.Log(PhotonNetwork.CountOfRooms + "rrom list");
    }
    
}

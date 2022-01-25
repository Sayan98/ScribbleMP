using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListInfo : MonoBehaviourPun {

    public Text _roomName;

    public void SetRoomName(RoomInfo info) {

        _roomName.text = info.Name;
        Debug.Log("Room Name::" + _roomName.name);
    }

    public void OnClickJoinRoom() {

        PhotonNetwork.JoinRoom(_roomName.text.ToString());
    }
}
//room multiple spawn after leaving closing room
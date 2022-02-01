using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

public class RoomListInfo : MonoBehaviourPun {

    public Text roomName;

    public void SetRoomName(RoomInfo info) {

        roomName.text = info.Name;
        Debug.Log("Room Name::" + roomName.name);
    }

    public void OnClickJoinRoom() {

        PhotonNetwork.JoinRoom(roomName.text);
    }
}
//room multiple spawn after leaving closing room
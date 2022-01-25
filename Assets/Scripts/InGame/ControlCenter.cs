using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ControlCenter : MonoBehaviourPunCallbacks {
    
    public int _ColorTakenCount;
    public ColorPallete _colorPallete;

    public InGameNetworkManager _InGameNetworkManager;


    private void Awake() {

        _ColorTakenCount = -1;
    }


    public Color GetTokenColor() {

        _ColorTakenCount = _ColorTakenCount.Equals(9)? 0 : ++_ColorTakenCount;
        return _colorPallete._ColorPallete[_ColorTakenCount];
    }


    int p = 0;
    //TEST
    public void GiveScore() {   

        p = PhotonNetwork.IsMasterClient? 0: 1;
        _InGameNetworkManager._playerStateList[p]._LocalPoints += 50;
        updateList();
//        Debug.Log(_InGameNetworkManager._playerStateList[0]._localName);
        updateOverNetwork(_InGameNetworkManager._playerStateList[p]._localName, 50);

    }

    private void updateList() {

        
        for (var i = 0; i < _InGameNetworkManager._playerStateList.Count - 1; i++)
        for (var j = i + 1; j < _InGameNetworkManager._playerStateList.Count; j++) {

            if(_InGameNetworkManager._playerStateList[i]._LocalPoints < _InGameNetworkManager._playerStateList[j]._LocalPoints) {
                _InGameNetworkManager._playerStateList[j].transform.SetSiblingIndex(i);
                Debug.Log("changing pos");
            }   
        }
        
        foreach (var item in _InGameNetworkManager._playerStateList)
            item.SetRankAndPoints();

    }

    private void updateOverNetwork(string info, int score) {
    //EVENT CALL

        var obj = new object[2];
        obj[0] = info;
        obj[1] = score;

        byte eventCode = 0;
        PhotonNetwork.RaiseEvent(eventCode, obj, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    private void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += sendscore; }
    private void OnDisable(){ PhotonNetwork.NetworkingClient.EventReceived -= sendscore; }

    private void sendscore(EventData data) {

        if(!data.Code.Equals(0)) return;

        object[] obj = new object[2];
        obj = (object[])data.CustomData;
        
        for (int i = 0; i < _InGameNetworkManager._playerStateList.Count; i++) {
            
            //Debug.Log(_InGameNetworkManager._playerStateList[i]._localName + ":aaa:" + (string)obj[0]);
            if(_InGameNetworkManager._playerStateList[i]._localName.Equals((string)obj[0])) {

                _InGameNetworkManager._playerStateList[i]._LocalPoints += (int)obj[1];
                break;
            }
        }
        
                updateList();

    }
}

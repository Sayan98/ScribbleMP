using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Serialization;

public class ControlCenter : MonoBehaviourPunCallbacks {
    
    public int colorTakenCount;
    public ColorPalette colorPalette;

    public InGameNetworkManager inGameNetworkManager;


    private void Awake() {

        colorTakenCount = -1;
    }


    public Color GetTokenColor() {

        colorTakenCount = colorTakenCount.Equals(9)? 0 : ++colorTakenCount;
        return colorPalette.colorPalette[colorTakenCount];
    }

    
    //TEST
    public void GiveScore0() {   

        inGameNetworkManager.playerStateList[0].localPoints += 50;
        UpdateOverNetwork(inGameNetworkManager.playerStateList[0].localName, 50);
        UpdateList();
    }

    public void GiveScore1() {   

        inGameNetworkManager.playerStateList[1].localPoints += 50;
        UpdateOverNetwork(inGameNetworkManager.playerStateList[1].localName, 50);
        UpdateList();
    }


    private void UpdateList() {

        Debug.Log("UPDATING");
        
        for (var i = 0; i < inGameNetworkManager.playerStateList.Count - 1; i++)
        for (var j = i + 1; j < inGameNetworkManager.playerStateList.Count; j++) {
            
            if (inGameNetworkManager.playerStateList[i].localPoints >= inGameNetworkManager.playerStateList[j].localPoints) continue;
            
            inGameNetworkManager.playerStateList[j].transform.SetSiblingIndex(i);
            
            var item = inGameNetworkManager.playerStateList[j];
            inGameNetworkManager.playerStateList.RemoveAt(j);
            inGameNetworkManager.playerStateList.Insert(i, item);
            
            Debug.Log("Changing Pos");
        }

        foreach (var item in inGameNetworkManager.playerStateList)
            item.SetRankAndPoints();

    }

    private static void UpdateOverNetwork(string info, int score) {
    //EVENT CALL

        var obj = new object[2];
        obj[0] = info;
        obj[1] = score;

        const byte eventCode = 2;
        PhotonNetwork.RaiseEvent(eventCode, obj, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    public override void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += SendScore; }
    public override void OnDisable(){ PhotonNetwork.NetworkingClient.EventReceived -= SendScore; }

    private void SendScore(EventData data) {

        if (!data.Code.Equals(2)) return;

        Debug.Log("SCORING");
        var obj = (object[]) data.CustomData;

        foreach (var t in inGameNetworkManager.playerStateList.Where(t => t.localName.Equals( (string)obj[0] ))) {
            
            t.localPoints += (int) obj[1];
            UpdateList();
            Debug.Log("UPDATING OVER NETWORK");
            break;
        }
    }


    public void Rpccall() {

            photonView.RPC("RPCCall", RpcTarget.AllBuffered, 1, 25);
    }

    [PunRPC]
    private void RPCCall(int i, int score) {

        inGameNetworkManager.playerStateList[i].localPoints += score;
        UpdateList();
    }
}//decide rpc or raise event

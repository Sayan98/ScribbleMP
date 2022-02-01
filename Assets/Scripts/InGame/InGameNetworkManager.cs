using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameNetworkManager : MonoBehaviourPunCallbacks {

    [SerializeField] private Text logText;

    [Header("Player List State")]
    public RectTransform playerListContentT;
    public List<PlayerStateInfo> playerStateList = new List<PlayerStateInfo>();
    [SerializeField] private PlayerStateInfo playerStatePrefab;


    private void Start() {
        
        if(!PhotonNetwork.IsConnected) {

            logText.text = "Disconnected";
            return;
        }
        logText.text = "Room: " + PhotonNetwork.CurrentRoom.Name + ", Players Joined: " + PhotonNetwork.PlayerList.Length;

        foreach  (var info in PhotonNetwork.PlayerList)  
            PlayerListSpawn(info);
    }


    private void PlayerListSpawn(Player info) {
    //SPAWN PLAYER LIST

        var playerStateInfo = Instantiate(playerStatePrefab, playerListContentT);
        playerStateInfo.SetPlayerName(info);
        playerStateList.Add(playerStateInfo);

        var v = playerListContentT.anchoredPosition;
        v.y -= 75;
        playerListContentT.anchoredPosition = v;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {

        Debug.Log("Players Joined " + newPlayer.NickName);
        PlayerListSpawn(newPlayer);
    }


    /*//RPC TEST
    public void callRPC() {

        for (int i = 0; i < 50; i++)
            photonView.RPC("RPCCall", RpcTarget.All, PhotonNetwork.LocalPlayer, System.DateTime.Now.Second.ToString());
    
    
    }

    [PunRPC]
    private void RPCCall(Player player, string s) {

        Debug.Log(player.NickName + " : " + s + " :: " + System.DateTime.Now.Second);
    }*/


}

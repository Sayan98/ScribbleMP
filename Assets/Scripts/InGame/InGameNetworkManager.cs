using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class InGameNetworkManager : MonoBehaviourPunCallbacks {

    [SerializeField] private Text _logText;

    [Header("Player List State")]
    public RectTransform _playerListContentT;
    public List<PlayerStateInfo> _playerStateList = new List<PlayerStateInfo>();
    [SerializeField] private PlayerStateInfo _playerStatePrefab;


    private void Start() {
        
        if(!PhotonNetwork.IsConnected) {

            _logText.text = "Disconnected";
            return;
        }
        _logText.text = "Room: " + PhotonNetwork.CurrentRoom.Name + ", Players Joined: " + PhotonNetwork.PlayerList.Length;

        foreach  (var info in PhotonNetwork.PlayerList)  
            PlayerListSpawn(info);
    }


    public void PlayerListSpawn(Player info) {
    //SPAWN PLAYER LIST

        var playerStateInfo = Instantiate(_playerStatePrefab, _playerListContentT);
        playerStateInfo.SetPlayerName(info);
        _playerStateList.Add(playerStateInfo);

        var v = _playerListContentT.anchoredPosition;
        v.y -= 75;
        _playerListContentT.anchoredPosition = v;
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

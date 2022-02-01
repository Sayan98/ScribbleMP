using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

public class PlayerStateInfo : MonoBehaviour {

    public Text nameText, pointsText, rankText;
    public Image playerListImage, winColorImage;

    public int localPoints, localRank;
    public string localName;
    
    private ControlCenter _controlCenter;


    private void Awake() {
        
        localPoints = 0;
    }

    private void Start() {

        _controlCenter = GameObject.Find("ControlCenter").GetComponent<ControlCenter>();
        
        playerListImage.color = _controlCenter.GetTokenColor();
    }

    public void SetPlayerName(Player info) {

        localRank = info.ActorNumber;
        localName = info.NickName;
        gameObject.name = localName;
        rankText.text = "#" + localRank;
        nameText.text = info.NickName.Equals(PhotonNetwork.NickName)? info.NickName + "(You)" : info.NickName;
        pointsText.text = "Points: " + localPoints;
    }

    public void SetRankAndPoints() {
        
        localRank = transform.GetSiblingIndex() + 1;

        rankText.text = "#" + localRank;
        pointsText.text = "Points: " + localPoints;
    }
    
}

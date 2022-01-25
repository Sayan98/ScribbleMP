using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerStateInfo : MonoBehaviour {

    public Text _NameText, _PointsText, _RankText;
    public Image _PlayerListImage, _WinColorImage;
    
    public int _LocalPoints;
    public int _LocalRank;
    public string _localName;
    
    private ControlCenter _controlCenter;


    private void Awake() {
        
        _LocalPoints = 0;
    }

    private void Start() {

        _controlCenter = GameObject.Find("ControlCenter").GetComponent<ControlCenter>();
        
        _PlayerListImage.color = _controlCenter.GetTokenColor();
    }

    public void SetPlayerName(Player info) {
        
        _LocalRank = info.ActorNumber;
        _localName = info.NickName;
        _RankText.text = "#" + _LocalRank;
        _NameText.text = info.NickName.Equals(PhotonNetwork.NickName)? info.NickName + "(You)" : info.NickName;
        _PointsText.text = "Points: " + _LocalPoints;
    }

    public void SetRankAndPoints() {
        
        _LocalRank = transform.GetSiblingIndex() + 1;

        _RankText.text = "#" + _LocalRank;
        _PointsText.text = "Points: " + _LocalPoints;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

public class PlayerListInfo : MonoBehaviour {

    public Text nameText;
    public Color[] colors;
    public Image image;

    public void SetPlayerName(Player info) {

        nameText.text = info.NickName;
    }

    public void ChangePlayerState(int i) {

        image.color = colors[i];
    }
    
}

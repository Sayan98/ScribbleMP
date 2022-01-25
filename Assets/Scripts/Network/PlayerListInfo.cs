using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListInfo : MonoBehaviour {

    public Text _nameText;
    public Color[] colors;
    public Image image;

    public void SetPlayerName(Player info) {

        _nameText.text = info.NickName;
    }

    public void ChangePlayerState(int i) {

        image.color = colors[i];
    }
    
}

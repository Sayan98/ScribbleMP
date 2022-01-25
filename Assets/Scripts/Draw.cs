using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Serialization;


public class Draw : MonoBehaviourPunCallbacks {

    [FormerlySerializedAs("_Drawing")] public bool drawing;

    [FormerlySerializedAs("_logText")] [SerializeField] private Text logText;
    [SerializeField] private Camera _Camera;
    [FormerlySerializedAs("_brushHolder")] [SerializeField] private Transform brushHolder;
    [FormerlySerializedAs("_brushPrefab")] [SerializeField] private LineRenderer brushPrefab;
    private LineRenderer _brush;
    private Vector3 _inputPos;
    private object[] _brushData;
    private byte _eventCode;


    private void Awake() {

        drawing = false;
        _eventCode = 0;
        _brushData = new object[3];
    }

    #region Brush
    public void OnMouseDown() {

        drawing = true;
        _brush = Instantiate(brushPrefab, brushHolder);
        SetBrushPosition();
        UpdateBrushPosition(true);
    }

    public void OnDrag() {

        _brush.positionCount++;
        SetBrushPosition();
        UpdateBrushPosition(false);
    }

    public void OnMouseUp() {

        drawing = false;

        if(PhotonNetwork.IsConnected)
            logText.text = "Ping: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime;
    }

    private void SetBrushPosition() {

        _inputPos = _Camera.ScreenToWorldPoint(Input.mousePosition);
        _inputPos.z = -5;
        _brush.SetPosition(_brush.positionCount - 1, _inputPos);
    }
    #endregion


    private void UpdateBrushPosition(bool newBrush) {
    //CALLING EVENT

        _brushData[0] = _brush.positionCount;
        _brushData[1] = _brush.GetPosition(_brush.positionCount-1);
        _brushData[2] = newBrush;

        _eventCode = 1;
        
        PhotonNetwork.RaiseEvent(_eventCode, _brushData, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    private void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += OnEvent; }
    private void OnDisable(){ PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; }

    private void OnEvent(EventData eventData) {
    //EVENT

        if(eventData.Code.Equals(1)) {

            _brushData = (object[])eventData.CustomData;

            if((bool)_brushData[2])
                _brush = Instantiate(brushPrefab, brushHolder);

            _brush.positionCount = (int)_brushData[0];
            _brush.SetPosition(_brush.positionCount-1, (Vector3)_brushData[1]);
        }
    }

}
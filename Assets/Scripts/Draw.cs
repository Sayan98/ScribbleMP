using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Serialization;


public class Draw : MonoBehaviourPunCallbacks {

    public bool drawing;//TODO REMOVE

    [SerializeField] private Text logText;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform brushHolder;
    [SerializeField] private LineRenderer brushPrefab;
    
    private LineRenderer _brush;
    private Color _brushColor;
    
    private Vector3 _inputPos;
    private object[] _brushData;

    [SerializeField] private ControlCenter controlCenter;


    private void Awake() {

        drawing = false;
        _brushColor = new Color(1, 1, 1, 1);
        _brushData = new object[3];
    }

    #region Brush
    public void OnMouseDown() {

        drawing = true;
        _brush = Instantiate(brushPrefab, brushHolder);
        _brush.startColor = _brush.endColor = _brushColor;
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

        _inputPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _inputPos.z = -5;
        _brush.SetPosition(_brush.positionCount - 1, _inputPos);
    }
    #endregion


    private void UpdateBrushPosition(bool newBrush) {
    //CALLING EVENT

        var positionCount = _brush.positionCount;
        _brushData[0] = newBrush;
        _brushData[1] = positionCount;
        _brushData[2] = _brush.GetPosition(positionCount-1);

        PhotonNetwork.RaiseEvent(0, _brushData, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    public override void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += OnEvent; }
    public override void OnDisable() { PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; }

    private void OnEvent(EventData eventData) { 
    //EVENT BRUSH
    
        switch (eventData.Code) {
        
            case 0:
                Debug.Log("DRAWING");
                _brushData = (object[])eventData.CustomData;
                if((bool)_brushData[0])
                    _brush = Instantiate(brushPrefab, brushHolder);
                _brush.positionCount = (int)_brushData[1];
                _brush.SetPosition(_brush.positionCount-1, (Vector3)_brushData[2]);
                break;
            case 1:
                Debug.Log("CLEANING");
                foreach (Transform child in brushHolder)
                    Destroy(child.gameObject);
                break;
        }
    }

    private void ClearScreen( ) {
        
        foreach (Transform child in brushHolder)
            Destroy(child.gameObject);
    }

    public void ClearScreenCall() { 
        
        //ClearScreen();
        var data = new EventData {

            Code = 1
        };
        OnEvent(new EventData(){Code = 1});
        //return;
        var reo = RaiseEventOptions.Default;
        reo.Receivers = ReceiverGroup.Others;
        PhotonNetwork.RaiseEvent(1, null, reo, SendOptions.SendReliable);
    }

    public void BrushAttribute(int i) {

        _brushColor = controlCenter.colorPalette.colorPalette[i];
    }
//SETUP EVENTDATA FOR LOCAL ALL ALSO, clear draw code
}
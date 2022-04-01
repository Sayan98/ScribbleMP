using System;
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

    private EventData _eventData;
    private List<object> _brushData;
    private Color _brushColor;
    private Vector3 _brushInputPos;

    [SerializeField] private ControlCenter controlCenter;

    private void Awake() {

        drawing = false;
        
        _eventData = new EventData();
        
        _brushData = new List<object>();
        
        _brushColor = new Color(1, 1, 1, 1);
        _brushInputPos = Vector2.zero;
    }

    #region BrushInput
    public void OnMouseDown() {

        drawing = true;
        
        _brushData.Clear();
        _brushData.Add(true);
        _brushData.Add((Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition));
        
        _brushData.Add(_brushColor.r);
        _brushData.Add(_brushColor.g);
        _brushData.Add(_brushColor.b);
        
        _eventData.Code = 1;
        OnEvent(_eventData);
        UpdateBrushPosition();
    }

    public void OnDrag() {
        
        _brushData.Clear();
        _brushData.Add(false);
        _brushData.Add((Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition));
        
        OnEvent(_eventData);
        UpdateBrushPosition();
    }

    public void OnMouseUp() {

        drawing = false;

        if(PhotonNetwork.IsConnected)
            logText.text = "Ping: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime;
    }
    #endregion


    private void UpdateBrushPosition() {
    //CALLING EVENT   
        PhotonNetwork.RaiseEvent(_eventData.Code, _brushData, null, SendOptions.SendUnreliable);
    }

    public override void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += OnEvent; }
    public override void OnDisable() { PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; }

    private void OnEvent(EventData eventData) { 
    //EVENT BRUSH
        switch (eventData.Code) {
        
            case 1: drawing = true;//TODO REMOVE

                if (eventData.Sender > 0) { 
                    
                    _brushData.Clear();
                    _brushData.AddRange((object[]) eventData.CustomData);
                }

                SetBrushPosition();
                break;
            case 2: Debug.Log("CLEANING");//TODO REMOVE
                
                foreach (Transform child in brushHolder)
                    Destroy(child.gameObject);
                break;
        }
    }

    private void SetBrushPosition() {

        if(!(bool)_brushData[0]) 
            _brush.positionCount++;
        else {

            _brush = Instantiate(brushPrefab, brushHolder);
            _brush.startColor = _brush.endColor = new Color((float)_brushData[2], (float)_brushData[3], (float)_brushData[4], 1);
            _brush.positionCount = 1;
        }

        _brushInputPos = (Vector2)_brushData[1];
        _brushInputPos.z = -5;
        _brush.SetPosition(_brush.positionCount - 1, _brushInputPos);
    }


    public void ClearScreen() { 
    //ERASER
        _eventData.Code = 2;
        OnEvent(_eventData);
        PhotonNetwork.RaiseEvent(_eventData.Code, null, null, SendOptions.SendReliable);
    }
    
    public void BrushAttribute(int i) {
    //GET BRUSH COLOR
        _brushColor = controlCenter.colorPalette.colorPalette[i];
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightCamera : MonoBehaviour
{
    private Vector3 _cameraStartPosition = new Vector3(0, 2f, -10);
    private GameObject _fightCamera;

    private float _cameraValueXAxis;
    private float _cameraValueZAxis;

    private int _cameraValueZAxisModifier = -4;

    public static GameObject _playerOne;
    public static GameObject _opponent;

    private Vector3 _playerPosition;
    private Vector3 _opponentPosition;

    private void Start()
    {
        _fightCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _fightCamera.transform.position = _cameraStartPosition;
    }
    private void Update()
    {
        UpdatePlayerPosition();
        UpdateOpponentPosition();
        UpdateCameraPosition();
    }
    private void UpdatePlayerPosition()
    {
        _playerPosition = _playerOne.transform.position;
    }

    private void UpdateOpponentPosition()
    {
        _opponentPosition = _opponent.transform.position;
    }

    private void UpdateCameraPosition()
    {
        _cameraValueXAxis = (_playerPosition.x + _opponentPosition.x) / 2;

        if(_playerPosition.x < _opponentPosition.x)
            _cameraValueZAxis = _playerPosition.x - _opponentPosition.x;

        if (_playerPosition.x > _opponentPosition.x)
            _cameraValueZAxis = _opponentPosition.x - _playerPosition.x;

        if (_cameraValueZAxis > -8f) 
            _cameraValueZAxis = -8f;

        if (_cameraValueZAxis +_cameraValueZAxisModifier < -15.5f)
            _cameraValueZAxis = -15.5f - _cameraValueZAxisModifier;

        _fightCamera.transform.position = new Vector3(_cameraValueXAxis, 2f, 
            _cameraValueZAxis + _cameraValueZAxisModifier);
    }
}

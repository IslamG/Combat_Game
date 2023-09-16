using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGUI : MonoBehaviour
{
    private int _currentTimerValue;

    public float _maximumPlayerHealth;
    public float _currentPlayerHealth;
    public float _playerHealthBarLength;

    public float _maximumOpponentHealth;
    public float _currentOpponentHealth;
    public float _opponentHealthBarLength;

    private Vector2 _healthBarSize;
    private Vector2 _fightGUITimerSize;

    public Texture2D _healthBarMinTexture;
    public Texture2D _healthBarMaxTexture; 
    public Texture2D _healthBarOutlineTexture; 

    public float _fightGUIHeightPosition;
    private float _fightGUIOffset;
    private float _fightGUIPosModifier;

    private GUIStyle _fightGUISkin;


    // Start is called before the first frame update
    void Start()
    {
        _fightGUIOffset = Screen.width / 20;
        _fightGUITimerSize = new Vector2(Screen.width / 7.5f, Screen.height / 7.5f);

        _healthBarSize = new Vector2(Screen.width / 2.5f, Screen.height / 15f);
    }

    // Update is called once per frame
    void Update()
    {
        _currentPlayerHealth = PlayerOneHealth._currentPlayerHealth;
        _currentOpponentHealth = OpponentHealth._currentOpponentHealth;
        _maximumPlayerHealth = PlayerOneHealth._maximumPlayerHealth;
        _maximumOpponentHealth = OpponentHealth._maximumOpponentHealth;

        _playerHealthBarLength = (_currentPlayerHealth / _maximumPlayerHealth);
        _opponentHealthBarLength = (_currentOpponentHealth / _maximumOpponentHealth);
    }

    private void LateUpdate()
    {
        _currentTimerValue = FightManager._currentFightTimer;
    }

    private void OnGUI()
    {
        _fightGUISkin = new GUIStyle(GUI.skin.GetStyle("lable"));
        _fightGUISkin.fontSize = Screen.width / 15;
        _fightGUISkin.alignment = TextAnchor.MiddleCenter;
        _fightGUIHeightPosition = _fightGUIOffset / 40;

        if (_currentTimerValue >= 10)
        {
            GUI.Label(new Rect(
                Screen.width / 2 - (_fightGUITimerSize.x /2),
                _fightGUIHeightPosition, 
                _fightGUITimerSize.x, _fightGUITimerSize.y),
                _currentTimerValue.ToString(), _fightGUISkin);
        }

        if (_currentTimerValue < 10)
        {
            GUI.Label(new Rect(
                Screen.width / 2 - (_fightGUITimerSize.x / 2),
                _fightGUIHeightPosition,
                _fightGUITimerSize.x, _fightGUITimerSize.y),
                "0" + _currentTimerValue.ToString(), _fightGUISkin);
        }

        //Player Health
        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
        GUI.DrawTexture(new Rect(
            _fightGUIOffset, _fightGUIOffset / 2, 
            _healthBarSize.x,// * _playerHealthBarLength, 
            _healthBarSize.y),
            _healthBarMinTexture);
        GUI.DrawTexture(new Rect(
            _fightGUIOffset, 
            _fightGUIOffset / 2, 
            _healthBarSize.x * _playerHealthBarLength,
            _healthBarSize.y),
            _healthBarMaxTexture);
        GUI.DrawTexture(new Rect(
            _fightGUIOffset - _fightGUIPosModifier/2, 
            _fightGUIOffset / 2 - _fightGUIPosModifier / 2,
            _healthBarSize.x  + _fightGUIPosModifier,
            _healthBarSize.y + _fightGUIPosModifier),
            _healthBarOutlineTexture);
        GUI.EndGroup();

        //Opponent Health
        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
        GUI.DrawTexture(new Rect(
            Screen.width - (_fightGUIOffset + _healthBarSize.x), 
            _fightGUIOffset / 2,
            _healthBarSize.x,
            _healthBarSize.y),
            _healthBarMinTexture);
        GUI.DrawTexture(new Rect(
            Screen.width - (_fightGUIOffset + _healthBarSize.x),
            _fightGUIOffset / 2,
            _healthBarSize.x * _opponentHealthBarLength,
            _healthBarSize.y),
            _healthBarMaxTexture);
        GUI.DrawTexture(new Rect(
            Screen.width - (
            _fightGUIOffset + _healthBarSize.x + _fightGUIPosModifier/2),
            _fightGUIOffset / 2 - _fightGUIPosModifier/2,
            _healthBarSize.x + _fightGUIPosModifier,
            _healthBarSize.y + _fightGUIPosModifier),
            _healthBarOutlineTexture);
        GUI.EndGroup();
    }
}

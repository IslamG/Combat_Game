using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1SpecialMove1 : MonoBehaviour
{
    public float _timerDefault = 2.0f;
    public float _timerCompleteButtonCombination;


    public float _pauseDefault = 1.0f;
    public float _pauseBetweenPresses;


    public int _buttonCombinationProgress = 0;

    public float _button1;
    public float _button2;
    public bool _button3;

    private GameObject _playerPosition; 
    private GameObject _opponentPosition; 

    public string[] _keyCombo = new string[] 
    { 
        "Button1", "Button2", "Button3"
    };
    void Start()
    {
        _playerPosition = FightCamera._playerOne;
        _opponentPosition = FightCamera._opponent;

        _button1 = 0;
        _button2 = 0;
        _button3 = false;

        _pauseBetweenPresses = _pauseDefault;
        _timerCompleteButtonCombination = _timerDefault;
    }

    void Update()
    {
        _button1 = Input.GetAxis("Vertical");
        _button2 = Input.GetAxis("Horizontal");
        _button3 = Input.GetButtonDown("Fire3");

        if((_buttonCombinationProgress == 0) && _button1 <= -0.5f)
        {
            StartCoroutine("SpecialMove1");
            _buttonCombinationProgress ++;
        }
    }

    IEnumerator SpecialMove1()
    {
        while (Application.isPlaying)
        {
            Timer();
            Pause();

            if (_playerPosition.transform.position.x < _opponentPosition.transform.position.x) 
            {
                if (_buttonCombinationProgress == 1 && _button2 >= 0.5f)
                {
                    _pauseBetweenPresses = _pauseDefault;
                    _buttonCombinationProgress++;
                }
            }

            if (_playerPosition.transform.position.x > _opponentPosition.transform.position.x)
            {
                if (_buttonCombinationProgress == 1 && _button2 <= 0.5f)
                {
                    _pauseBetweenPresses = _pauseDefault;
                    _buttonCombinationProgress++;
                }
            }


            
            if (_buttonCombinationProgress == 2 && _button3)
            {
                _pauseBetweenPresses = _pauseDefault;
                _buttonCombinationProgress++;
            }
            if(_buttonCombinationProgress >= _keyCombo.Length)
            {
                PlayerOneMovement._specialMoveOneActive = true;

                GetComponent<PlayerOneMovement>()._playerOneStates = PlayerOneMovement.PlayerOneStates.SpecialMoveOne;

                StopAllCoroutines();
                SM1Reset();
            }
            yield return null;
        }
    }
    private void Timer()
    {
        _timerCompleteButtonCombination -= Time.deltaTime;

        if(_timerCompleteButtonCombination < 0)
        {
            _timerCompleteButtonCombination = 0;
        }

        if(_timerCompleteButtonCombination == 0)
        {
            _buttonCombinationProgress = 0;
            _timerCompleteButtonCombination = _timerDefault;
            _pauseBetweenPresses = _pauseDefault;

            StopAllCoroutines();
        }
    }

    private void Pause()
    {
        _pauseBetweenPresses -= Time.deltaTime;

        if (_pauseBetweenPresses < 0)
        {
            _pauseBetweenPresses = 0;
        }

        if (_pauseBetweenPresses == 0)
        {
            _pauseBetweenPresses = 0;
            _pauseBetweenPresses = _pauseDefault;
            _timerCompleteButtonCombination = _timerDefault;

            StopAllCoroutines();
        }
    }

    private void SM1Reset()
    {
        _button1 = 0;
        _button2 = 0;
        _button3 = false;

        _buttonCombinationProgress = 0;

        _pauseBetweenPresses = _pauseDefault;
        _timerCompleteButtonCombination = _timerDefault;
    }
}

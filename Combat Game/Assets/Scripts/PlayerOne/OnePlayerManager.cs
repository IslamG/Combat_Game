using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnePlayerManager : GameManager
{
    private Vector3 _playerOnePosition; //= new Vector3();
    private Vector3 _playerOneRotation;// = new Vector3();

    private GameObject _playerOneCharacter;

    private bool _returnChar1;
    private bool _returnChar2;
    private bool _returnChar3;
    private bool _returnChar4;

    // Start is called before the first frame update
    void Start()
    {
        _playerOnePosition = _playerStartingPosition;
        _playerOneRotation = _playerStartingRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AssignPlayerChoice()
    {
        _returnChar1 = ChooseCharacterManager._char1;
        _returnChar2 = ChooseCharacterManager._char2;
        _returnChar3 = ChooseCharacterManager._char3;
        _returnChar4 = ChooseCharacterManager._char4;
    }

    void LoadPlayerOneCharacter()
    {
        if (_playerOneCharacter != null)
            return;

        AssignPlayerChoice();

        if (_returnChar1)
            _playerOneCharacter = Instantiate(Resources.Load("Char1"), _playerOnePosition, Quaternion.Euler(_playerOneRotation)) as GameObject;
        if (_returnChar2)
            _playerOneCharacter = Instantiate(Resources.Load("Char2"), _playerOnePosition, Quaternion.Euler(_playerOneRotation)) as GameObject;
        if (_returnChar3)
            _playerOneCharacter = Instantiate(Resources.Load("Char3"), _playerOnePosition, Quaternion.Euler(_playerOneRotation)) as GameObject; 
        if (_returnChar4)
            _playerOneCharacter = Instantiate(Resources.Load("Char4"), _playerOnePosition, Quaternion.Euler(_playerOneRotation)) as GameObject;

        //Debug.Log("p1 character " + _playerOneCharacter);

        FightCamera._playerOne = _playerOneCharacter;
        OpponentAI._playerOne = _playerOneCharacter;
        PlayerOneMovement._playerOne = _playerOneCharacter;

        _playerOneCharacter.GetComponent<PlayerOneMovement>().enabled = true;
        _playerOneCharacter.GetComponent<PlayerOneHealth>().enabled = true;

        _playerOneCharacter.GetComponent<OpponentAI>().enabled = false;
        _playerOneCharacter.GetComponent<OpponentHealth>().enabled = false;
    }
}

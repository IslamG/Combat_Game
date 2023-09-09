using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    public Vector3 _opponentPosition = new Vector3();
    public Vector3 _opponentRotation = new Vector3();

    public GameObject _currentOpponent;

    public string _selectedOpponent = "";

    public int _opponentCounter;

    public string[] _opponentOrder = new string[]
    {
        "Char1", "Char2", "Char3", "Char4"
    };

    private bool _returnChar1;
    private bool _returnChar2;
    private bool _returnChar3;
    private bool _returnChar4;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        
        _opponentCounter = 0;


        for(int i = 0; i < _opponentOrder.Length; i++)
        {
            string temp = _opponentOrder[i];
            int randomOrder = Random.Range(i, _opponentOrder.Length);
            _opponentOrder[i] = _opponentOrder[randomOrder];
            _opponentOrder[randomOrder] = temp;
        }
        
        _selectedOpponent = _opponentOrder[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetAssignedCharacter()
    {
        _returnChar1 = ChooseCharacterManager._char1;
        _returnChar2 = ChooseCharacterManager._char2;
        _returnChar3 = ChooseCharacterManager._char3;
        _returnChar4 = ChooseCharacterManager._char4;
    }

    void LoadCurrentOpponent()
    {
        GetAssignedCharacter();

        if (_selectedOpponent == "Char1")
            _currentOpponent = _returnChar1 ?
             Instantiate(Resources.Load(_selectedOpponent + "Alt"), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject :
             Instantiate(Resources.Load(_selectedOpponent), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject;
        if (_selectedOpponent == "Char2")
            _currentOpponent = _returnChar2 ?
             Instantiate(Resources.Load(_selectedOpponent + "Alt"), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject :
             Instantiate(Resources.Load(_selectedOpponent), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject;
        if (_selectedOpponent == "Char3")
            _currentOpponent = _returnChar3 ?
             Instantiate(Resources.Load(_selectedOpponent + "Alt"), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject :
             Instantiate(Resources.Load(_selectedOpponent), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject;
        if (_selectedOpponent == "Char4")
            _currentOpponent = _returnChar4 ?
             Instantiate(Resources.Load(_selectedOpponent + "Alt"), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject :
             Instantiate(Resources.Load(_selectedOpponent), _opponentPosition, Quaternion.Euler(_opponentRotation)) as GameObject;

        //Debug.Log("op character " + _currentOpponent);

        FightCamera._opponent = _currentOpponent;

        _currentOpponent.GetComponent<PlayerOneMovement>().enabled = false;
        _currentOpponent.GetComponent<PlayerOneHealth>().enabled = false;

        _currentOpponent.GetComponent<OpponentAI>().enabled = true;
        _currentOpponent.GetComponent<OpponentHealth>().enabled = true;
    }

    private void SetOpponent()
    {

    }
}

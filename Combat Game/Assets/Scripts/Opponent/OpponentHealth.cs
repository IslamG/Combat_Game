using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentHealth : MonoBehaviour
{
    public int _minimimOpponentHealth = 0;
    public int _maximimOpponentHealth = 100;
    public int _currentOpponentHealth;

    private bool _isOpponentDefeated; 

    // Start is called before the first frame update
    void Start()
    {
        _currentOpponentHealth = _maximimOpponentHealth;
        _isOpponentDefeated= false;

    }

    // Update is called once per frame
    void Update()
    {
        if (_isOpponentDefeated) 
            return;

        if (_currentOpponentHealth <= _minimimOpponentHealth)
        {
            _currentOpponentHealth = _minimimOpponentHealth;
            _isOpponentDefeated = true;
            SendMessage("SetOpponentDefeated", SendMessageOptions.DontRequireReceiver);
        }
            
    }
}

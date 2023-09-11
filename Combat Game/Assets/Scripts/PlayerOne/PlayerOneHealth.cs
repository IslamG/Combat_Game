using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneHealth : MonoBehaviour
{
    public static int _minimimPlayerHealth = 0;
    public static int _maximumPlayerHealth = 100;
    public static int _currentPlayerHealth;

    void Start()
    {
        _currentPlayerHealth = _maximumPlayerHealth;
        //_isOpponentDefeated = false;

    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (_isOpponentDefeated)
    //        return;

    //    if (_currentPlayerHealth <= _minimimPlayerHealth)
    //    {
    //        _currentPlayerHealth = _minimimPlayerHealth;
    //        _isOpponentDefeated = true;
    //        SendMessage("SetOpponentDefeated", SendMessageOptions.DontRequireReceiver);
    //    }

    //}
}

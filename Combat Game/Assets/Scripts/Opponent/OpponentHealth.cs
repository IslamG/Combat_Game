using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentHealth : MonoBehaviour
{
    public static int _minimimOpponentHealth = 0;
    public static int _maximumOpponentHealth = 100;
    public static int _currentOpponentHealth;

    private bool _isOpponentDefeated; 

    void Start()
    {
        _currentOpponentHealth = _maximumOpponentHealth;
        _isOpponentDefeated= false;

    }

    void Update()
    {
        
            
    }

    public void OpponentLowPunchDamage(int _damageDealt)
    {
        if (_isOpponentDefeated)
            return;
        _currentOpponentHealth -= _damageDealt;

        SendMessageUpwards("OpponentHitByLowPunch", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }
    public void OpponentHighPunchDamage(int _damageDealt)
    {
        if (_isOpponentDefeated)
            return;
        _currentOpponentHealth -= _damageDealt;

        SendMessageUpwards("OpponentHitByHighPunch", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }
    public void OpponentLowKickDamage(int _damageDealt)
    {
        if (_isOpponentDefeated)
            return;
        _currentOpponentHealth -= _damageDealt;

        SendMessageUpwards("OpponentHitByLowKick", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }
    public void OpponentHighKickDamage(int _damageDealt)
    {
        if (_isOpponentDefeated)
            return;
        _currentOpponentHealth -= _damageDealt;

        SendMessageUpwards("OpponentHitByHighKick", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }

    private void CheckHealth()
    {
        if (_currentOpponentHealth <= _minimimOpponentHealth)
        {
            _currentOpponentHealth = _minimimOpponentHealth;
            _isOpponentDefeated = true;
            SendMessage("SetOpponentDefeated", SendMessageOptions.DontRequireReceiver);
        }
    }
}

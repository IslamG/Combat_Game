using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneHealth : MonoBehaviour
{
    public static int _minimimPlayerHealth = 0;
    public static int _maximumPlayerHealth = 100;
    public static int _currentPlayerHealth;

    private bool _isPlayerDefeated;
    void Start()
    {
        _currentPlayerHealth = _maximumPlayerHealth;
        _isPlayerDefeated = false;

    }

    void PlayerLowPunchDamage(int _damageDealt)
    {
        if (_isPlayerDefeated)
            return;

        _currentPlayerHealth -= _damageDealt;

        SendMessageUpwards("PlayerHitByLowPunch", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }
    void PlayerHighPunchDamage(int _damageDealt)
    {
        if (_isPlayerDefeated)
            return;

        _currentPlayerHealth -= _damageDealt;

        SendMessageUpwards("PlayerHitByHighPunch", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }
    void PlayerLowKickDamage(int _damageDealt)
    {
        if (_isPlayerDefeated)
            return;

        _currentPlayerHealth -= _damageDealt;

        SendMessageUpwards("PlayerHitByLowKick", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }
    void PlayerHighKickDamage(int _damageDealt)
    {
        if (_isPlayerDefeated)
            return;

        _currentPlayerHealth -= _damageDealt;

        SendMessageUpwards("PlayerHitByHighKick", SendMessageOptions.DontRequireReceiver);

        CheckHealth();
    }

    private void CheckHealth()
    {
        if (_currentPlayerHealth <= _minimimPlayerHealth)
        {
            _currentPlayerHealth = _minimimPlayerHealth;
            _isPlayerDefeated = true;
            SendMessage("SetPlayerDefeated", SendMessageOptions.DontRequireReceiver);
        }
    }
}

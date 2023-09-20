using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPunchLow : MonoBehaviour
{
    public static Vector3 _playerImpactPoint;

    public float _nextPunchIsAllowed = -1f;
    public float _attackDelay = 1f;

    private Collider _hitCollider;
    private bool _isOpponentPunchingLow;
    private int _lowPunchDamageValue;

    private GameObject _playerOne;
    private PlayerOneMovement _playerOneMovement;

    private void Start()
    {
        _playerImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
        
        LowPunchDamageSetUp();
    }
    private void Update()
    {
        _isOpponentPunchingLow = OpponentAI._isOpponentPunchingLow;
        _hitCollider.enabled = _isOpponentPunchingLow || OpponentAI._isOpponentPunchingHigh;
    }

    void OnTriggerStay(Collider _playerBodyHit)
    {
        if (_playerBodyHit.CompareTag("BodyHitBox")
            && _isOpponentPunchingLow
            && Time.time >= _nextPunchIsAllowed)
        {
            BodyPunch();
            _nextPunchIsAllowed = Time.time + _attackDelay;
        };

        _playerBodyHit.ClosestPointOnBounds(transform.position);
        _playerImpactPoint = _playerBodyHit.transform.position;
    }
    private void LowPunchDamageSetUp()
    {
        _lowPunchDamageValue = GetComponentInParent<CharacterStats>()._lowPunchDamage;
    }
    void BodyPunch()
    {
        Debug.Log("Hit body");
        _playerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByLowPunch;
        
        _playerOne = FightCamera._playerOne;
        _playerOneMovement = _playerOne.GetComponent<PlayerOneMovement>();

        PlayerOneHealth _tempDamage = _playerOne.GetComponent<PlayerOneHealth>();

        _tempDamage.PlayerLowPunchDamage(_lowPunchDamageValue);
    }
}
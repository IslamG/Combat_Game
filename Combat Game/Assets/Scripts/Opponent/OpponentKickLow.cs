using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   
public class OpponentKickLow : MonoBehaviour
{
    public static Vector3 _playerImpactPoint;

    public float _nextKickIsAllowed = -1f;
    public float _attackDelay = 1f;

    private Collider _hitCollider;
    private bool _isOpponentKickingLow;
    private int _lowKickDamageValue;

    private GameObject _playerOne;
    private PlayerOneMovement _playerOneMovement;

    private void Start()
    {
        _playerImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
       
        LowKickDamageSetUp();
    }
    private void Update()
    {
        _isOpponentKickingLow = OpponentAI._isOpponentKickingLow;
        _hitCollider.enabled = _isOpponentKickingLow || OpponentAI._isOpponentKickingHigh;
    }

    void OnTriggerStay(Collider _playerBodyHit)
    {
        if (_playerBodyHit.CompareTag("BodyHitBox")
            && _isOpponentKickingLow
            && Time.time >= _nextKickIsAllowed)
        {
            BodyKick();
            _nextKickIsAllowed = Time.time + _attackDelay;
        }

        _playerBodyHit.ClosestPointOnBounds(transform.position);
        _playerImpactPoint = _playerBodyHit.transform.position;
    }
    private void LowKickDamageSetUp()
    {
        _lowKickDamageValue = GetComponentInParent<CharacterStats>()._lowKickDamage;
    }
    void BodyKick()
    {
        Debug.Log("Hit body");
        _playerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByLowKick;

        _playerOne = FightCamera._playerOne;
        _playerOneMovement = _playerOne.GetComponent<PlayerOneMovement>();

        PlayerOneHealth _tempDamage = _playerOne.GetComponent<PlayerOneHealth>();

        _tempDamage.PlayerLowKickDamage(_lowKickDamageValue);
    }
}



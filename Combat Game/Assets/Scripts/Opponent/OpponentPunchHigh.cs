using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPunchHigh : MonoBehaviour
{
    public static Vector3 _playerImpactPoint;

    public float _nextPunchIsAllowed = -1f;
    public float _attackDelay = 1f;

    private Collider _hitCollider;
    private bool _isOpponentPunchingHigh;
    private int _highPunchDamageValue;

    private GameObject _playerOne;
    private PlayerOneMovement _playerOneMovement;

    private void Start()
    {
        _playerImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
        
        HighPunchDamageSetUp();
    }
    private void Update()
    {
        _isOpponentPunchingHigh = OpponentAI._isOpponentPunchingHigh;
        _hitCollider.enabled = _isOpponentPunchingHigh || OpponentAI._isOpponentPunchingLow;
    }
    void OnTriggerStay(Collider _playerHeadHit)
    {
        if (_playerHeadHit.CompareTag("HeadHitBox")
            && _isOpponentPunchingHigh
            && Time.time >= _nextPunchIsAllowed)
        {
            HeadPunch();
            _nextPunchIsAllowed = Time.time + _attackDelay;
        }

        _playerHeadHit.ClosestPointOnBounds(transform.position);
        _playerImpactPoint = _playerHeadHit.transform.position;
    }
    private void HighPunchDamageSetUp()
    {
        _highPunchDamageValue = GetComponentInParent<CharacterStats>()._highPunchDamage;
    }
    void HeadPunch()
    {
        Debug.Log("Hit head");
        _playerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByHighPunch;

        _playerOne = FightCamera._playerOne;
        _playerOneMovement = _playerOne.GetComponent<PlayerOneMovement>();

        PlayerOneHealth _tempDamage = _playerOne.GetComponent<PlayerOneHealth>();

        _tempDamage.PlayerHighPunchDamage(_highPunchDamageValue);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
     
public class OpponentKickHigh : MonoBehaviour
{
    //public static Vector3 _playerImpactPoint;

    public float _nextKickIsAllowed = -1f;
    public float _attackDelay = 1f;

    private Collider _hitCollider;
    private bool _isOpponentKickingHigh;
    private int _highKickDamageValue;

    private GameObject _playerOne;
    private PlayerOneMovement _playerOneMovement;

    private void Start()
    {
        
        //_playerImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
        
        HighKickDamageSetUp();
    }
    private void Update()
    {
        _isOpponentKickingHigh = OpponentAI._isOpponentKickingHigh;
        _hitCollider.enabled = _isOpponentKickingHigh || OpponentAI._isOpponentKickingLow;
    }

    void OnTriggerStay(Collider _playerHeadHit)
    {
        if (_playerHeadHit.CompareTag("BodyHitBox") 
            && _isOpponentKickingHigh
            && Time.time >= _nextKickIsAllowed)
        {
            HeadKick();
            _nextKickIsAllowed = Time.time + _attackDelay;
        }

        //_playerHeadHit.ClosestPointOnBounds(transform.position);
        //_playerImpactPoint = _playerHeadHit.transform.position;
    }
    private void HighKickDamageSetUp()
    {
        _highKickDamageValue = GetComponentInParent<CharacterStats>()._highKickDamage;
    }
    void HeadKick()
    {
        Debug.Log("Hit body");
        


        _playerOne = FightCamera._playerOne;
        Debug.Log("player one  being hit2 " + _playerOne);
        _playerOneMovement = _playerOne.GetComponent<PlayerOneMovement>();
        _playerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByHighKick;

        PlayerOneHealth _tempDamage = _playerOne.GetComponent<PlayerOneHealth>();

        _tempDamage.PlayerHighKickDamage(_highKickDamageValue);
    }
}
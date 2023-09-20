using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickHigh : MonoBehaviour
{
    public static Vector3 _opponentImpactPoint;
    private Collider _hitCollider;
    private bool _isPlayerKickingHigh;
    public float _nextKickIsAllowed = -1f;
    public float _attackDelay = 1f;
    private int _highKickDamageValue;

    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
        HighKickDamageSetUp();
    }
    private void Update()
    {
        _isPlayerKickingHigh = PlayerOneMovement._isPlayerKickingHigh;
        _hitCollider.enabled = _isPlayerKickingHigh || PlayerOneMovement._isPlayerKickingLow;
    }

    void OnTriggerStay(Collider _opponentHeadHit)
    {
        //if (_isPlayerKickingHigh) return;

        if (_opponentHeadHit.CompareTag("BodyHitBox") 
            && _isPlayerKickingHigh
            && Time.time >= _nextKickIsAllowed)
        {
            HeadKick();
            _nextKickIsAllowed = Time.time + _attackDelay;
        }

        _opponentHeadHit.ClosestPointOnBounds(transform.position);
        _opponentImpactPoint = _opponentHeadHit.transform.position;
    }

    void HeadKick()
    {
        Debug.Log("Hit body with high kick");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByHighKick;
        
        OpponentHealth _tempDamage = FightCamera._opponent.GetComponent<OpponentHealth>();

        _tempDamage.OpponentHighKickDamage(_highKickDamageValue);
    }

    private void HighKickDamageSetUp()
    {
        _highKickDamageValue = GetComponentInParent<CharacterStats>()._highKickDamage;
    }
}
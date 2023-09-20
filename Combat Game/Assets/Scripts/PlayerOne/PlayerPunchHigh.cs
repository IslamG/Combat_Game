using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunchHigh : MonoBehaviour
{
    public static Vector3 _opponentImpactPoint;

    public float _nextPunchIsAllowed = -1f;
    public float _attackDelay = 1f;

    private Collider _hitCollider;
    private bool _isPlayerPunchingHigh;
    private int _highPunchDamageValue;

    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
        HighPunchDamageSetUp();
    }
    private void Update()
    {
        _isPlayerPunchingHigh = PlayerOneMovement._isPlayerPunchingHigh;
        _hitCollider.enabled = _isPlayerPunchingHigh || PlayerOneMovement._isPlayerPunchingLow;
    }
    void OnTriggerStay(Collider _opponentHeadHit)
    {
        //if (_isPlayerPunchingHigh) return;

        if (_opponentHeadHit.CompareTag("HeadHitBox")
            && _isPlayerPunchingHigh
            && Time.time >= _nextPunchIsAllowed)
        {
            HeadPunch();
            _nextPunchIsAllowed = Time.time + _attackDelay;
        }

        _opponentHeadHit.ClosestPointOnBounds(transform.position);
        _opponentImpactPoint = _opponentHeadHit.transform.position;
    }

    void HeadPunch()
    {

        Debug.Log("Hit head with high punch");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByHighPunch;

        OpponentHealth _tempDamage = FightCamera._opponent.GetComponent<OpponentHealth>();

        _tempDamage.OpponentHighPunchDamage(_highPunchDamageValue);
    }

    private void HighPunchDamageSetUp()
    {
        _highPunchDamageValue = GetComponentInParent<CharacterStats>()._highPunchDamage;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerPunchLow : MonoBehaviour
{
    public static Vector3 _opponentImpactPoint;
    private Collider _hitCollider;
    private bool _isPlayerPunchingLow;
    public float _nextPunchIsAllowed = -1f;
    public float _attackDelay = 1f;

    private int _lowPunchDamageValue;

    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;

        LowPunchDamageSetUp();
    }
    private void Update()
    {
        _isPlayerPunchingLow = PlayerOneMovement._isPlayerPunchingLow;
        _hitCollider.enabled = _isPlayerPunchingLow;
    }

    void OnTriggerStay(Collider _opponentBodyHit)
    {
        if (_opponentBodyHit.CompareTag("BodyHitBox") && Time.time >= _nextPunchIsAllowed)
        {
            BodyPunch();
            _nextPunchIsAllowed = Time.time + _attackDelay;
        };

        _opponentBodyHit.ClosestPointOnBounds(transform.position);
        _opponentImpactPoint = _opponentBodyHit.transform.position;
    }

    void BodyPunch()
    {
        Debug.Log("Hit body with low punch");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByLowPunch;
    }

    private void LowPunchDamageSetUp()
    {
        _lowPunchDamageValue = GetComponentInParent<CharacterStats>()._lowPunchDamage;
    }
}
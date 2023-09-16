using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickLow : MonoBehaviour
{
    public static Vector3 _opponentImpactPoint;
    private Collider _hitCollider;
    private bool _isPlayerKickingLow;
    public float _nextKickIsAllowed = -1f;
    public float _attackDelay = 1f;
    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
    }
    private void Update()
    {
        _isPlayerKickingLow = PlayerOneMovement._isPlayerKickingLow;
        _hitCollider.enabled = _isPlayerKickingLow;
    }

    void OnTriggerStay(Collider _opponentBodyHit)
    {
        if (_opponentBodyHit.CompareTag("BodyHitBox") && Time.time >= _nextKickIsAllowed)
        {
            BodyKick();
            _nextKickIsAllowed = Time.time + _attackDelay;
        }

        _opponentBodyHit.ClosestPointOnBounds(transform.position);
        _opponentImpactPoint = _opponentBodyHit.transform.position;
    }

    void BodyKick()
    {
        Debug.Log("Hit body by low kick");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByLowKick;
    }
}



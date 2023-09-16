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

    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
    }
    private void Update()
    {
        _isPlayerKickingHigh = PlayerOneMovement._isPlayerKickingHigh;
        _hitCollider.enabled = _isPlayerKickingHigh;
    }

    void OnTriggerStay(Collider _opponentHeadHit)
    {
        if (_opponentHeadHit.CompareTag("BodyHitBox") && Time.time >= _nextKickIsAllowed)
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
    }
}
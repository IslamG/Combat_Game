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

    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
    }
    private void Update()
    {
        _isPlayerPunchingHigh = PlayerOneMovement._isPlayerPunchingHigh;
        _hitCollider.enabled = _isPlayerPunchingHigh;
    }
    void OnTriggerStay(Collider _opponentHeadHit)
    {
        if (_opponentHeadHit.CompareTag("HeadHitBox") && Time.time >= _nextPunchIsAllowed)
        {
            HeadPunch();
            _nextPunchIsAllowed = Time.time + _attackDelay;
        }

        _opponentHeadHit.ClosestPointOnBounds(transform.position);
        _opponentImpactPoint = _opponentHeadHit.transform.position;
    }

    void HeadPunch()
    {

        Debug.Log("Hit head");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByHighPunch;
    }
}

public class PlayerPunchLow : MonoBehaviour
{
    public static Vector3 _opponentImpactPoint;
    private Collider _hitCollider;
    private bool _isPlayerPunchingLow;
    public float _nextPunchIsAllowed = -1f;
    public float _attackDelay = 1f;

    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
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
        Debug.Log("Hit body");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByLowPunch;
    }
}


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
        Debug.Log("Hit body");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByHighKick;
    }
}


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
        Debug.Log("Hit body");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitByLowKick;
    }
}



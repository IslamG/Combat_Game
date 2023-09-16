using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPunchLow : MonoBehaviour
{
    public static Vector3 _playerImpactPoint;
    private Collider _hitCollider;
    private bool _isOpponentPunchingLow;
    public float _nextPunchIsAllowed = -1f;
    public float _attackDelay = 1f;

    private void Start()
    {
        _playerImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
    }
    private void Update()
    {
        _isOpponentPunchingLow = OpponentAI._isOpponentPunchingLow;
        _hitCollider.enabled = _isOpponentPunchingLow;
    }

    void OnTriggerStay(Collider _playerBodyHit)
    {
        if (_playerBodyHit.CompareTag("BodyHitBox") && Time.time >= _nextPunchIsAllowed)
        {
            //BodyPunch();
            _nextPunchIsAllowed = Time.time + _attackDelay;
        };

        _playerBodyHit.ClosestPointOnBounds(transform.position);
        _playerImpactPoint = _playerBodyHit.transform.position;
    }

    //void BodyPunch()
    //{
    //    Debug.Log("Hit body");
    //    PlayerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByLowPunch;
    //}
}
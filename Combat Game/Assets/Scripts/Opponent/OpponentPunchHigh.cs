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

    private void Start()
    {
        _playerImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
    }
    private void Update()
    {
        _isOpponentPunchingHigh = OpponentAI._isOpponentPunchingHigh;
        _hitCollider.enabled = _isOpponentPunchingHigh;
    }
    void OnTriggerStay(Collider _playerHeadHit)
    {
        if (_playerHeadHit.CompareTag("HeadHitBox") && Time.time >= _nextPunchIsAllowed)
        {
            //HeadPunch();
            _nextPunchIsAllowed = Time.time + _attackDelay;
        }

        _playerHeadHit.ClosestPointOnBounds(transform.position);
        _playerImpactPoint = _playerHeadHit.transform.position;
    }

    //void HeadPunch()
    //{

    //    Debug.Log("Hit head");
    //    PlayerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByHighPunch;
    //}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
     
public class OpponentKickHigh : MonoBehaviour
{
    public static Vector3 _playerImpactPoint;
    private Collider _hitCollider;
    private bool _isOpponentKickingHigh;
    public float _nextKickIsAllowed = -1f;
    public float _attackDelay = 1f;

    private void Start()
    {
        _playerImpactPoint = Vector3.zero;
        _hitCollider = GetComponent<Collider>();
        _hitCollider.enabled = false;
    }
    private void Update()
    {
        _isOpponentKickingHigh = OpponentAI._isOpponentKickingHigh;
        _hitCollider.enabled = _isOpponentKickingHigh;
    }

    void OnTriggerStay(Collider _playerHeadHit)
    {
        if (_playerHeadHit.CompareTag("BodyHitBox") && Time.time >= _nextKickIsAllowed)
        {
            //HeadKick();
            _nextKickIsAllowed = Time.time + _attackDelay;
        }

        _playerHeadHit.ClosestPointOnBounds(transform.position);
        _playerImpactPoint = _playerHeadHit.transform.position;
    }

    //void HeadKick()
    //{
    //    Debug.Log("Hit body");
    //    PlayerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByHighKick;
    //}
}
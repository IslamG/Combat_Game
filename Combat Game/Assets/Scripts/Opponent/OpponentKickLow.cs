using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   
public class OpponentKickLow : MonoBehaviour
{
    public static Vector3 _playerImpactPoint;
    private Collider _hitCollider;
    private bool _isOpponentKickingLow;
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
        _isOpponentKickingLow = OpponentAI._isOpponentKickingLow;
        _hitCollider.enabled = _isOpponentKickingLow;
    }

    void OnTriggerStay(Collider _playerBodyHit)
    {
        if (_playerBodyHit.CompareTag("HeadHitBox") && Time.time >= _nextKickIsAllowed)
        {
            //BodyKick();
            _nextKickIsAllowed = Time.time + _attackDelay;
        }

        _playerBodyHit.ClosestPointOnBounds(transform.position);
        _playerImpactPoint = _playerBodyHit.transform.position;
    }

    //void BodyKick()
    //{
    //    Debug.Log("Hit body");
    //    PlayerOneMovement._playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHitByLowKick;
    //}
}



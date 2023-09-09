using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentHeadHit : MonoBehaviour
{
    public static Vector3 _opponentImpactPoint;

    private void Start()
    {
        _opponentImpactPoint = Vector3.zero;
    }
    void OnTriggerEnter(Collider _opponentHeadHit)
    {
        if (_opponentHeadHit.CompareTag("HeadHitBox"))
            HeadStruck();

        _opponentHeadHit.ClosestPointOnBounds(transform.position);
        _opponentImpactPoint = _opponentHeadHit.transform.position;
    }

    void HeadStruck()
    {

        Debug.Log("Hit head");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitHead;
    }
}


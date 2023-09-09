using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentBodyHit : MonoBehaviour
{
    public static Vector3 _opponentImpactPoint;

    void OnTriggerEnter(Collider _opponentBodyHit)
    {
        if (_opponentBodyHit.CompareTag("BodyHitBox"))
            BodyStruck();

        _opponentBodyHit.ClosestPointOnBounds(transform.position);
        _opponentImpactPoint = _opponentBodyHit.transform.position;
    }

    void BodyStruck()
    {
        Debug.Log("Hit body");
        OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitBody;
    }
}

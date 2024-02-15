using Assets.Scripts.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class S1Movement : SpecialMoveStats
{
    private Rigidbody _rigidBody;
    private Vector3 _projectileMovementVector;

    public float _projectileSpeed = 4f;
    public float _projectileLifetime = 5f;

    public int _projectileDamage;

    private GameObject _playerPosition;
    private GameObject _opponentPosition;

    private bool _specialMoveHit;
    private GameObject _hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        _playerPosition = FightCamera._playerOne;
        _opponentPosition = FightCamera._opponent;

        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.useGravity = false;
        _projectileMovementVector = Vector3.zero;

        _specialMoveHit = false;
        _projectileDamage = _specialMove1Damage;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerPosition.transform.position.x < _opponentPosition.transform.position.x)
            _projectileMovementVector = Vector3.right;

        if (_playerPosition.transform.position.x > _opponentPosition.transform.position.x)
            _projectileMovementVector = Vector3.left;

        _projectileLifetime -= Time.deltaTime;
        if (_projectileLifetime < 0)
            _projectileLifetime = 0;

        if (_projectileLifetime == 0)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        _rigidBody.velocity = _projectileMovementVector * _projectileSpeed;
    }

    private void OnTriggerEnter(Collider _collision)
    {
        if (_specialMoveHit)
            return;

        var tag = _collision.gameObject.tag;
        switch (tag)
        {
            case "PlayerHeadHit":
                _specialMoveHit = true;
                ApplyProjectileDamangeToPlayer(true);
                break;
            case "PlayerBodyHit":
                _specialMoveHit = true;
                ApplyProjectileDamangeToPlayer(false);
                break;
            case "OpponentHeadHit":
                _specialMoveHit = true;
                ApplyProjectileDamangeToOpponent(true);
                OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitBySP1Head;
                break;
            case "OpponentBodyHit":
                _specialMoveHit = true;
                ApplyProjectileDamangeToOpponent(false);
                OpponentAI._opponentAIState = OpponentAI.OpponentAIState.OpponentHitBySP1Body;
                break;
        }
    }
    private void ApplyProjectileDamangeToOpponent(bool headHit)
    {
        OpponentHealth _tempDamage = FightCamera._opponent.GetComponent<OpponentHealth>();
        _tempDamage.OpponentSP1Damage(_projectileDamage, headHit);

        HitExplosion();
        Destroy(gameObject);
    }
    private void ApplyProjectileDamangeToPlayer(bool headHit)
    {
        //OpponentHealth _tempDamage = FightCamera._opponent.GetComponent<OpponentHealth>();
        //_tempDamage.OpponentSP1Damage(_projectileDamage, headHit);

        HitExplosion();
        Destroy(gameObject);
    }

    private void HitExplosion()
    {
        _hitEffect = Instantiate(Resources.Load("Sp1HitEffect", typeof(GameObject))) as GameObject;

        _hitEffect.transform.position = new Vector3(
            transform.position.x, 
            transform.position.y, 
            transform.position.z);
    }
}

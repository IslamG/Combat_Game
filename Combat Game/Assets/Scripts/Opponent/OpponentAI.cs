using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class OpponentAI : MonoBehaviour
{
    private Transform _opponentTransform;
    private CharacterController _opponentController;

    private Animation _opponentAnim;
    public AnimationClip _opponentIdleAnim;
    public AnimationClip _opponentHitBodyAnim;
    public AnimationClip _opponentHitHeadAnim;
    public AnimationClip _opponentDefeatedAnim;

    private AudioSource _opponentAIAudioSource;
    public AudioClip _opponentHeadHitAudio;
    public AudioClip _opponentBodyHitAudio;

    public GameObject _hitEffect;

    private Vector3 _opponentMoveDirection = Vector3.zero;

    private bool _fightIntroFinished;

    private float _opponentGravity = 5f;
    private float _opponentGravityModifier = 5f;
    private float _opponentVerticalSpeed = 0.0f;

    private CollisionFlags _collisionFlagsOpponent;

    public static OpponentAIState _opponentAIState;

    public enum OpponentAIState
    {
        Initialize,
        OpponentIdle, 
        OpponentHitBody, 
        OpponentHitHead, 
        WaitForAnimations,
        OpponentDefeated
    }

    private void Start()
    {
        _opponentController = GetComponent<CharacterController>();
        _opponentAnim = GetComponent<Animation>();
        _opponentAIAudioSource = GetComponent<AudioSource>();
        _opponentTransform = transform;
        _opponentMoveDirection = Vector3.zero;

        StartCoroutine("OpponentFSM");
    }

    private void Update()
    {
        ApplyOpponentGravity();
    }
    
    private IEnumerator OpponentFSM()
    {
        while (true)
        {
            switch (_opponentAIState)
            {
                case OpponentAIState.Initialize:
                    Initialize();
                    break;
                case OpponentAIState.OpponentIdle:
                    OpponentIdle();
                    break;
                case OpponentAIState.OpponentHitBody:
                    OpponentHitBody();
                    break;
                case OpponentAIState.OpponentHitHead:
                    OpponentHitHead();
                    break;
                case OpponentAIState.WaitForAnimations:
                    WaitForAnimations();
                    break;
                case OpponentAIState.OpponentDefeated:
                    OpponentDefeated();
                    break;
            }
            yield return null;
        }
    }

    private void Initialize()
    {
        _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void OpponentIdle()
    {
        OpponentIdleAnim();
        
        if (OpponentIsGrounded()) return;

        _opponentMoveDirection = new Vector3(0, _opponentVerticalSpeed, 0);

        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        _fightIntroFinished = FightIntro._fightIntroFinished;

        if (!_fightIntroFinished)
            return;

    }
    private void OpponentHitBody()
    {
        OpponentHitBodyAnim();

        _opponentAIAudioSource.PlayOneShot(_opponentBodyHitAudio);

        Vector3 _impactPoint = OpponentBodyHit._opponentImpactPoint;

        GameObject _he = Instantiate(_hitEffect,
            _impactPoint,
            //new Vector3(_opponentTransform.position.x,_opponentTransform.position.y * -1.1f, _opponentTransform.position.z), 
            Quaternion.identity) as GameObject;

        _opponentAIState = OpponentAIState.WaitForAnimations;
    }
    private void OpponentHitHead()
    {
        OpponentHitHeadAnim();

        _opponentAIAudioSource.PlayOneShot(_opponentHeadHitAudio);

        Vector3 _impactPoint = OpponentHeadHit._opponentImpactPoint;

        GameObject _he = Instantiate(_hitEffect, 
            _impactPoint,
            //new Vector3(_opponentTransform.position.x, _opponentTransform.position.y * -1.5f, _opponentTransform.position.z),
            Quaternion.identity) as GameObject;
        
        _opponentAIState = OpponentAIState.WaitForAnimations;
    }
    private void WaitForAnimations()
    {
        if (_opponentAnim.IsPlaying(_opponentHitBodyAnim.name))
            return;
        if (_opponentAnim.IsPlaying(_opponentHitHeadAnim.name))
            return;

        _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void OpponentDefeated()
    {
        _opponentMoveDirection = new Vector3(0, _opponentVerticalSpeed, 0);

        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        if (_opponentAnim.IsPlaying(_opponentDefeatedAnim.name))
            return;

        StopCoroutine("OpponentFSM");
    }
    private void SetOpponentDefeated()
    {
        OpponentDefeatedAnim();
        _opponentAIState = OpponentAIState.OpponentDefeated;
    }
    private void OpponentIdleAnim()
    {
        _opponentAnim.CrossFade(_opponentIdleAnim.name);
    }
    private void OpponentHitBodyAnim()
    {
        _opponentAnim.CrossFade(_opponentHitBodyAnim.name);
    }
    private void OpponentHitHeadAnim()
    {
        _opponentAnim.CrossFade(_opponentHitHeadAnim.name);
    }
    private void OpponentDefeatedAnim()
    {
        _opponentAnim.CrossFade(_opponentDefeatedAnim.name);
    }

    void ApplyOpponentGravity()
    {
        if (OpponentIsGrounded())
            _opponentVerticalSpeed = 0.0f;
        else
        {
            _opponentVerticalSpeed -= 
                _opponentGravity * _opponentGravityModifier * Time.deltaTime;
        }
    }

    public bool OpponentIsGrounded() => 
        (_collisionFlagsOpponent & CollisionFlags.CollidedBelow) != 0;
}

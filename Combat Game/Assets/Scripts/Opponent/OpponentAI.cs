using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class OpponentAI : MonoBehaviour
{
    private Transform _opponentTransform;
    private CharacterController _opponentController;

    public static GameObject _playerOne;
    public static GameObject _opponent;

    private Vector3 _playerPosition;
    private Vector3 _opponentPosition;

    private Quaternion _targetRotation;
    private int _defaultRotation = 0;
    private int _alternativeRotation = 180;
    public float _rotationSpeed = 5f;
    public float _opponentWalkSpeed = 1.0f;

    private Animation _opponentAnim;
    public AnimationClip _opponentIdleAnim;
    public AnimationClip _opponentHitBodyAnim;
    public AnimationClip _opponentHitHeadAnim;
    public AnimationClip _opponentDefeatedAnim;
    public AnimationClip _opponentWalkAnim;
    public AnimationClip _opponentJumpAnim;

    public AnimationClip _opponentPunchHighAnim;
    public AnimationClip _opponentPunchLowAnim;
    public AnimationClip _opponentKickHighAnim;
    public AnimationClip _opponentKickLowAnim;

    private AudioSource _opponentAIAudioSource;
    public AudioClip _opponentHeadHitAudio;
    public AudioClip _opponentBodyHitAudio;

    public GameObject _hitEffect;

    private Vector3 _opponentMoveDirection = Vector3.zero;

    public float _opponentJumpHeight = 0.5f;
    public float _opponentJumpSpeed = 1;
    public float _opponentJumpHorizontal = 1f;
    private Vector3 _jumpHeightTemp;

    private float _opponentGravity = 5f;
    private float _opponentGravityModifier = 5f;
    private float _opponentVerticalSpeed = 0.0f;

    public RangeInt _offensivePriority = new RangeInt(1, 3);
    public RangeInt _passivePriority = new RangeInt(4, 6);
    public RangeInt _defensivePriority = new RangeInt(7, 9);

    private int _decideMoveForwards; 
    private int _decideMoveBackwards;

    private int _minimumDecideValue;
    private int _maximumDecideValue;

    public int _tippingPointDecideValue = 1;

    private int _decideAggressionPriority;
    private bool _isPassive;
    public float _passiveTime = 2;

    private bool _fightIntroFinished;

    private CollisionFlags _collisionFlagsOpponent;

    public static OpponentAIState _opponentAIState;

    public enum OpponentAIState
    {
        Initialize,
        OpponentIdle, 
        Advance,
        StayPassive,
        Retreat,
        WalkTowardsPlayer,
        WalkAwayFromPlayer,
        JumpUp, 
        JumpTowardsPlayer, 
        JumpAwayFromPlayer,
        ComeDown, 
        ComeDownForward, 
        ComeDownBackwards,
        OpponentHitByLowPunch, 
        OpponentHitByHighPunch,
        OpponentHitByLowKick,
        OpponentHitByHighKick,
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

        _jumpHeightTemp = new Vector3(0, _opponentJumpHeight, 0);

        _decideAggressionPriority = 0;
        _isPassive = false;

        _minimumDecideValue = 1;
        _maximumDecideValue = 10;

        StartCoroutine("OpponentFSM");
    }

    private void Update()
    {
        ApplyOpponentGravity();

        UpdatePlayerPosition();
        UpdateOpponentPosition();
        UpdateOpponentRotation();
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
                case OpponentAIState.Advance:
                    Advance();
                    break;
                case OpponentAIState.Retreat:
                    Retreat();
                    break;
                case OpponentAIState.WalkTowardsPlayer:
                    WalkTowardsPlayer();
                    break;
                case OpponentAIState.WalkAwayFromPlayer:
                    WalkAwayFromPlayer();
                    break;
                case OpponentAIState.JumpUp:
                    JumpUp();
                    break;
                case OpponentAIState.JumpTowardsPlayer:
                    JumpTowardsPlayer();
                    break;
                case OpponentAIState.JumpAwayFromPlayer:
                    JumpAwayFromPlayer();
                    break;
                case OpponentAIState.ComeDown:
                    ComeDown();
                    break;
                case OpponentAIState.ComeDownForward:
                    ComeDownForward();
                    break;
                case OpponentAIState.ComeDownBackwards:
                    ComeDownBackwards();
                    break;
                case OpponentAIState.OpponentHitByLowPunch:
                    OpponentHitByLowPunch();
                    break;
                case OpponentAIState.OpponentHitByHighPunch:
                    OpponentHitByHighPunch();
                    break;
                case OpponentAIState.OpponentHitByLowKick:
                    OpponentHitByLowKick();
                    break;
                case OpponentAIState.OpponentHitByHighKick:
                    OpponentHitByHighKick();
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
        _decideAggressionPriority = Random.Range(1, 9);
        _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void OpponentIdle()
    {
        OpponentIdleAnim();
        Idle();

        _fightIntroFinished = FightIntro._fightIntroFinished;
        if (!_fightIntroFinished)
            return;
        //Debug.Log("Idle deciding");
        if (_decideAggressionPriority < _passivePriority.start)
            _opponentAIState = OpponentAIState.Advance;
        if (_decideAggressionPriority > _passivePriority.end &&
            _decideAggressionPriority < _defensivePriority.start)
        {
            if (_isPassive) return;

            _isPassive = true;
            
            StartCoroutine("StayPassive");
        }
        if (_decideAggressionPriority == _defensivePriority.start)
            _opponentAIState = OpponentAIState.Retreat;

        

    }
    private void Idle()
    {
        if (OpponentIsGrounded()) return;
        //Debug.Log("Idling");
        _opponentMoveDirection = new Vector3(0, _opponentVerticalSpeed, 0);

        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

    }
    private void OpponentHitByLowPunch()
    {
        OpponentHitBodyAnim();

        _opponentAIAudioSource.PlayOneShot(_opponentBodyHitAudio);

        Vector3 _impactPoint = PlayerPunchLow._opponentImpactPoint;

        GameObject _he = Instantiate(_hitEffect,
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f, _impactPoint.y + 0.78f, _impactPoint.z - 0.6f), 
            Quaternion.identity) as GameObject;

        _opponentAIState = OpponentAIState.WaitForAnimations;
    }
    private void OpponentHitByHighPunch()
    {
        OpponentHitHeadAnim();

        _opponentAIAudioSource.PlayOneShot(_opponentHeadHitAudio);

        Vector3 _impactPoint = PlayerPunchHigh._opponentImpactPoint;

        GameObject _he = Instantiate(_hitEffect, 
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f,  _impactPoint.y * -1.5f, _impactPoint.z - 0.6f),
            Quaternion.identity) as GameObject;
        
        _opponentAIState = OpponentAIState.WaitForAnimations;
    }
    private void OpponentHitByLowKick()
    {
        OpponentHitBodyAnim();

        _opponentAIAudioSource.PlayOneShot(_opponentBodyHitAudio);

        Vector3 _impactPoint = PlayerKickLow._opponentImpactPoint;

        GameObject _he = Instantiate(_hitEffect,
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f, _impactPoint.y + 0.78f, _impactPoint.z - 0.6f),
            Quaternion.identity) as GameObject;

        _opponentAIState = OpponentAIState.WaitForAnimations;
    }
    private void OpponentHitByHighKick()
    {
        OpponentHitHeadAnim();

        _opponentAIAudioSource.PlayOneShot(_opponentHeadHitAudio);

        Vector3 _impactPoint = PlayerKickHigh._opponentImpactPoint;

        GameObject _he = Instantiate(_hitEffect,
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f, _impactPoint.y * -1.5f, _impactPoint.z - 0.6f),
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
    private void WalkTowardsPlayer()
    {
       // Debug.Log("called walk");
        _opponentMoveDirection = (_playerPosition - transform.position) * _opponentWalkSpeed;
        _opponentMoveDirection.Normalize();

        _opponentMoveDirection.y = 0;
        _opponentMoveDirection.z = 0;

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        OpponentWalkAnim();
    }
    private void WalkAwayFromPlayer()
    {
        //Debug.Log("called walk");
        _opponentMoveDirection = (transform.position - _playerPosition) * _opponentWalkSpeed;
        _opponentMoveDirection.Normalize();

        _opponentMoveDirection.y = 0;
        _opponentMoveDirection.z = 0;

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        OpponentWalkAnim();
    }
    private void JumpUp() 
    {
        OpponentJumpAnim();
        _opponentMoveDirection = new Vector3(0, _opponentJumpSpeed, 0);
        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection).normalized;
        _opponentMoveDirection *= _opponentJumpSpeed;

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        if (_opponentTransform.transform.position.y >= _jumpHeightTemp.y)
        {
            _opponentAIState = OpponentAIState.ComeDown;
        }
    }
    private void JumpTowardsPlayer()
    {
        OpponentJumpAnim();

        _opponentMoveDirection = new Vector3(-_opponentJumpHorizontal, _opponentVerticalSpeed, 0);
        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);
        _opponentMoveDirection *= _opponentVerticalSpeed;

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        if (_opponentTransform.transform.position.y >= _jumpHeightTemp.y)
        {
            _opponentAIState = OpponentAIState.ComeDownForward;
        }
    }
    private void JumpAwayFromPlayer()
    {
        OpponentJumpAnim();

        _opponentMoveDirection = new Vector3(_opponentJumpHorizontal, _opponentVerticalSpeed, 0);
        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);
        _opponentMoveDirection *= _opponentVerticalSpeed;

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        if (_opponentTransform.transform.position.y >= _jumpHeightTemp.y)
        {
            _opponentAIState = OpponentAIState.ComeDownBackwards;
        }
    }
    private void ComeDown()
    {
        _opponentMoveDirection = new Vector3(0, _opponentVerticalSpeed, 0);
        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        if (OpponentIsGrounded())
            _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void ComeDownForward()
    {
        _opponentMoveDirection = new Vector3(-_opponentJumpHorizontal, _opponentVerticalSpeed, 0);
        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        if (OpponentIsGrounded())
            _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void ComeDownBackwards()
    {
        _opponentMoveDirection = new Vector3(_opponentJumpHorizontal, _opponentVerticalSpeed, 0);
        _opponentMoveDirection = _opponentTransform.TransformDirection(_opponentMoveDirection);

        _collisionFlagsOpponent = _opponentController.Move(_opponentMoveDirection * Time.deltaTime);

        if (OpponentIsGrounded())
            _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void Advance()
    {
        //Debug.Log("Advance deciding");
        //_opponentAIState = OpponentAIState.Attack;
        _decideMoveForwards = Random.Range(1, 10);

        if(_decideMoveForwards >= _minimumDecideValue && 
            _decideMoveForwards <= _tippingPointDecideValue)
        {
            WalkAwayFromPlayer();
        }
        if (_decideMoveForwards <= _maximumDecideValue &&
            _decideMoveForwards <= _tippingPointDecideValue)
        {
            WalkAwayFromPlayer();

        }
    }
    private IEnumerator StayPassive()
    {
        //_opponentAIState = OpponentAIState.StayPassive;
        Debug.Log("Staying passive");

        yield return new WaitForSeconds(_passiveTime);
        _isPassive = false;
    }
    private void Retreat()
    {
        //Debug.Log("Retreat deciding");
        //_opponentAIState = OpponentAIState.Retreat;
        _decideMoveBackwards = Random.Range(1, 10);

        if (_decideMoveBackwards >= _minimumDecideValue &&
            _decideMoveBackwards <= _tippingPointDecideValue)
        {
            WalkAwayFromPlayer();
        }

        if (_decideMoveBackwards <= _maximumDecideValue &&
            _decideMoveBackwards > _tippingPointDecideValue)
        {
            WalkAwayFromPlayer();
        }
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
    private void OpponentWalkAnim()
    {
        //Debug.Log("reached animation");
        _opponentAnim.CrossFade(_opponentWalkAnim.name);
    }
    private void OpponentJumpAnim()
    {
        //Debug.Log("reached animation");
        _opponentAnim.CrossFade(_opponentJumpAnim.name);
    }
    private void OpponentPunchHighAnim()
    {
        _opponentAnim.CrossFade(_opponentPunchHighAnim.name);
    }
    private void OpponentPunchLowAnim()
    {
        _opponentAnim.CrossFade(_opponentPunchLowAnim.name);
    }
    private void OpponentKickHighAnim()
    {
        _opponentAnim.CrossFade(_opponentKickHighAnim.name);
    }
    private void OpponentKickLowAnim()
    {
        _opponentAnim.CrossFade(_opponentKickLowAnim.name);
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

    private void UpdatePlayerPosition()
    {
        _playerPosition = _playerOne.transform.position;
    }
    private void UpdateOpponentPosition()
    {
        _opponentPosition = _opponent.transform.position;
    }
    private void UpdateOpponentRotation()
    {
        if (_playerPosition.x > _opponentPosition.x)
        {
            if (_opponent.transform.rotation.y == _defaultRotation)
                return;
            else
            {
                _targetRotation = Quaternion.Euler(0, _defaultRotation, 0);
                _opponent.transform.rotation = Quaternion.Slerp(transform.rotation,
                    _targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }

        if (_playerPosition.x < _opponentPosition.x)
        {
            if (_opponent.transform.rotation.y == _alternativeRotation)
                return;
            else
            {
                _targetRotation = Quaternion.Euler(0, _alternativeRotation, 0);
                _opponent.transform.rotation = Quaternion.Slerp(transform.rotation,
                    _targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }
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

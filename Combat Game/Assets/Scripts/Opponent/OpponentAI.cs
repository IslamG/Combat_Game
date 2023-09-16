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

    public static bool _isOpponentPunchingLow;
    public static bool _isOpponentPunchingHigh;
    public static bool _isOpponentKickingLow;
    public static bool _isOpponentKickingHigh;

    private Vector3 _playerPosition;
    private Vector3 _opponentPosition;
    private Vector3 _positionDifference;
    private float _positionDifferenceModifier = 5.0f;

    private Quaternion _targetRotation;
    private int _defaultRotation = 0;
    private int _alternativeRotation = 180;
    public float _rotationSpeed = 5f;
    private float _opponentWalkSpeed = 2.75f;

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

    private float _opponentJumpHeight = 1f;
    private float _opponentJumpSpeed = 1;
    private float _opponentJumpHorizontal = 3f;
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

    private int _tippingPointDecideValue = 1;

    private int _decideAggressionPriority;
    private bool _isPassive;
    private float _passiveTime = 2;

    private ChooseAttack _chooseAttack;
    private int _switchAttackValue;
    private int _switchAttackStateValue;
    private int _attackTypePivotValue;
    private float _attackDistanceModifier = 1.75f;

    public int _lowPunchRangeMin, _lowPunchRangeMax;
    public int _highPunchRangeMin, _highPunchRangeMax;
    public int _lowKickRangeMin, _lowKickRangeMax;
    public int _highKickRangeMin, _highKickRangeMax;

    private bool _fightIntroFinished;

    private CollisionFlags _collisionFlagsOpponent;

    public static OpponentAIState _opponentAIState;

    public enum OpponentAIState
    {
        Initialize, OpponentIdle, 
        Advance, StayPassive, Retreat,
        WalkTowardsPlayer,  WalkAwayFromPlayer,
        JumpUp, JumpTowardsPlayer, JumpAwayFromPlayer,
        ComeDown, ComeDownForward, ComeDownBackwards,
        OpponentHitByLowPunch, OpponentHitByHighPunch, OpponentHitByLowKick, OpponentHitByHighKick,
        OpponentLowPunch, OpponentHighPunch, OpponentLowKick, OpponentHighKick,
        ChooseAttackState, 
        WaitForAnimations, WaitForStrikeAnimations, 
        OpponentDefeated
    }
    private enum ChooseAttack
    {
        LowPunch, HighPunch, LowKick, HighKick
    }
    private IEnumerator OpponentFSM()
    {
        while (true)
        {
            switch (_opponentAIState)
            {
                case OpponentAIState.Initialize: Initialize(); break;
                case OpponentAIState.OpponentIdle: OpponentIdle(); break;
                case OpponentAIState.Advance: Advance(); break;
                case OpponentAIState.Retreat: Retreat(); break;
                case OpponentAIState.WalkTowardsPlayer: WalkTowardsPlayer(); break;
                case OpponentAIState.WalkAwayFromPlayer: WalkAwayFromPlayer(); break;
                case OpponentAIState.JumpUp: JumpUp(); break;
                case OpponentAIState.JumpTowardsPlayer: JumpTowardsPlayer(); break;
                case OpponentAIState.JumpAwayFromPlayer: JumpAwayFromPlayer(); break;
                case OpponentAIState.ComeDown: ComeDown(); break;
                case OpponentAIState.ComeDownForward: ComeDownForward(); break;
                case OpponentAIState.ComeDownBackwards: ComeDownBackwards(); break;
                case OpponentAIState.OpponentHitByLowPunch: OpponentHitByLowPunch(); break;
                case OpponentAIState.OpponentHitByHighPunch: OpponentHitByHighPunch(); break;
                case OpponentAIState.OpponentHitByLowKick: OpponentHitByLowKick(); break;
                case OpponentAIState.OpponentHitByHighKick: OpponentHitByHighKick(); break;
                case OpponentAIState.OpponentLowPunch: OpponentLowPunch(); break;
                case OpponentAIState.OpponentHighPunch: OpponentHighPunch(); break;
                case OpponentAIState.OpponentLowKick: OpponentLowKick(); break;
                case OpponentAIState.OpponentHighKick: OpponentHighKick(); break;
                case OpponentAIState.ChooseAttackState: ChooseAttackState(); break;
                case OpponentAIState.WaitForAnimations: WaitForAnimations(); break;
                case OpponentAIState.WaitForStrikeAnimations: WaitForStrikeAnimations(); break;
                case OpponentAIState.OpponentDefeated: OpponentDefeated(); break;
            }
            yield return null;
        }
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
        UpdatePositionDifference();
        UpdateOpponentPlanePosition();
    }

    private void Initialize()
    {
        _decideAggressionPriority = Random.Range(1, 9);
        
        if(_attackTypePivotValue == 0)
            _attackTypePivotValue = 3;
        
        //if (_lowPunchRangeMin != 0)
        //    _lowPunchRangeMin = 0;
        //if (_lowPunchRangeMax == 0)
        //    _lowPunchRangeMax = 1;

        //if (_highPunchRangeMin == 0)
        //    _highPunchRangeMin = 2;
        //if (_highPunchRangeMax == 0)
        //    _highPunchRangeMax = 3;

        //if (_lowKickRangeMin == 0)
        //    _lowKickRangeMin = 4;
        //if (_lowKickRangeMax == 0)
        //    _lowKickRangeMax = 5;

        //if (_highKickRangeMin == 0)
        //    _highKickRangeMin = 6;
        //if (_highKickRangeMax == 0)
        //    _highKickRangeMax = 7;

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
    private void ChooseAttackState()
    {
        OpponentIdleAnim();

        _switchAttackValue = Random.Range(0, 7);

        if (_switchAttackValue >= _lowPunchRangeMin && _switchAttackValue <= _lowPunchRangeMax)
            _switchAttackStateValue = 0;

        if (_switchAttackValue >= _highPunchRangeMin && _switchAttackValue <= _highPunchRangeMax)
            _switchAttackStateValue = 1;

        if (_switchAttackValue >= _lowKickRangeMin && _switchAttackValue <= _lowKickRangeMax)
            _switchAttackStateValue = 2;

        if (_switchAttackValue >= _highKickRangeMin && _switchAttackValue <= _highKickRangeMax)
            _switchAttackStateValue = 3;

        switch (_switchAttackStateValue)
        {
            case 0: _chooseAttack = ChooseAttack.LowPunch; break;
            case 1: _chooseAttack = ChooseAttack.HighPunch; break;
            case 2: _chooseAttack = ChooseAttack.LowKick; break;
            case 3: _chooseAttack = ChooseAttack.HighKick; break;
        }

        if (_chooseAttack == ChooseAttack.LowPunch)
            _opponentAIState = OpponentAIState.OpponentLowPunch;
        if (_chooseAttack == ChooseAttack.HighPunch)
            _opponentAIState = OpponentAIState.OpponentHighPunch;
        if (_chooseAttack == ChooseAttack.LowKick)
            _opponentAIState = OpponentAIState.OpponentLowKick;
        if (_chooseAttack == ChooseAttack.HighKick)
            _opponentAIState = OpponentAIState.OpponentHighKick;
    }
    private void OpponentLowPunch()
    {
        OpponentPunchLowAnim();
        _opponentAIState = OpponentAIState.WaitForStrikeAnimations;
    }
    private void OpponentHighPunch()
    {
        OpponentPunchHighAnim();
        _opponentAIState = OpponentAIState.WaitForStrikeAnimations;

    }
    private void OpponentLowKick()
    {
        OpponentKickHighAnim();
        _opponentAIState = OpponentAIState.WaitForStrikeAnimations;

    }
    private void OpponentHighKick()
    {
        OpponentKickLowAnim();
        _opponentAIState = OpponentAIState.WaitForStrikeAnimations;

    }
    private void WaitForAnimations()
    {
        if (_opponentAnim.IsPlaying(_opponentHitBodyAnim.name))
            return;
        if (_opponentAnim.IsPlaying(_opponentHitHeadAnim.name))
            return;

        _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void WaitForStrikeAnimations()
    {
        if (_opponentAnim.IsPlaying(_opponentPunchLowAnim.name))
            return;
        if (_opponentAnim.IsPlaying(_opponentPunchHighAnim.name))
            return;
        if (_opponentAnim.IsPlaying(_opponentKickLowAnim.name))
            return;
        if (_opponentAnim.IsPlaying(_opponentKickHighAnim.name))
            return;

        _isOpponentPunchingLow = false;
        _isOpponentPunchingHigh = false;
        _isOpponentKickingLow = false;
        _isOpponentKickingHigh = false;

        _opponentAIState = OpponentAIState.OpponentIdle;
    }
    private void WalkTowardsPlayer()
    {
        if (Mathf.Abs(_playerOne.transform.position.x - _opponent.transform.position.x) 
            <= _attackDistanceModifier)
            _opponentAIState = OpponentAIState.ChooseAttackState;

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
            _opponentAIState = OpponentAIState.WalkTowardsPlayer;
        }
        if (_decideMoveForwards <= _maximumDecideValue &&
            _decideMoveForwards >= _tippingPointDecideValue)
        {
            _opponentAIState = _positionDifference.x >= _positionDifferenceModifier ?
                OpponentAIState.JumpTowardsPlayer : OpponentAIState.WalkTowardsPlayer;
                 
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
    public void SetOpponentDefeated()
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
    private void UpdatePositionDifference()
    {
        if (_playerOne.transform.position.x < _opponent.transform.position.x)
            _positionDifference = _opponentPosition - _playerPosition;

        if (_playerOne.transform.position.x > _opponent.transform.position.x)
            _positionDifference =  _playerPosition - _opponentPosition;
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
    private void UpdateOpponentPlanePosition()
    {
        if (_opponentController.transform.position.z != GameManager._opponentStartingPosition.z)
            transform.position = new Vector3(transform.position.x,
                transform.position.y, GameManager._opponentStartingPosition.z);
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

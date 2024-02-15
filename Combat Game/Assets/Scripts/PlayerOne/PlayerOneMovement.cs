using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OpponentAI;

public class PlayerOneMovement : MonoBehaviour
{
    private Transform _playerOneTransform;
    [SerializeField]
    private CharacterController _playerController;

    private float _playerControllersHeight;
    private float _playerControllersRadius;

    public static GameObject _playerOne;
    public static GameObject _opponent;

    private Vector3 _playerPosition;
    private Vector3 _opponentPosition;

    private Quaternion _targetRotation;
    private int _defaultRotation = 180;
    private int _alternativeRotation = 0;
    public float _rotationSpeed = 5f;

    public static bool _isPlayerPunchingLow;
    public static bool _isPlayerPunchingHigh;
    public static bool _isPlayerKickingLow;
    public static bool _isPlayerKickingHigh;

    public float _playerWalkSpeed = 1f;
    public float _playerRetreatSpeed = 0.75f;
    public float _playerJumpHeight = 0.5f;
    public float _playerJumpSpeed = 1f;
    public float _playerJumpHorizontal = 1f;
    private Vector3 _jumpHeightTemp;

    public static bool _specialMoveOneActive;
    public static bool _specialMoveTwoActive;
    public static bool _specialMoveThreeActive;

    public float _controllerDeadZonePos = 0.1f;
    public float _controllerDeadZoneNeg = -0.1f;

    private float _xAxis;
    private float _yAxis;
    private float _directionAngle;

    private int _zeroDegreeAngle = 0;
    private int _90DegreeAngle = 90;
    private int _180DegreeAngle = 180;
    public float _degreeModifier = 22.5f;

    public float _playerGravity = 20f;
    public float _playerGravityModifier = 5;
    public float _playerSpeedYAxis;

    private Animation _playerOneAnim;
    public AnimationClip _playerOneIdleClip;
    public AnimationClip _playerWalkClip;
    public AnimationClip _playerJumpClip;
    public AnimationClip _playerHitHeadClip;
    public AnimationClip _playerHitBodyClip;
    public AnimationClip[] _playerAttackAnim;
    public AnimationClip _playerDefeatedClip;

    public AnimationClip _playerSpecialMoveOneClip;
    public AnimationClip _playerSpecialMoveTwoClip;
    public AnimationClip _playerSpecialMoveThreeClip;

    private GameObject _specialMove1Projectile; 

    private AudioSource _playerAudioSource;
    public AudioClip _playerHeadHitAudio;
    public AudioClip _playerBodyHitAudio;

    public GameObject _hitEffect;

    private bool _returnDemoState;
    private int _demoRotationValue = 500;

    private bool _fightIntroFinished;

    private Vector3 _playerOneMoveDirection = Vector3.zero;
    private CollisionFlags _collisionFlags;

    public PlayerOneStates _playerOneStates;

    public enum PlayerOneStates
    {
        PlayerOneIdle, PlayerWalkLeft, PlayerWalkRight,
        PlayerJump, PlayerJumpForwards, PlayerJumpBackwards,
        PlayerComeDown, PlayerComeDownForwards, PlayerComeDownBackwards,
        PlayerHighPunch, PlayerLowPunch, PlayerHighKick, PlayerLowKick,
        SpecialMoveOne, SpecialMoveTwo, SpecialMoveThree,
        PlayerHitByHighPunch, PlayerHitByLowPunch, PlayerHitByHighKick, PlayerHitByLowKick,
        WaitForAnimations, WaitForSpecialMoveAnimations, WaitForStrikeAnimations,
        PlayerDemo, PlayerDefeated
    }
    private IEnumerator PlayerOneFSM()
    {
        while (true)
        {
            switch (_playerOneStates)
            {
                case PlayerOneStates.PlayerOneIdle: PlayerOneIdle(); break;
                case PlayerOneStates.PlayerWalkLeft: PlayerWalkLeft(); break;
                case PlayerOneStates.PlayerWalkRight: PlayerWalkRight(); break;
                case PlayerOneStates.PlayerJump: PlayerJump(); break;
                case PlayerOneStates.PlayerJumpForwards: PlayerJumpForwards(); break;
                case PlayerOneStates.PlayerJumpBackwards: PlayerJumpBackwards(); break;
                case PlayerOneStates.PlayerComeDown: PlayerComeDown(); break;
                case PlayerOneStates.PlayerComeDownForwards: PlayeComeDownForwards(); break;
                case PlayerOneStates.PlayerComeDownBackwards: PlayerComeDownBackwards(); break;
                case PlayerOneStates.SpecialMoveOne: SpecialMoveOne(); break;
                case PlayerOneStates.SpecialMoveTwo: SpecialMoveTwo(); break;
                case PlayerOneStates.SpecialMoveThree: SpecialMoveThree(); break;
                case PlayerOneStates.PlayerHighPunch: PlayerHighPunch(); break;
                case PlayerOneStates.PlayerLowPunch: PlayerLowPunch(); break;
                case PlayerOneStates.PlayerHighKick: PlayerHighKick(); break;
                case PlayerOneStates.PlayerLowKick: PlayerLowKick(); break;
                case PlayerOneStates.PlayerHitByLowPunch: PlayerHitByLowPunch(); break;
                case PlayerOneStates.PlayerHitByHighPunch: PlayerHitByHighPunch(); break;
                case PlayerOneStates.PlayerHitByLowKick: PlayerHitByLowKick(); break;
                case PlayerOneStates.PlayerHitByHighKick: PlayerHitByHighKick(); break;
                case PlayerOneStates.WaitForAnimations: WaitForAnimations(); break;
                case PlayerOneStates.WaitForSpecialMoveAnimations: WaitForSpecialMoveAnimations(); break;
                case PlayerOneStates.WaitForStrikeAnimations: WaitForStrikeAnimations(); break;
                case PlayerOneStates.PlayerDemo: PlayerDemo(); break;
                case PlayerOneStates.PlayerDefeated: PlayerDefeated(); break;
            }
            yield return null;
        }
    }

    void Start()
    {
        _playerOneTransform = transform;
        _playerOneMoveDirection = Vector3.zero;

        _jumpHeightTemp = new Vector3(0, _playerJumpHeight, 0);
        _playerSpeedYAxis = 0;

        _playerController = GetComponent<CharacterController>();
        _playerControllersHeight = _playerController.height;
        _playerControllersRadius = _playerController.radius;

        _playerOneAnim = GetComponent<Animation>();
        _playerAudioSource = GetComponent<AudioSource>();

        for (int a = 0; a < _playerAttackAnim.Length; a++)
        {
            _playerOneAnim[_playerAttackAnim[a].name].wrapMode = WrapMode.Once;
        }

        _isPlayerPunchingLow = false;
        _isPlayerPunchingHigh = false;
        _isPlayerKickingLow = false;
        _isPlayerKickingHigh = false;

        _specialMoveOneActive = false;
        _specialMoveTwoActive = false;
        _specialMoveThreeActive = false;

        StartCoroutine("PlayerOneFSM");

        _returnDemoState = false;
        _returnDemoState = ChooseCharacter._demoPlayer;
        if (_returnDemoState)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerDemo;
        }
    }
    void Update()
    {
        ApplyGravity();
        InputController();

        for (int a = 0; a < _playerAttackAnim.Length; a++)
        {
            if (_playerOneAnim.IsPlaying(_playerAttackAnim[a].name))
                return;
        }

        _fightIntroFinished = FightIntro._fightIntroFinished;

        if (!_fightIntroFinished)
            return;

        if (PlayerIsGrounded())
        {
            HorizontalJumpInputManager();
            AttackInputManager();
            StandardInputManager();
        }

        UpdatePlayerPosition();
        UpdateOpponentPosition();
        UpdatePlayerRotation();
        UpdatePlayerPlanePosition();
    }

    #region Player state methods
    private void PlayerOneIdle()
    {
        PlayerOneIdleAnim();

        if (PlayerIsGrounded())
            return;

        _playerOneMoveDirection = new Vector3(0, _playerSpeedYAxis, 0);

        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);
    }
    private void PlayerWalkLeft()
    {
        PlayerRetreatAnim();

        _playerOneMoveDirection = new Vector3(+_playerWalkSpeed, 0, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection).normalized;
        _playerOneMoveDirection *= _playerWalkSpeed;

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (Input.GetAxis("Horizontal") == 0)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerOneIdle;
        }
    }
    private void PlayerWalkRight()
    {
        PlayerWalkAnim();

        _playerOneMoveDirection = new Vector3(-_playerWalkSpeed, 0, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection).normalized;
        _playerOneMoveDirection *= _playerWalkSpeed;

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (_directionAngle < (_180DegreeAngle - _degreeModifier)
                && _directionAngle > 0 - (_180DegreeAngle + _degreeModifier))
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerOneIdle;
        }
    }
    private void PlayerJump()
    {
        PlayerJumpAnim();

        _playerOneMoveDirection = new Vector3(0, _playerJumpSpeed, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection).normalized;
        _playerOneMoveDirection *= _playerJumpSpeed;

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (_playerOneTransform.transform.position.y >= _jumpHeightTemp.y)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerComeDown;
        }
    }
    private void PlayerJumpForwards()
    {
        PlayerJumpAnim();

        _playerOneMoveDirection = new Vector3(-_playerJumpHorizontal, _playerJumpSpeed, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);
        _playerOneMoveDirection *= _playerJumpSpeed;

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (_playerOneTransform.transform.position.y >= _jumpHeightTemp.y)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerComeDownForwards;
        }
    }
    private void PlayerJumpBackwards()
    {
        PlayerJumpAnim();

        _playerOneMoveDirection = new Vector3(_playerJumpHorizontal, _playerJumpSpeed, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);
        _playerOneMoveDirection *= _playerJumpSpeed;

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (_playerOneTransform.transform.position.y >= _jumpHeightTemp.y)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerComeDownBackwards;
        }
    }
    private void PlayerComeDown()
    {
        _playerOneMoveDirection = new Vector3(0, _playerSpeedYAxis, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (PlayerIsGrounded())
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerOneIdle;
    }
    private void PlayeComeDownForwards()
    {
        _playerOneMoveDirection = new Vector3(-_playerJumpHorizontal, _playerSpeedYAxis, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (PlayerIsGrounded())
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerOneIdle;
    }
    private void PlayerComeDownBackwards()
    {
        _playerOneMoveDirection = new Vector3(_playerJumpHorizontal, _playerSpeedYAxis, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (PlayerIsGrounded())
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerOneIdle;
    }
    public void SpecialMoveOne()
    {
        SpecialMoveOneAnim();
        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForSpecialMoveAnimations;
    }
    public void SpecialMoveTwo()
    {
        SpecialMoveTwoAnim();
        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForSpecialMoveAnimations;
    }
    public void SpecialMoveThree()
    {
        SpecialMoveThreeAnim();
        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForSpecialMoveAnimations;
    }
    private void PlayerHighPunch()
    {
        PlayerHighPunchAnim();

        _isPlayerPunchingHigh = true;
        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForStrikeAnimations;
    }
    private void PlayerLowPunch()
    {
        PlayerLowPunchAnim();

        _isPlayerPunchingLow = true;
        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForStrikeAnimations;
    }
    private void PlayerHighKick()
    {
        PlayerHighKickAnim();

        _isPlayerKickingHigh = true;
        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForStrikeAnimations;
    }
    private void PlayerLowKick()
    {
        PlayerLowKickAnim();

        _isPlayerKickingLow = true;
        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForStrikeAnimations;
    }
    private void PlayerHitByLowPunch()
    {
        PlayerHitBodyAnim();

        _playerAudioSource.PlayOneShot(_playerBodyHitAudio);

        Vector3 _impactPoint = new Vector3(
             transform.position.x - _playerController.radius / 1.25f,
             transform.position.y + _playerController.height / 6,
             transform.position.z - 0.6f
             );

        GameObject _he = Instantiate(_hitEffect,
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f, _impactPoint.y + 0.78f, _impactPoint.z - 0.6f),
            Quaternion.identity) as GameObject;

        _playerOneStates = PlayerOneStates.WaitForAnimations;
    }
    private void PlayerHitByHighPunch()
    {
        PlayerHitHeadAnim();

        _playerAudioSource.PlayOneShot(_playerHeadHitAudio);

        Vector3 _impactPoint = new Vector3(
             transform.position.x - _playerController.radius / 1.25f,
             transform.position.y + _playerController.height / 6,
             transform.position.z - 0.6f
             );

        GameObject _he = Instantiate(_hitEffect,
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f, _impactPoint.y * -1.5f, _impactPoint.z - 0.6f),
            Quaternion.identity) as GameObject;

        _playerOneStates = PlayerOneStates.WaitForAnimations;
    }
    private void PlayerHitByLowKick()
    {
        PlayerHitBodyAnim();

        _playerAudioSource.PlayOneShot(_playerBodyHitAudio);

        Vector3 _impactPoint = new Vector3(
             transform.position.x - _playerController.radius / 1.25f,
             transform.position.y + _playerController.height / 6,
             transform.position.z - 0.6f
             );

        GameObject _he = Instantiate(_hitEffect,
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f, _impactPoint.y + 0.78f, _impactPoint.z - 0.6f),
            Quaternion.identity) as GameObject;

        _playerOneStates = PlayerOneStates.WaitForAnimations;
    }
    private void PlayerHitByHighKick()
    {
        PlayerHitHeadAnim();

        _playerAudioSource.PlayOneShot(_playerHeadHitAudio);

        Vector3 _impactPoint = new Vector3(
             transform.position.x - _playerController.radius / 1.25f,
             transform.position.y + _playerController.height / 6,
             transform.position.z - 0.6f
             );

        GameObject _he = Instantiate(_hitEffect,
            //_impactPoint,
            new Vector3(_impactPoint.x - 0.6f, _impactPoint.y + 0.78f, _impactPoint.z - 0.6f),
            Quaternion.identity) as GameObject;

        _playerOneStates = PlayerOneStates.WaitForAnimations;
    }
    private void WaitForStrikeAnimations()
    {
        for (int a = 0; a < _playerAttackAnim.Length; a++)
        {
            if (_playerOneAnim.IsPlaying(_playerAttackAnim[a].name))
                return;
        }

        _isPlayerPunchingLow = false;
        _isPlayerPunchingHigh = false;
        _isPlayerKickingLow = false;
        _isPlayerKickingHigh = false;

        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForStrikeAnimations;
    }
    private void WaitForAnimations()
    {
        if (_playerOneAnim.IsPlaying(_playerHitBodyClip.name))
            return;
        if (_playerOneAnim.IsPlaying(_playerHitHeadClip.name))
            return;

        _playerOneStates = PlayerOneStates.PlayerOneIdle;
    }
    private void WaitForSpecialMoveAnimations()
    {
        if (_playerOneAnim[_playerSpecialMoveOneClip.name].time >= 0.5f)
        {
            if (_specialMove1Projectile != null)
                return;
            else
            {
                _specialMove1Projectile = Instantiate(Resources.Load("SP1", typeof(GameObject))) as GameObject;
                int direction = 1;

                //if (_playerPosition.x < _opponentPosition.x)
                //    direction = 1;

                if (_playerPosition.x > _opponentPosition.x)
                    direction = -1;

                _specialMove1Projectile.transform.position = new Vector3(
                    transform.position.x + ((_playerControllersRadius * 3) * direction),
                    transform.position.y + (_playerControllersHeight / 1.5f),
                    transform.position.z);
            }
        }

        if (_playerOneAnim.IsPlaying(_playerSpecialMoveOneClip.name))
            return;
        _specialMoveOneActive = false;

        _playerOneStates = PlayerOneStates.PlayerOneIdle;
    }
    private void PlayerDemo()
    {
        PlayerDemoAnim();

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)//(Input.GetButtonDown("LeftTrigger")) //Horizontal//LeftTrigger
        {
            transform.Rotate(Vector3.up * _demoRotationValue * Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)//(Input.GetButtonDown("RightTrigger")) //Horizontal//LeftTrigger
        {
            transform.Rotate(Vector3.down * _demoRotationValue * Time.deltaTime);
        }
    }
    private void PlayerDefeated()
    {
        _playerOneMoveDirection = new Vector3(0, _yAxis, 0);

        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (_playerOneAnim.IsPlaying(_playerDefeatedClip.name))
            return;

        StopCoroutine("PlayerOneFSM");
    }
    #endregion Player state methods

    #region Animation methods
    private void PlayerOneIdleAnim()
    {
        // Debug.Log(_playerOneIdleClip + " " + _playerOneIdleClip.name);
        _playerOneAnim.CrossFade(_playerOneIdleClip.name);
    }
    private void PlayerWalkAnim()
    {
        _playerOneAnim.CrossFade(_playerWalkClip.name);

        if (_playerOneAnim[_playerWalkClip.name].speed == _playerWalkSpeed)
            return;

        if (_playerOneAnim[_playerWalkClip.name].speed < _playerWalkSpeed)
            _playerOneAnim[_playerWalkClip.name].speed = _playerWalkSpeed;

    }
    private void PlayerRetreatAnim()
    {
        _playerOneAnim.CrossFade(_playerWalkClip.name);

        if (_playerOneAnim[_playerWalkClip.name].speed == _playerWalkSpeed)
            return;

        if (_playerOneAnim[_playerWalkClip.name].speed > _playerRetreatSpeed)
            _playerOneAnim[_playerWalkClip.name].speed = _playerRetreatSpeed;
    }
    private void PlayerJumpAnim()
    {
        _playerOneAnim.CrossFade(_playerJumpClip.name);
    }
    private void SpecialMoveOneAnim()
    {
        _playerOneAnim.CrossFade(_playerSpecialMoveOneClip.name);
    }
    private void SpecialMoveTwoAnim()
    {
        _playerOneAnim.CrossFade(_playerSpecialMoveTwoClip.name);
    }
    private void SpecialMoveThreeAnim()
    {
        _playerOneAnim.CrossFade(_playerSpecialMoveThreeClip.name);
    }
    private void PlayerHighPunchAnim()
    {
        _playerOneAnim.CrossFade(_playerAttackAnim[0].name);
    }
    private void PlayerLowPunchAnim()
    {
        _playerOneAnim.CrossFade(_playerAttackAnim[1].name);
    }
    private void PlayerHighKickAnim()
    {
        _playerOneAnim.CrossFade(_playerAttackAnim[2].name);
    }
    private void PlayerLowKickAnim()
    {
        _playerOneAnim.CrossFade(_playerAttackAnim[3].name);
    }
    private void PlayerHitBodyAnim()
    {
        _playerOneAnim.CrossFade(_playerHitBodyClip.name);
    }
    private void PlayerHitHeadAnim()
    {
        _playerOneAnim.CrossFade(_playerHitHeadClip.name);
    }
    private void PlayerDemoAnim()
    {
        _playerOneAnim.CrossFade(_playerOneIdleClip.name);
    }
    private void PlayerDefeatedAnim()
    {
        _playerOneAnim.CrossFade(_playerDefeatedClip.name);
    }
    #endregion Animation methods

    #region Input Management
    private void InputController()
    {
        _xAxis = Input.GetAxis("Horizontal");
        _yAxis = Input.GetAxis("Vertical");

        _directionAngle = Mathf.Atan2(_yAxis, _xAxis) * Mathf.Rad2Deg;
    }
    private void AttackInputManager()
    {
        if (_specialMoveOneActive || _specialMoveTwoActive || _specialMoveThreeActive)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHighPunch;
        if (Input.GetButtonDown("Fire2"))
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerLowPunch;
        if (Input.GetButtonDown("Fire3"))
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerHighKick;
        if (Input.GetButtonDown("Fire4"))
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerLowKick;
    }
    private void HorizontalJumpInputManager()
    {
        if (_specialMoveOneActive || _specialMoveTwoActive || _specialMoveThreeActive)
        {
            return;
        }

        if (Input.GetAxis("Vertical") > _controllerDeadZonePos &&
            Input.GetAxis("Horizontal") > 0)
        {
            if (_directionAngle > 45 + _degreeModifier
                || _directionAngle < 45 - _degreeModifier)
                return;
            
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerJumpBackwards;
        }

        if (Input.GetAxis("Vertical") > _controllerDeadZonePos &&
            Input.GetAxis("Horizontal") < 0)
        {
            if (_directionAngle > 135 + _degreeModifier
                || _directionAngle < 135 - _degreeModifier)
                return;

            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerJumpForwards;
        }

    }
    private void StandardInputManager()
    {
        if (_specialMoveOneActive || _specialMoveTwoActive || _specialMoveThreeActive)
        {
            return;
        }

        if (Input.GetAxis("Horizontal") <  _controllerDeadZoneNeg)
        {
            if (_directionAngle < (_180DegreeAngle - _degreeModifier)
                && _directionAngle > 0 - (_180DegreeAngle + _degreeModifier))
                return;

            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerWalkRight;
        }

        if (Input.GetAxis("Horizontal") > _controllerDeadZonePos)
        {
            if (_directionAngle > (0 + _degreeModifier)
                || _directionAngle < 0 - _degreeModifier)
                return;

            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerWalkLeft;
        }

        if (Input.GetAxis("Vertical") > _controllerDeadZonePos)
        {
            if (_directionAngle > 90 + _degreeModifier
                || _directionAngle < 90 - _degreeModifier)
                return;

            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerJump;
        }
    }
    #endregion InputManagement

    public void SetPlayerDefeated()
    {
        PlayerDefeatedAnim();
        _playerOneStates = PlayerOneStates.PlayerDefeated;
    }


    private void UpdatePlayerPosition()
    {
        _playerPosition = _playerOne.transform.position;
    }
    private void UpdateOpponentPosition()
    {
        _opponentPosition = _opponent.transform.position;
    }
    private void UpdatePlayerRotation()
    {
        if (_playerPosition.x > _opponentPosition.x)
        {
            if (_playerOne.transform.rotation.y == _defaultRotation)
                return;
            else
            {
                _targetRotation = Quaternion.Euler(0, _defaultRotation, 0);
                _playerOne.transform.rotation = Quaternion.Slerp(transform.rotation,
                    _targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }

        if (_playerPosition.x < _opponentPosition.x)
        {
            if (_playerOne.transform.rotation.y == _alternativeRotation)
                return;
            else
            {
                _targetRotation = Quaternion.Euler(0, _alternativeRotation, 0);
                _playerOne.transform.rotation = Quaternion.Slerp(transform.rotation,
                    _targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }
    }
    private void UpdatePlayerPlanePosition()
    {
        if (_playerController.transform.position.z != GameManager._playerStartingPosition.z)
            transform.position = new Vector3(transform.position.x,
                transform.position.y, GameManager._opponentStartingPosition.z);
    }

    private void ApplyGravity()
    {
        if (PlayerIsGrounded())
        {
            _playerSpeedYAxis = 0f;
        }
        else
        {
            _playerSpeedYAxis -= _playerGravity * _playerGravityModifier * Time.deltaTime;
        }
    }
    public bool PlayerIsGrounded()
    {
        return (_collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

}

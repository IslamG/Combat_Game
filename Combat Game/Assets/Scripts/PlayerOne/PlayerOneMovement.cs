using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneMovement : MonoBehaviour
{
    private Transform _playerOneTransform;
    private CharacterController _playerController;

    public float _playerWalkSpeed = 1f;
    public float _playerRetreatSpeed = 0.75f;
    public float _playerJumpHeight = 5f;
    public float _playerJumpSpeed = 5f;
    public float _playerJumpHorizontal = 5f;

    public float _controllerDeadZonePos = 0.1f;
    public float _controllerDeadZoneNeg = -0.1f;

    public float _playerGravity = 20f;
    public float _playerGravityModifier = 5;
    public float _playerSpeedYAxis;

    private Animation _playerOneAnim;
    public AnimationClip _playerOneIdleClip;
    public AnimationClip _playerWalkClip;
    public AnimationClip _playerJumpClip;
    public AnimationClip[] _playerAttackAnim;

    private bool _returnDemoState;
    private int _demoRotationValue = 120;

    private bool _fightIntroFinished; 

    private Vector3 _playerOneMoveDirection = Vector3.zero;
    private CollisionFlags _collisionFlags;

    private PlayerOneStates _playerOneStates;

    private enum PlayerOneStates
    {
        PlayerOneIdle, PlayerWalkLeft, PlayerWalkRight, 
        PlayerJump, PlayerJumpForwards, PlayerJumpBackwards,
        PlayerComeDown, PlayerComeDownForwards, PlayerComeDownBackwards,
        PlayerHighPunch, PlayerLowPunch, PlayerHighKick, PlayerLowKick,
        WaitForAnimations, 
        PlayerDemo
    }
    // Start is called before the first frame update
    void Start()
    {
        _playerOneTransform = transform;
        _playerOneMoveDirection = Vector3.zero;

        _playerSpeedYAxis = 0;

        _playerController = GetComponent<CharacterController>();

         _playerOneAnim = GetComponent<Animation>();

        for(int a = 0; a < _playerAttackAnim.Length; a++)
        {
            _playerOneAnim[_playerAttackAnim[a].name].wrapMode = WrapMode.Once;
        }

        StartCoroutine("PlayerOneFSM");

        _returnDemoState = false;
        _returnDemoState = ChooseCharacter._demoPlayer;
        if(_returnDemoState )
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerDemo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();

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
        
    }

    private IEnumerator PlayerOneFSM()
    {
        while (true)
        {
            switch (_playerOneStates)
            {
                case PlayerOneStates.PlayerOneIdle: PlayerOneIdle();break;
                case PlayerOneStates.PlayerWalkLeft: PlayerWalkLeft();break;
                case PlayerOneStates.PlayerWalkRight: PlayerWalkRight();break;
                case PlayerOneStates.PlayerJump: PlayerJump();break;
                case PlayerOneStates.PlayerJumpForwards: PlayerJumpForwards();break;
                case PlayerOneStates.PlayerJumpBackwards: PlayerJumpBackwards();break;
                case PlayerOneStates.PlayerComeDown: PlayerComeDown();break;
                case PlayerOneStates.PlayerComeDownForwards: PlayeComeDownForwards();break;
                case PlayerOneStates.PlayerComeDownBackwards: PlayerComeDownBackwards();break;
                case PlayerOneStates.PlayerHighPunch: PlayerHighPunch();break;
                case PlayerOneStates.PlayerLowPunch: PlayerLowPunch();break;
                case PlayerOneStates.PlayerHighKick: PlayerHighKick();break;
                case PlayerOneStates.PlayerLowKick: PlayerLowKick();break;
                case PlayerOneStates.WaitForAnimations: WaitForAnimations();break;
                case PlayerOneStates.PlayerDemo: PlayerDemo();break;
            }
            yield return null;
        }
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

        _playerOneMoveDirection = new Vector3(+ _playerWalkSpeed, 0, 0);
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

        _playerOneMoveDirection = new Vector3(- _playerWalkSpeed, 0, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection).normalized;
        _playerOneMoveDirection *= _playerWalkSpeed;

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if (Input.GetAxis("Horizontal") > 0f)
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

        if (_playerOneTransform.transform.position.y >= _playerJumpHeight)
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

        if (_playerOneTransform.transform.position.y >= _playerJumpHeight)
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

        if (_playerOneTransform.transform.position.y >= _playerJumpHeight)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerComeDownBackwards;
        }
    }

    private void PlayerComeDown()
    {
        _playerOneMoveDirection = new Vector3(0, _playerSpeedYAxis, 0);
        _playerOneMoveDirection = _playerOneTransform.TransformDirection(_playerOneMoveDirection);

        _collisionFlags = _playerController.Move(_playerOneMoveDirection * Time.deltaTime);

        if(PlayerIsGrounded())
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

    private void PlayerHighPunch()
    {
        PlayerHighPunchAnim();

        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForAnimations;
    }

    private void PlayerLowPunch()
    {
        PlayerLowPunchAnim();

        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForAnimations;
    }

    private void PlayerHighKick()
    {
        PlayerHighKickAnim();

        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForAnimations;
    }

    private void PlayerLowKick()
    {
        PlayerLowKickAnim();

        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForAnimations;
    }

    private void WaitForAnimations()
    {
        for(int a = 0; a < _playerAttackAnim.Length; a++)
        {
            if (_playerOneAnim.IsPlaying(_playerAttackAnim[a].name))
                return;
        }

        _playerOneStates = PlayerOneMovement.PlayerOneStates.WaitForAnimations; 
    }

    private void PlayerDemo()
    {
        PlayerDemoAnim();

        if (Input.GetButtonDown("LeftTrigger")) //Horizontal//LeftTrigger
        {
            transform.Rotate(Vector3.up *_demoRotationValue * Time.deltaTime);
        }

        if (Input.GetButtonDown("RightTrigger")) //Horizontal//LeftTrigger
        {
            transform.Rotate(Vector3.down * _demoRotationValue * Time.deltaTime);
        }
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

    private void PlayerDemoAnim()
    {
        _playerOneAnim.CrossFade(_playerOneIdleClip.name);
    }

    #endregion Animation methods


    private void AttackInputManager()
    {
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
        if(Input.GetAxis("Vertical") > _controllerDeadZonePos && 
            Input.GetAxis("Horizontal") > _controllerDeadZoneNeg)
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerJumpForwards;

        if (Input.GetAxis("Vertical") > _controllerDeadZonePos &&
            Input.GetAxis("Horizontal") < _controllerDeadZoneNeg)
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerJumpBackwards;

    }

    private void StandardInputManager()
    {
        //Debug.Log("Input H " + Input.GetAxis("Horizontal"));
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) return;

        //Debug.Log("Input V " + Input.GetAxis("Vertical"));
        if (Input.GetAxis("Vertical") < _controllerDeadZonePos)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerJump;
        }

        if (Input.GetAxis("Horizontal") >  _controllerDeadZoneNeg)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerWalkLeft;
        }

        if (Input.GetAxis("Horizontal") < _controllerDeadZonePos)
        {
            _playerOneStates = PlayerOneMovement.PlayerOneStates.PlayerWalkRight;
        }
        
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

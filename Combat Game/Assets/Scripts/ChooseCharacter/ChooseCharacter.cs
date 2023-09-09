using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class ChooseCharacter : ChooseCharacterManager
{
    public Vector3 _spawnPosition;// = new Vector3();

    public Texture2D _selectCharacterTextBackground;
    public Texture2D _selectCharacterTextForeground;
    public Texture2D _selectCharacterText;

    public Texture2D _selectCharacterArrowLeft; //Use seperate graphics for different input looks
    public Texture2D _selectCharacterArrowRight;//PS vs XBox vs PC buttons
    //public Texture2D _selectCharacterArrowLeft;
    //public Texture2D _selectCharacterArrowRight;

    private float _foregroundTextWidth;
    private float _foregroundTextHeight;
    private float _arrowSize;

    private float _chooseCharacterInputTimer;
    private float _chooseCharacterInputDelay = 0.1f;

    public AudioClip _cycleCharacterButtonPress;

    private GameObject _characterDemo;
    public static bool _demoPlayer;

    private GameObject _switchCharacterParticleSystem;
    private int _pickRandomCharacter;

    public int _characterSelectSate;
    private int _yRot = 90;

    private enum CharacterSelectModels
    {
        Char1 = 0, Char2, Char3, Char4
    }

    // Start is called before the first frame update
    void Start()
    {
        CharacterSelectManager();

        _foregroundTextWidth = Screen.width / 1.5f;
        _foregroundTextHeight = Screen.height / 7;
        _arrowSize = _foregroundTextHeight;
        _demoPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            _demoPlayer = false;
            GameObject.FindGameObjectWithTag("BackgroundManager").GetComponent<BackgroundManager>()
                .SendMessage("SceneBackgroundLoad");
        }

        if (_chooseCharacterInputTimer > 0) 
            _chooseCharacterInputTimer -= 1f * Time.deltaTime;

        if (_chooseCharacterInputTimer > 0)
            return;

        if (Input.GetAxis("Horizontal") < -0.5f)
        {
            if (_characterSelectSate == 0)
                return;

            GetComponent<AudioSource>().PlayOneShot(_cycleCharacterButtonPress);

            _characterSelectSate--;
            _switchCharacterParticleSystem = Instantiate(Resources.Load("SwitchCharParticles")) as GameObject;
            _switchCharacterParticleSystem.transform.position = _spawnPosition;
            CharacterSelectManager();

            _chooseCharacterInputTimer = _chooseCharacterInputDelay;
        }
        if (Input.GetAxis("Horizontal") > 0.5f)
        {
            if (_characterSelectSate == 3)
                return;

            GetComponent<AudioSource>().PlayOneShot(_cycleCharacterButtonPress);
            _characterSelectSate++;
            _switchCharacterParticleSystem = Instantiate(Resources.Load("SwitchCharParticles")) as GameObject;
            _switchCharacterParticleSystem.transform.position = _spawnPosition;
            CharacterSelectManager();

            _chooseCharacterInputTimer = _chooseCharacterInputDelay;

        }

        if (Input.GetButtonDown("Select"))
        {
            _pickRandomCharacter = Random.Range(0, 3);
            _characterSelectSate = _pickRandomCharacter;

            GetComponent<AudioSource>().PlayOneShot(_cycleCharacterButtonPress);

            _switchCharacterParticleSystem = Instantiate(Resources.Load("SwitchCharParticles")) as GameObject;
            _switchCharacterParticleSystem.transform.position = _spawnPosition;

            CharacterSelectManager();
        }
    }

    private void CharacterSelectManager()
    {
        switch (_characterSelectSate)
        {
            default:
            case 0: Char1(); break;
            case 1: Char2(); break;
            case 2: Char3(); break;
            case 3: Char4(); break;
        }
    }

    private void Char1() 
    {
        DestroyObject(_characterDemo);
        _characterDemo = Instantiate(Resources.Load("Char1"), _spawnPosition, Quaternion.identity) as GameObject;

        //_characterDemo.transform.position = _spawnPosition;
        _characterDemo.transform.eulerAngles = new Vector3(0, _yRot, 0);

        _char1 = true;
        _char2 = false;
        _char3 = false;
        _char4 = false;
    }
    private void Char2()
    {
        DestroyObject(_characterDemo);
        _characterDemo = Instantiate(Resources.Load("Char2"), _spawnPosition, Quaternion.identity) as GameObject;

        //_characterDemo.transform.position = _spawnPosition;
        _characterDemo.transform.eulerAngles = new Vector3(0, _yRot, 0);

        _char2 = true;
        _char1 = false;
        _char3 = false;
        _char4 = false;
    }
    private void Char3()
    {
        DestroyObject(_characterDemo);
        _characterDemo = Instantiate(Resources.Load("Char3"), _spawnPosition, Quaternion.identity) as GameObject;

        //_characterDemo.transform.position = _spawnPosition;
        _characterDemo.transform.eulerAngles = new Vector3(0, _yRot, 0);

        _char3 = true;
        _char2 = false;
        _char1 = false;
        _char4 = false;
    }
    private void Char4()
    {
        DestroyObject(_characterDemo);
        _characterDemo = Instantiate(Resources.Load("Char4"), _spawnPosition, Quaternion.identity) as GameObject;

        //_characterDemo.transform.position = _spawnPosition;
        _characterDemo.transform.eulerAngles = new Vector3(0, _yRot, 0);

        _char4 = true;
        _char2 = false;
        _char3 = false;
        _char1 = false;
    }

    void OnGUI()
    {
        //Background
        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height / 7),
            _selectCharacterTextBackground);
        //Foreground
        GUI.DrawTexture(new Rect(
            Screen.width /2 - (_foregroundTextWidth/2),
            0,
            _foregroundTextWidth, _foregroundTextHeight),
            _selectCharacterTextForeground);
        //Text
        GUI.DrawTexture(new Rect(
            Screen.width / 2 - (_foregroundTextWidth / 2),
            0,
            _foregroundTextWidth, _foregroundTextHeight),
            _selectCharacterText);

        //Replace _selectArrow with controller specific textures
        if(GameObject.FindGameObjectWithTag("ControllerManager")?
            .GetComponent<ControllerManager>()?._xBOXController ?? false)
        {
            _selectCharacterArrowLeft = _selectCharacterArrowLeft;
            _selectCharacterArrowRight = _selectCharacterArrowRight;
        }
        //Arrow right 
        GUI.DrawTexture(new Rect(
            Screen.width / 2 + (_foregroundTextWidth / 2),
            0,
            _arrowSize, _arrowSize),
            _selectCharacterArrowRight);
        //Arrow left
        GUI.DrawTexture(new Rect(
            Screen.width / 2 - (_foregroundTextWidth / 2) - _arrowSize,
            0,
            _arrowSize, _arrowSize),
            _selectCharacterArrowLeft);
    }
}

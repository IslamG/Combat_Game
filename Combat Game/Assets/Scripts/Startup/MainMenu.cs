using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{
    private int _selectedButton = 0;
    private float _timeBetweenButtonPress = 0.5f;
    private float _timeDelay;

    private float _mainMenuVerticalInputTimer;
    private float _mainMenuVerticalInputDelay = 0.1f;

    public Texture2D _mainMenuBackground;
    public Texture2D _mainMenuTitle;

    private AudioSource _mainMenuAudio;
    public AudioClip _mainMenuMusic;
    public AudioClip _mainMenuStartButtonAudio;
    public AudioClip _mainMenuQuitButtonAudio;

    private float _mainMenuFadeValue;
    private float _mainMenuFadeSpeed = 0.5f;

    private float _mainMenuButtonWidth = 100f;
    private float _mainMenuButtonHeight = 25f;
    private float _mainMenuGUIOffset = 10f;

    private bool _startingOnePlayerGame;
    private bool _startingTwoPlayerGame;
    private bool _quittingGame;

    private bool _ps4Controller;
    private bool _xboxController;

    private string[] _mainMenuButtons = new string[]
    {
        "_onePlayer",
        "_twoPlayer",
        "_quit"
    };

    private MainMenuController _mainMenuController;

    private enum MainMenuController
    {
        MainMenuFadeIn = 0,
        MainMenuAtIdle = 1,
        MainMenuFadeOut = 2
    }

    // Start is called before the first frame update
    void Start()
    {
        _startingOnePlayerGame = true;
        _startingTwoPlayerGame = false;
        _quittingGame = false;

        _ps4Controller = false;
        _xboxController = false;

        _mainMenuFadeValue = 0;

        _mainMenuAudio = GetComponent<AudioSource>();

        _mainMenuAudio.volume = 0;
        _mainMenuAudio.clip = _mainMenuMusic;
        _mainMenuAudio.loop = true;
        _mainMenuAudio.Play();

        _mainMenuController = 
            MainMenu.MainMenuController.MainMenuFadeIn;

        StartCoroutine("MainMenuManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (_mainMenuFadeValue < 1)
            return;

        string[] _joyStickNames = Input.GetJoystickNames();
        for (int i = 0; i < _joyStickNames.Length; i++)
        {
            if (_joyStickNames[i].Length == 0) return;

            if (_joyStickNames[i].Length == 19)
                _ps4Controller = true;

            if (_joyStickNames[i].Length == 33)
                _xboxController = true;
        }

        if (_mainMenuVerticalInputTimer > 0)
            _mainMenuVerticalInputTimer -= 1f * Time.deltaTime;

        if (Input.GetAxis("Vertical") > 0f && _selectedButton == 0)
            return;
        if (Input.GetAxis("Vertical") > 0f && _selectedButton == 1)
        {
            if (_mainMenuVerticalInputTimer > 0)
                return;

            _mainMenuVerticalInputTimer = _mainMenuVerticalInputDelay;
            _selectedButton = 0;
        }
        if (Input.GetAxis("Vertical") < 0f && _selectedButton == 1)
        {
            if (_mainMenuVerticalInputTimer > 0)
                return;

            _mainMenuVerticalInputTimer = _mainMenuVerticalInputDelay;
            _selectedButton = 2;
        }
        if (Input.GetAxis("Vertical") < 0f && _selectedButton == 2)
            return;

        if (Time.deltaTime >= _timeDelay && (Input.GetButton("Fire1")))
        {
            StartCoroutine("MainMenuButtonPress");
            _timeDelay = Time.deltaTime + _timeBetweenButtonPress;
        }

    }

    private IEnumerator MainMenuManager()
    {
        while (true)
        {
            switch (_mainMenuController)
            {
                case MainMenuController.MainMenuFadeIn: MainMenuFadeIn(); break;
                case MainMenuController.MainMenuAtIdle: MainMenuAtIdle(); break;
                case MainMenuController.MainMenuFadeOut: MainMenuFadeOut(); break;
            }
            yield return null;
        }
    }
    private void MainMenuFadeIn()
    {
        _mainMenuAudio.volume += _mainMenuFadeSpeed * Time.deltaTime;
        _mainMenuFadeSpeed += _mainMenuFadeSpeed * Time.deltaTime;

        if (_mainMenuFadeValue > 1)
            _mainMenuFadeValue = 1;

        if (_mainMenuFadeValue == 1)
            _mainMenuController = MainMenu.MainMenuController.MainMenuAtIdle;
    }

    private void MainMenuAtIdle()
    {
        if (_startingOnePlayerGame || _quittingGame)
            _mainMenuController = MainMenu.MainMenuController.MainMenuFadeOut;
    }

    private void MainMenuFadeOut()
    {
        _mainMenuAudio.volume -= _mainMenuFadeSpeed * Time.deltaTime;
        _mainMenuFadeSpeed -= _mainMenuFadeSpeed * Time.deltaTime;

        if (_mainMenuFadeValue < 0)
            _mainMenuFadeValue = 0;

        Debug.Log("Fade Out " + _mainMenuFadeValue + " " + _startingOnePlayerGame);
        if (_mainMenuFadeValue == 0 && _startingOnePlayerGame)
            SceneManager.LoadScene("ChooseCharacter");

    }

    private void MainMenuButtonPress()
    {
        GUI.FocusControl(_mainMenuButtons[_selectedButton]);

        switch (_selectedButton)
        {
            case 0: 
                _mainMenuAudio.PlayOneShot(_mainMenuStartButtonAudio);
                _startingOnePlayerGame = true;
                GameObject.FindGameObjectWithTag("OnePlayerManager").GetComponent<OnePlayerManager>().enabled = true;
                Debug.Log("Fade Out " + _mainMenuFadeValue + " " + _startingOnePlayerGame);
                if (_mainMenuFadeValue == 0 && _startingOnePlayerGame)
                    SceneManager.LoadScene("ChooseCharacter");
                break;
            case 1:
                _mainMenuAudio.PlayOneShot(_mainMenuStartButtonAudio);
                _startingTwoPlayerGame = true;
                GameObject.FindGameObjectWithTag("TwoPlayerManager").GetComponent<TwoPlayerManager>().enabled = true;
                break;
            case 2:
                _mainMenuAudio.PlayOneShot(_mainMenuQuitButtonAudio);
                _quittingGame = true;
                break;
        }
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
            _mainMenuBackground);
        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
            _mainMenuTitle);

        //GUI.color = new Color(1, 1, 1, _mainMenuFadeValue);

        GUI.BeginGroup(new Rect(Screen.width / 2 - _mainMenuButtonWidth / 2, Screen.height / 1.5f,
            _mainMenuButtonWidth, _mainMenuButtonHeight * 3
            + _mainMenuGUIOffset * 2));

        GUI.SetNextControlName("_onePlayer");
        if(GUI.Button(new Rect(0,0, _mainMenuButtonWidth, _mainMenuButtonHeight), 
            "Single Player"))
        {
            _selectedButton = 0;
            MainMenuButtonPress();
        }
        GUI.SetNextControlName("_twoPlayer");
        if (GUI.Button(new Rect(0, _mainMenuButtonHeight + _mainMenuGUIOffset,
            _mainMenuButtonWidth, _mainMenuButtonHeight),
            "VS.!!"))
        {
            _selectedButton = 1;
            MainMenuButtonPress();
        }
        GUI.SetNextControlName("_quit");
        if (GUI.Button(new Rect(0, _mainMenuButtonHeight*2 + _mainMenuGUIOffset*2,
            _mainMenuButtonWidth, _mainMenuButtonHeight),
            "Give up"))
        {
            _selectedButton = 2;
            MainMenuButtonPress();
        }

        GUI.EndGroup();

        if (_ps4Controller || _xboxController)
            GUI.FocusControl(_mainMenuButtons[_selectedButton]);
    }
}

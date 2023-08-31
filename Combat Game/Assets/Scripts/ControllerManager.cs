using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ControllerManager : MonoBehaviour
{
    public Texture2D _controllerNotDetected;

    public bool _pS4Controller;
    public bool _xBOXController;
    //public bool _pCController;
    public bool _controllerDetected;

    public static bool _startUpFinished;

    private AudioSource _cmAudio;
    public AudioClip _controllerDetectedAudioClip;


    void Awake()
    {
        _pS4Controller = false;
        _xBOXController = false;
        //_pCController = false;
        _controllerDetected = false;

        _startUpFinished = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_controllerDetected)
            return;

        if (_startUpFinished)
            Time.timeScale = 0;
    }

    void LateUpdate()
    {
        if (_startUpFinished)
            _cmAudio = GetComponent<AudioSource>();

        string[] _joyStickNames = Input.GetJoystickNames();

        for(int i = 0; i< _joyStickNames.Length; i++)
        {
            if(_joyStickNames[i].Length == 19)
            {
                _pS4Controller = true;

                if (_controllerDetected)
                    return;
                if (_startUpFinished)
                    _cmAudio.PlayOneShot(_controllerDetectedAudioClip);

                Time.timeScale = 1;

                _controllerDetected = true;
            }

            if (_joyStickNames[i].Length == 33)
            {
                _xBOXController = true;

                if (_controllerDetected)
                    return;
                if (_startUpFinished)
                    _cmAudio.PlayOneShot(_controllerDetectedAudioClip);

                Time.timeScale = 1;

                _controllerDetected = true;
            }

            if (_joyStickNames[i].Length != 0) return;

            if (string.IsNullOrEmpty(_joyStickNames[0]))
                _controllerDetected = false;

        }
        
    }

    private void OnGUI()
    {
        if (!_startUpFinished)
            return;

        if (_controllerDetected)
            return;

        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
            _controllerNotDetected);
    }
}

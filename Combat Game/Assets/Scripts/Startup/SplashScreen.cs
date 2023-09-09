using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SplashScreen : MonoBehaviour
{
    public Texture2D _splashScreenBackground;
    public Texture2D _splashScreenText;

    private AudioSource _splashScreenAudio;
    public AudioClip _splashScreenMusic;

    private float _splashScreenFadeValue;
    private float _splashScreenFadeSpeed = 0.5f;

    private SplashScreenController _splashScreenController; 

    private enum SplashScreenController 
    { 
        SplashScreenFadeIn = 0,
        SplashScreenFadeOut = 1
    }

    void Awake()
    {
        _splashScreenFadeValue = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        _splashScreenAudio = GetComponent<AudioSource>();

        _splashScreenAudio.volume = 0;
        _splashScreenAudio.clip = _splashScreenMusic;
        _splashScreenAudio.loop = true;
        _splashScreenAudio.Play();

        _splashScreenController = 
            SplashScreen.SplashScreenController.SplashScreenFadeIn;
        
        StartCoroutine("SplashScreenManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SplashScreenManager()
    {
        while (true)
        {
            switch(_splashScreenController)
            {
                case SplashScreenController.SplashScreenFadeIn: SplashScreenFadeIn(); break;
                case SplashScreenController.SplashScreenFadeOut: SplashScreenFadeOut(); break;
            }
            yield return null;
        }
    }
    private void SplashScreenFadeIn()
    {
        _splashScreenAudio.volume += _splashScreenFadeSpeed * Time.deltaTime;
        _splashScreenFadeValue += _splashScreenFadeSpeed * Time.deltaTime;
        
        if (_splashScreenFadeValue > 1)
            _splashScreenFadeValue = 1;

        if (_splashScreenFadeValue == 1)
            _splashScreenController = SplashScreenController.SplashScreenFadeOut;
    }

    private void SplashScreenFadeOut()
    {
        _splashScreenAudio.volume -= _splashScreenFadeSpeed * Time.deltaTime;
        _splashScreenFadeValue -= _splashScreenFadeSpeed * Time.deltaTime;

        if (_splashScreenFadeValue < 0)
            _splashScreenFadeValue = 0;

        if (_splashScreenFadeValue == 0)
            SceneManager.LoadScene("ControllerWarning");
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
            _splashScreenBackground);

        GUI.color = new Color(1,1,1, _splashScreenFadeValue);

        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
            _splashScreenText);
    }
}

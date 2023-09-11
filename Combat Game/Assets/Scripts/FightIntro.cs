using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FightIntro : MonoBehaviour
{
    private int _roundCounter;

    public Texture2D _roundOneText;
    public Texture2D _roundTwoText;
    public Texture2D _roundThreeText;
    public Texture2D _fightText;

    private AudioSource _fightIntroAudioSource;

    public AudioClip _roundOneAnnouncement;
    public AudioClip _roundTwoAnnouncement;
    public AudioClip _roundThreeAnnouncement;
    public AudioClip _fightAnnouncement;

    private float _fightIntroFadeValue;
    private float _fightIntroFadeSpeed = 0.5f;

    private bool _displayingRound;
    private bool _displayingFight;

    public static bool _fightIntroFinished;

    private FightIntroState _fightIntroState; 

    private enum FightIntroState
    {
        FightIntroInitialize = 0, 
        FightIntroFadeInRound, 
        FightIntroFightAnnouncement
    }
    // Start is called before the first frame update
    void Start()
    {
        _fightIntroAudioSource = GetComponent<AudioSource>();

        _fightIntroFinished = false;

        _fightIntroFadeValue = 0;

        _roundCounter = 1;

        _displayingRound = false;
        _displayingFight = false;

        StartCoroutine("FightIntroManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FightIntroManager()
    {
        while (true)
        {
            switch (_fightIntroState)
            {
                case FightIntroState.FightIntroInitialize: FightIntroInitialize(); break;
                case FightIntroState.FightIntroFadeInRound: FightIntroFadeInRound(); break;
                case FightIntroState.FightIntroFightAnnouncement: FightIntroFightAnnouncement(); break;
            }
            yield return null;
        }
    }

    private void FightIntroInitialize()
    {
        _displayingRound = true;

        if (_roundCounter == 1)
            _fightIntroAudioSource.PlayOneShot(_roundOneAnnouncement);
        if (_roundCounter == 2)
            _fightIntroAudioSource.PlayOneShot(_roundTwoAnnouncement);
        if (_roundCounter == 2)
            _fightIntroAudioSource.PlayOneShot(_roundThreeAnnouncement);

        _fightIntroState = FightIntroState.FightIntroFadeInRound;
    }

    private void FightIntroFadeInRound()
    {
        _fightIntroFadeValue += _fightIntroFadeSpeed * Time.deltaTime;

        if (_fightIntroFadeValue > 1) _fightIntroFadeValue = 1;

        if(_fightIntroFadeValue == 1)
        {
            _displayingFight = true;

            _fightIntroAudioSource.PlayOneShot(_fightAnnouncement);

            _fightIntroState = FightIntroState.FightIntroFightAnnouncement;
        }
    }

    private void FightIntroFightAnnouncement()
    {
        _fightIntroFadeValue -= _fightIntroFadeSpeed * 2 * Time.deltaTime;

        if(_fightIntroFadeValue < 0)
            _fightIntroFadeValue= 0;

        if(_fightIntroFadeValue == 0)
        {
            _displayingRound = false;
            _displayingFight = false;

            _fightIntroFinished = true;

            StopCoroutine("FightIntroManager");
        }
    }

    private void IncreaseRoundCounter()
    {
        _roundCounter++;
    }

    private void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, _fightIntroFadeValue);

        if (_displayingRound)
        {
            if(_roundCounter ==1)
                GUI.DrawTexture(new Rect(0, 0,
                    Screen.width, Screen.height),
                    _roundOneText);

            if (_roundCounter == 2)
                GUI.DrawTexture(new Rect(0, 0,
                    Screen.width, Screen.height),
                    _roundTwoText);

            if (_roundCounter == 3)
                GUI.DrawTexture(new Rect(0, 0,
                    Screen.width, Screen.height),
                    _roundThreeText);
        }

        if (_displayingFight)
        {
            GUI.DrawTexture(new Rect(0, 0,
                    Screen.width, Screen.height),
                    _fightText);
        }
    }
}

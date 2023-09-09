using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerWarning : ControllerManager
{
    public Texture2D _controllerWarningBackground;
    public Texture2D _controllerWarningText;
    public Texture2D _controllerDetectedText;

    private float _controllerWarningFadeValue;
    private float _controllerWarningFadeSpeed = 0.5f;
    private bool _controllerConditionsMet; 

    // Start is called before the first frame update
    void Start()
    {
        _controllerWarningFadeValue = 1;
        _controllerConditionsMet = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (true)//_controllerDetected
            StartCoroutine("WaitToLoadMainMenu");

        if (!_controllerConditionsMet)
            return;

        if (_controllerConditionsMet)
        {
            _controllerWarningFadeValue -=
                _controllerWarningFadeSpeed * Time.deltaTime;

            if (_controllerWarningFadeValue < 0)
                _controllerWarningFadeValue = 0;

            if(_controllerWarningFadeValue == 0)
            {
                _startUpFinished = true;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    private IEnumerator WaitToLoadMainMenu()
    {
        yield return new WaitForSeconds(2);

        _controllerConditionsMet = true;
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
            _controllerWarningBackground);

        GUI.color = new Color(1, 1, 1, _controllerWarningFadeValue);

        GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
             _controllerWarningText);

        if(_controllerDetected)
            GUI.DrawTexture(new Rect(0, 0,
            Screen.width, Screen.height),
             _controllerDetectedText);

    }
}

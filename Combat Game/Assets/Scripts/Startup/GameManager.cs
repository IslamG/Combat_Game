using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Vector3 _playerStartingPosition = new Vector3(-3.26f, -0.65f, -6f);
    public static Vector3 _playerStartingRotation = new Vector3();

    public static Vector3 _opponentStartingPosition = new Vector3 (3.66f, -0.3f, -6);
    public static Vector3 _opponentStartingRotation = new Vector3(0, 180, 0);

    void Awake()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        GameObject.FindGameObjectWithTag("OnePlayerManager").GetComponent<OnePlayerManager>().enabled = false;
        GameObject.FindGameObjectWithTag("TwoPlayerManager").GetComponent<TwoPlayerManager>().enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

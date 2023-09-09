using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseCharacterManager : MonoBehaviour
{
    public static bool _char1;
    public static bool _char2;
    public static bool _char3;
    public static bool _char4;

    void Awake()
    {
        _char1 = false;
        _char2 = false;
        _char3 = false;
        _char4 = false;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    private int _fightTimer = 99;
    public static int _currentFightTimer;

    // Start is called before the first frame update
    void Start()
    {
        _currentFightTimer = _fightTimer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

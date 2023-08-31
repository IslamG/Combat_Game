using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMatch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("OnePlayerManager").GetComponent<OnePlayerManager>()
            .SendMessage("LoadPlayerOneCharacter");

        GameObject.FindGameObjectWithTag("OpponentManager").GetComponent<OpponentManager>()
            .SendMessage("LoadCurrentOpponent");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

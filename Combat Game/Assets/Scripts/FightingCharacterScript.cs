using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingCharacterScript : MonoBehaviour
{
    public GameObject playerObject;
    private Animator playerAnimator;

    public bool isBlocking = false;

    void Start()
    {
        playerAnimator = playerObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Attack1();
        }
        if (Input.GetKeyDown("left ctrl"))
        {
            isBlocking = true;
            SetBlocking(isBlocking);
        }
        if (Input.GetKeyUp("left ctrl"))
        {
            isBlocking = false;
            SetBlocking(isBlocking);
        }
    }

    public void Attack1()
    {
        playerAnimator.SetBool("Attack", true);
        Debug.Log(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            playerAnimator.SetBool("Attack", false);
    }
    public void SetBlocking(bool block)
    {
        playerAnimator.SetBool("Block", block);
    }
}

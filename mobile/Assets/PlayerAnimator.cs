using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator playerMobile;

    void Start()
    {

    }


    void Update()
    {
        palyerW();
        palyerS();
    }

    public void palyerW()
    {
        if (Input.GetKeyDown(KeyCode.W)|| Input.GetKeyDown(KeyCode.A))
        {
            playerMobile.GetComponent<Animator>().SetBool("前", true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            playerMobile.GetComponent<Animator>().SetBool("前", false);
        }
    }
    public void palyerS()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerMobile.GetComponent<Animator>().SetBool("後", true);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            playerMobile.GetComponent<Animator>().SetBool("後", false);
        }
    }
}

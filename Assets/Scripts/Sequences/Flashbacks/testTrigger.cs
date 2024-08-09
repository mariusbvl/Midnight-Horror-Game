using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTrigger : MonoBehaviour
{
    public FlashbackManager flashbackManager;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            flashbackManager.PlayFlashback(0);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            flashbackManager.PlayFlashback(1);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            flashbackManager.PlayFlashback(2);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            flashbackManager.PlayFlashback(3);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            flashbackManager.PlayFlashback(4);
        }
    }
}

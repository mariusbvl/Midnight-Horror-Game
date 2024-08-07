using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbackManager : MonoBehaviour
{
    public MonoBehaviour[] flashbacks;

    public void PlayFlashback(int index)
    {
        if (index >= 0 && index < flashbacks.Length)
        {
            foreach (MonoBehaviour flashback in flashbacks)
            {
                flashback.gameObject.SetActive(false);
            }

            flashbacks[index].gameObject.SetActive(true);
            flashbacks[index].Invoke("PlayFlashback", 0);
        }
        else
        {
            Debug.Log("Flashback index out of bounds!");
        }
    }
}

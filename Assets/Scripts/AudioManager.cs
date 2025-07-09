using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource BGM;

    public void ChangeBGM(AudioClip newClip)
    {
        BGM.Stop();
        BGM.clip = newClip;
        BGM.Play();
    }
}

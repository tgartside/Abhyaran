using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwapTrigger : MonoBehaviour
{
    public AudioClip newTrack;
    private AudioManager audioManager;

    public void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (newTrack != null)
            {
                audioManager.ChangeBGM(newTrack);
            }
        }
    }

}

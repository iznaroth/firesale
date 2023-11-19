using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasStarted = false;
    
    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        hasStarted = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(hasStarted && !audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }

    public void PlayAudio(AudioClip newClip, float volumeScaler, float newPitch)
    {
        if(newClip != null)
        {
            audioSource.pitch = newPitch;
            audioSource.PlayOneShot(newClip, volumeScaler);
            hasStarted = true;
        }
        else
        {
            Debug.Log("Spawned Audio Object with no audio clip");
            Destroy(this.gameObject);
        }
    }
}

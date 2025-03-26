using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public float volume = 0.75f;
    public bool isRandom = false;
    public void PlaySoundClip(string soundName) 
    { 
        if (isRandom)
        {
            AudioHandler.Instance.PlayRandomPitch(soundName, volume);
        }
        else
        {
            AudioHandler.Instance.PlaySound(soundName, volume);
        }
    }
}

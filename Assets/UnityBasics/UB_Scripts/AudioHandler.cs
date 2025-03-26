using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{    
    public static AudioHandler Instance;
    public List<AudioClip> sfx = new List<AudioClip>();
    public GameObject audioPref;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySound(string sound, float vol)
    {
        AudioClip s = null;
        var newSource = Instantiate(audioPref);
        var aud = newSource.GetComponent<AudioSource>();
        foreach (AudioClip clip in sfx)
        {
            if (clip.name == sound)
            {
                s = clip;
            }
        }
        if (s == null)
        {
            return;
        }
        aud.volume = vol;
        aud.PlayOneShot(s);
        Destroy(newSource, s.length + 0.1f);
    }
    public void PlayRandomPitch(string sound, float vol)
    {
        AudioClip s = null;
        var newSource = Instantiate(audioPref);
        var aud = newSource.GetComponent<AudioSource>();
        foreach (AudioClip clip in sfx)
        {
            if (clip.name == sound)
            {
                s = clip;
            }
        }
        if (s == null)
        {
            return;
        }
        float randomPitch = Random.Range(aud.pitch - 0.18f, aud.pitch + 0.18f);
        aud.volume = vol;
        aud.pitch = randomPitch;
        aud.PlayOneShot(s);
        Destroy(newSource, s.length + 0.1f);
    }
}

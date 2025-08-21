using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public void PlayRandomSound(string sound0, string sound1, string sound2 = null, string sound3 = null, bool doRandomPitch = true, bool do3D = false, Vector3 position = default, float vol = 1)
    {
        AudioClip s = null;
        GameObject newSource;

        if (do3D == true)
        {
            newSource = Instantiate(audioPref, position, Quaternion.identity);
            newSource.GetComponent<AudioSource>().spatialBlend = 0.5f;
        }
        else
        {
            newSource = Instantiate(audioPref);
        }
        var aud = newSource.GetComponent<AudioSource>();
        List<string> allSounds = new List<string>();
        if (sound0 != null) //THIS IS HORRIBLENESS
            allSounds.Add(sound0);
        if (sound1 != null) 
            allSounds.Add(sound1);
        if (sound2 != null) 
            allSounds.Add(sound2);
        if (sound3 != null) 
            allSounds.Add(sound3);

        int r = Random.Range(0, allSounds.Count - 1);
        foreach (AudioClip clip in sfx)
        {
            if (clip.name == allSounds[r])
            {
                s = clip;
            }
        }
        if (s == null)
        {
            return;
        }
        
        float randomPitch = Random.Range(aud.pitch - 0.18f, aud.pitch + 0.18f);        
        if (doRandomPitch == true)
            aud.pitch = randomPitch;

        aud.volume = vol;

        aud.PlayOneShot(s);
        Destroy(newSource, s.length + 0.1f);
    }
}

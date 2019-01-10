using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    public static AudioPlayer instance;

    public int numSources = 15;

    private AudioSource[] sources;

    private void Start()
    {
        sources = new AudioSource[numSources];
        for(int i = 0; i < numSources; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
        }

        instance = this;
    }

    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        for(int i = 0; i < numSources; i++)
        {
            AudioSource source = sources[i];

            if(i == numSources - 1 || !source.isPlaying)
            {
                source.Stop();
                source.clip = clip;
                source.pitch = pitch;
                source.volume = volume;
                source.spatialBlend = 0f;
                source.Play();
            }
        }
    }
}

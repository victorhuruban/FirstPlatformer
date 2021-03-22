﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Sound {

    public string name;
    public AudioClip clip;


    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;
    private AudioSource source;

    public void SetSource(AudioSource _source) {
        source = _source;
        source.clip = clip;
    }

    public void Play() {
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] Sound[] sounds;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogWarning("Multiple AudioManager instances");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string clipName) {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == clipName) {
                sounds[i].Play();
                return;
            }
        }

        // NO SOUND WITH clipName
        Debug.LogWarning("No clip found");
    }
}

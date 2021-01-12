using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip mBackgroundMusic;

    private void Start()
    {
        GameObject AudioObject = AudioSystem.Instance.GetAudioObject();
        AudioSource ObjectAudioSource = AudioObject.GetComponent<AudioSource>();

        AudioObject.SetActive(true);

        ObjectAudioSource.loop = true;
        ObjectAudioSource.clip = mBackgroundMusic;

        ObjectAudioSource.volume = 0.1f;

        ObjectAudioSource.Play();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip mBackgroundMusic;

    private GameObject AudioObject;
    private AudioSource ObjectAudioSource;

    private void Start()
    {
        AudioObject = AudioSystem.Instance.GetAudioObject();
        ObjectAudioSource = AudioObject.GetComponent<AudioSource>();

        AudioObject.SetActive(true);

        ObjectAudioSource.loop = true;
        ObjectAudioSource.clip = mBackgroundMusic;

        ObjectAudioSource.volume = 0.1f * (float)PlayerPrefs.GetInt("AudioVolumeSliderValue") / 100.0f; ;

        ObjectAudioSource.Play();
    }

    public void StopMusic()
    {
        ObjectAudioSource.Stop();
        AudioObject.SetActive(false);
    }
}
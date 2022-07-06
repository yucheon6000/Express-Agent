using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private static AudioController instance;

    [SerializeField]
    private AudioSource itemAudioSource;
    [SerializeField]
    private AudioSource monsterAudioSource;
    [SerializeField]
    private AudioSource gameBgmAudioSource;
    [SerializeField]
    private AudioSource shopBgmAudioSource;

    private void Awake() => instance = this;

    private void Start()
    {
        gameBgmAudioSource.Play();
        shopBgmAudioSource.Play();
        shopBgmAudioSource.Pause();
    }

    public static void PlayItemAudio(AudioClip audioClip)
    {
        if (audioClip == null || instance == null || instance.itemAudioSource == null) return;

        instance.itemAudioSource.Stop();
        instance.itemAudioSource.clip = audioClip;
        instance.itemAudioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerCollision.TAG))
        {
            gameBgmAudioSource.Pause();
            shopBgmAudioSource.UnPause();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PlayerCollision.TAG))
        {
            gameBgmAudioSource.UnPause();
            shopBgmAudioSource.Pause();
        }
    }

    public static void PlayMonsterAudioClip(AudioClip audioClip)
    {
        if (audioClip == null || instance == null || instance.monsterAudioSource == null) return;

        instance.monsterAudioSource.PlayOneShot(audioClip);
    }
}

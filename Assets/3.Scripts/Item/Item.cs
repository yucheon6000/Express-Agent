using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private AudioClip itemAudioClip;

    protected virtual void OnDisable()
    {
        ObjectPooler.ReturnToPool(this.gameObject);
    }

    protected void PlayAudio()
    {
        AudioController.PlayItemAudio(itemAudioClip);
    }
}

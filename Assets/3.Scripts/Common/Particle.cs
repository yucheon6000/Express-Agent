using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [Header("[Particle]")]
    [SerializeField]
    private ParticleSystem[] particleSystems;

    private void OnEnable()
    {
        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
    }

    private void Update()
    {
        foreach (var particleSystem in particleSystems)
            if (particleSystem.isPlaying) return;

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }
}

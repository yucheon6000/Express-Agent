using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [Header("[Particle]")]
    [SerializeField]
    private new ParticleSystem particleSystem;

    private void OnEnable()
    {
        particleSystem.Play();
    }

    private void Update()
    {
        if (!particleSystem.isPlaying)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }
}

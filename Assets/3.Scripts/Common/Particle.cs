using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [Header("[Particle]")]
    [SerializeField]
    private new ParticleSystem particleSystem;

    private void Awake()
    {
        if (!particleSystem)
            particleSystem = GetComponent<ParticleSystem>();
        if (!particleSystem)
            particleSystem = GetComponentInChildren<ParticleSystem>();
    }

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

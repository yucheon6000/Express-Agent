using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField]
    private float amount;
    [SerializeField]
    private float duration;

    [SerializeField]
    private bool shakeOnDisable = false;

    private void OnDisable()
    {
        if (shakeOnDisable)
            Shake();
    }

    public void Shake()
    {
        TargetCamera.Shake(amount, duration);
    }
}

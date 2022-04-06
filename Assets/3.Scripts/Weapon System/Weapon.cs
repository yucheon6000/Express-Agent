using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("[Attacks]")]
    [SerializeField]
    private Attack[] attacks;

    [Header("[@Debug]")]
    [SerializeField]
    private bool debugMode = false;

    private void Start()
    {
        if (debugMode)
            StartTrigger();
    }

    [ContextMenu("Start Trigger")]
    private void StartTrigger()
    {
        foreach (Attack attack in attacks)
            attack.StartAttack();
    }

    [ContextMenu("Stop Trigger")]
    private void StopTrigger()
    {
        foreach (Attack attack in attacks)
            attack.StopAttack();
    }
}

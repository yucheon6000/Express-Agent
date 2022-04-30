using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderChanger : MonoBehaviour
{
    [SerializeField]
    private PlayerAngleDetector playerAngleDetector;

    [SerializeField]
    private Collider2D[] colliders;
    private int currentIndex = 0;

    public Collider2D CurrentCollider => colliders[currentIndex];

    private void Start()
    {
        DisableAllColliders();
        colliders[currentIndex].enabled = true;
        playerAngleDetector.AddPlayerAngleAction(OnPlayerAngleChanged);
    }

    private void OnPlayerAngleChanged(PlayerAngle playerAngle)
    {
        DisableAllColliders();
        currentIndex = (int)playerAngle;
        Collider2D collider = colliders[currentIndex];
        if (collider) collider.enabled = true;
    }

    private void DisableAllColliders()
    {
        foreach (Collider2D collider in colliders)
            collider.enabled = false;
    }
}

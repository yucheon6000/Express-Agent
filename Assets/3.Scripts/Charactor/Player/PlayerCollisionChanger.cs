using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionChanger : MonoBehaviour
{
    [SerializeField]
    private PlayerAngleDetector playerAngleDetector;

    [SerializeField]
    private Collider2D[] colliders;

    private void Start()
    {
        DisableAllColliders();
        playerAngleDetector.AddPlayerAngleAction(OnPlayerAngleChanged);
    }

    private void OnPlayerAngleChanged(PlayerAngle playerAngle)
    {
        DisableAllColliders();
        Collider2D collider = colliders[(int)playerAngle];
        if (collider) collider.enabled = true;
    }

    private void DisableAllColliders()
    {
        foreach (Collider2D collider in colliders)
            collider.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteChanger : MonoBehaviour
{
    [SerializeField]
    private PlayerAngleDetector playerAngleDetector;

    [SerializeField]
    private Animator animator;

    private void Start()
    {
        playerAngleDetector.AddPlayerAngleAction(OnPlayerAngleChanged);
    }

    private void OnPlayerAngleChanged(PlayerAngle playerAngle)
    {
        if (!animator) return;

        animator.SetFloat("angleIndex", (int)playerAngle);
    }
}

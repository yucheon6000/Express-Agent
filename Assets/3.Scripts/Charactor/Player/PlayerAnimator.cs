using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
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

    public static readonly string IsMoving = "isMoving";
    public static readonly string IsAttacking = "isAttacking";

    public void SetState(string state, bool value)
    {
        animator.SetBool(state, value);
    }


    public static readonly int HitLeft = 0;
    public static readonly int HitRight = 4;
    public void Hit(int hitDirection)
    {
        animator.SetFloat("hitDirection", hitDirection);
        animator.Play("Hit", -1);
    }
}

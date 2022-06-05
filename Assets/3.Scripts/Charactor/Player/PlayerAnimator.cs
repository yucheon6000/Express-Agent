using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private PlayerAngleDetector playerAngleDetector;

    [SerializeField]
    private Animator[] animators;

    [SerializeField]
    private Animator powerUpAnimator;

    private bool isDead = false;

    private void Start()
    {
        playerAngleDetector.AddPlayerAngleAction(OnPlayerAngleChanged);
    }

    private void OnPlayerAngleChanged(PlayerAngle playerAngle)
    {
        if (isDead) return;

        foreach (var animator in animators)
            animator.SetFloat("angleIndex", (int)playerAngle);
    }

    public static readonly string IsMoving = "isMoving";
    public static readonly string IsAttacking = "isAttacking";

    public void SetState(string state, bool value)
    {
        if (isDead) return;

        foreach (var animator in animators)
            animator.SetBool(state, value);
    }

    public static readonly int HitLeft = 0;
    public static readonly int HitRight = 4;
    public void Hit(int hitDirection)
    {
        if (isDead) return;

        foreach (var animator in animators)
            animator.Play("Hit", -1);
    }

    public void SetAnimationSpeedPercent(float percet)
    {
        percet = Mathf.Clamp(percet, 0, 1);

        foreach (var animator in animators)
            animator.speed = percet;
    }

    public void Dead()
    {
        if (isDead) return;

        isDead = true;

        foreach (var animator in animators)
            animator.Play("Dead", -1);
    }

    public void PowerUp()
    {
        powerUpAnimator.SetTrigger("Trigger");
    }
}

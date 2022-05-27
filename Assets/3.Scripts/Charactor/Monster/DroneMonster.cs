using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneMonster : Monster
{
    [Header("[DroneMonster]")]
    [SerializeField]
    private NavMeshAgent agent;
    private Player target;

    [SerializeField]
    private ClosestTargetDetector detector;

    [SerializeField]
    private Animator animator;

    protected override void Start()
    {
        base.Start();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        detector.onUpdatedClosestTarget.AddListener((transform, _) =>
        {
            target = transform.gameObject.GetComponentInParent<Player>();
            weapon.StartTrigger();
            animator.SetBool("isMoving", true);
        });
    }

    protected override void Update()
    {
        base.Update();

        // 넉백 중        
        if (knockBack.IsKnockBacking)
        {
            if (agent.enabled)
                agent.enabled = false;
            return;
        }
        // 넉백 중 아님
        else
        {
            if (!agent.enabled)
                agent.enabled = true;

            if (movement.enabled)
                movement.enabled = false;
        }

        // 죽었으면 타겟팅 안 함
        if (isDead) return;

        // 타켓 있을 경우
        if (target)
        {
            agent.speed = characterStat.MoveSpeed;
            agent.SetDestination(target.TargetPosition);
        }
        // 타켓 없을 경우
        else
        {
            movement.SetMoveDirection(Vector2.zero);
            return;
        }
    }

    public override void Hit(float attack, float knockBack, Vector3 hitPosition)
    {
        animator.Play("Hit", -1);
        base.Hit(attack, knockBack, hitPosition);
    }

    protected override void OnDead()
    {
        if (isDead) return;
        base.OnDead();

        agent.ResetPath();
        agent.enabled = false;

        animator.SetBool("isDead", true);
        Invoke("Inactive", 1.05f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        agent.enabled = true;
        target = null;

        animator.SetBool("isMoving", false);
        animator.SetBool("isDead", false);
    }
}

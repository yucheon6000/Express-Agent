using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TotemMonster : Monster
{
    [Header("[TotemMonster]")]
    private Player target;
    [SerializeField]
    private Transform pivot;
    private Vector2 moveDirecion = Vector2.zero;

    [SerializeField]
    private ClosestTargetDetector detector;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private int minJumpCount = 2;
    [SerializeField]
    private int maxJumpCount = 5;
    [SerializeField]
    private float jumpTime = 1f;
    [SerializeField]
    private float jumpForce = 1f;
    [SerializeField]
    private float jumpDeltaTime = 0.3f;
    [SerializeField]
    private float minIdleTime = 0.5f;
    [SerializeField]
    private float maxIdleTime = 2f;

    protected override void Start()
    {
        base.Start();
        movement.MoveSpeedType = MoveSpeedType.Manual;

        detector.onUpdatedClosestTarget.AddListener((transform, _) =>
        {
            target = transform.gameObject.GetComponentInParent<Player>();
            StartMove();
            // weapon.StartTrigger();
            animator.SetBool("isMoving", true);
        });
    }

    protected override void Update()
    {
        base.Update();

        // 넉백 중        
        if (knockBack.IsKnockBacking)
        {
            // if (agent.enabled)
            //     agent.enabled = false;
            return;
        }
        // 넉백 중 아님
        else
        {
            // if (!agent.enabled)
            //     agent.enabled = true;

            // if (movement.enabled)
            //     movement.enabled = false;
        }

        // 죽었으면 타겟팅 안 함
        if (isDead) return;

        // 타켓 있을 경우
        if (target)
        {
            // agent.speed = characterStat.MoveSpeed;
            // agent.SetDestination(target.TargetPosition);
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
        if (isDead) return;

        animator.Play("Hit", -1);
        // IncreaseHp(-attack);
        base.Hit(attack, knockBack, hitPosition);
    }

    protected override void OnDead()
    {
        if (isDead) return;
        base.OnDead();

        // agent.ResetPath();
        // agent.enabled = false;

        animator.Play("Death", -1);
        Invoke("Inactive", 1.05f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // movement.enabled = true;
        // agent.enabled = true;

        Transform targetTransform = detector.Target;
        if (targetTransform)
            target = targetTransform.GetComponent<Player>();

        animator.SetBool("isMoving", false);
        animator.SetBool("isDead", false);
    }

    private bool isMoving = false;
    private void StartMove()
    {
        if (isMoving) return;
        isMoving = true;

        StopAllCoroutines();

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        // 점프(이동)
        int jumpCount = Random.Range(minJumpCount, maxJumpCount + 1);
        for (int i = 0; i < jumpCount; i++)
        {
            print("jump " + i);
            yield return StartCoroutine(JumpRoutine());

            yield return new WaitForSeconds(jumpDeltaTime);
        }

        // 공격

        // 쉬기
        isMoving = false;
        movement.SetMoveDirection(Vector2.zero);
        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
        StartMove();
    }

    private IEnumerator JumpRoutine()
    {
        UpdateMoveDirection();

        float timer = 0;
        float percent = 0;

        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / jumpTime;

            Vector2 jumpAmount = Vector2.up * jumpForce * Mathf.Sin(percent * Mathf.PI);
            Vector2 moveAmount = moveDirecion * CharacterStat.MoveSpeed;
            moveAmount += jumpAmount;
            movement.SetMoveSpeed(moveAmount.magnitude);
            movement.SetMoveDirection(moveAmount);

            yield return null;
        }
    }

    private void UpdateMoveDirection()
    {
        moveDirecion = target.transform.position - pivot.position;
        moveDirecion.Normalize();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pivot.position, pivot.position + movement.MovedForce);
    }
}

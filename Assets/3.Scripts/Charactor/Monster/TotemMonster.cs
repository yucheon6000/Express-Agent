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
    private Vector2 moveDirection = Vector2.zero;

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
    [SerializeField]
    private int minAttackJumpCount = 2;
    [SerializeField]
    private int maxAttackJumpCount = 5;

    [SerializeField]
    private Transform footTransform;
    [SerializeField]
    private Transform shadowTransform;
    private Vector3 prevShadowPosition;
    [SerializeField]
    private float yTotalMovedForce;

    [Header("[Sound]")]
    [SerializeField]
    private AudioClip jumpAudioClip;

    protected override void Start()
    {
        base.Start();
        movement.MoveSpeedType = MoveSpeedType.Manual;

        detector.onUpdatedClosestTarget.AddListener((transform, _) =>
        {
            if (isDead) return;

            target = Player.Main;
            StartMove();
        });
    }

    protected override void Update()
    {
        base.Update();

        // yTotalMovedForce += movement.MovedForce.y;

        // Vector3 shadowPosition = footTransform.localPosition;
        // shadowPosition.y -= yTotalMovedForce;
        // // shadowPosition.y += movement.MovedForce.y;
        // print("yTotal: " + yTotalMovedForce);

        // shadowTransform.localPosition = shadowPosition;

        // 죽었으면 타겟팅 안 함
        if (isDead) return;

        // 타켓 있을 경우
        if (target)
        {
            StartMove();
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

        movement.SetMoveDirection(Vector2.zero);

        weapon.StopTrigger();
        StopAllCoroutines();

        animator.Play("Death", -1);
        Invoke(INACTIVE, 1.05f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Transform targetTransform = detector.Target;
        if (targetTransform)
            target = Player.Main;

        isMoving = false;
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
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
        animator.SetBool("isMoving", true);
        for (int i = 0; i < jumpCount; i++)
        {
            yield return StartCoroutine(JumpRoutine());
            yield return new WaitForSeconds(jumpDeltaTime);
        }
        animator.SetBool("isMoving", false);

        // 공격
        int attackJumpCount = Random.Range(minAttackJumpCount, maxAttackJumpCount + 1);
        weapon.StartTrigger();
        animator.SetBool("isAttacking", true);
        for (int i = 0; i < attackJumpCount; i++)
        {
            yield return StartCoroutine(JumpRoutine(Vector2.zero));
            yield return new WaitForSeconds(jumpDeltaTime);
        }
        weapon.StopTrigger();
        animator.SetBool("isAttacking", false);

        // 쉬기
        isMoving = false;
        movement.SetMoveDirection(Vector2.zero);
        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
        StartMove();
    }

    private IEnumerator JumpRoutine()
    {
        UpdateMoveDirection();
        yield return StartCoroutine(JumpRoutine(moveDirection));
    }

    private IEnumerator JumpRoutine(Vector2 moveDirection)
    {

        // yTotalMovedForce = 0;

        float timer = 0;
        float percent = 0;

        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / jumpTime;

            Vector2 jumpAmount = Vector2.up * jumpForce * Mathf.Sin(percent * Mathf.PI);
            Vector2 moveAmount = moveDirection * CharacterStat.MoveSpeed;
            moveAmount += jumpAmount;
            movement.SetMoveSpeed(moveAmount.magnitude);
            movement.SetMoveDirection(moveAmount);

            // yTotalMovedForce += Mathf.Sign(Mathf.Sin(percent * Mathf.PI)) * Mathf.Abs(movement.MovedForce.y);
            // print(yTotalMovedForce);

            // shadowTransform.position = footTransform.position;
            // shadowTransform.position -= new Vector3(0, yTotalMovedForce * 2, 0);

            yield return null;
        }

        AudioController.PlayMonsterAudioClip(jumpAudioClip);
    }

    private void UpdateMoveDirection()
    {
        moveDirection = Player.Main.TargetPosition - pivot.position;
        moveDirection.Normalize();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pivot.position, pivot.position + movement.MovedForce);
    }
}

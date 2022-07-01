using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneMonster : Monster
{
    [Space]
    [Header("[DroneMonster]")]
    [SerializeField]
    private NavMeshAgent agent;
    private Player target;

    [SerializeField]
    private ClosestTargetDetector detector;

    [SerializeField]
    private Animator animator;

    /* 공격 */
    [Header("[Attack]")]
    [SerializeField]
    private bool useWeapon = false;                 // 무기 사용 여부
    [SerializeField]
    private float moveTime = 3f;                    // 이동 시간
    [SerializeField]
    private float attackTime = 2f;                  // 공격 시간

    /* 자폭 */
    [Header("[Suicide]")]
    [SerializeField]
    private bool useSuicideBombing = false;         // 자폭 사용 여부
    [SerializeField]
    private Shoot suicideShoot;                     // 자폭 Shoot
    [SerializeField]
    private float minSuicideDistance = 0.3f;        // 이 거리 안에 들어와야 자폭
    private bool isSuicideBombed = false;           // 자폭 했는지 여부

    protected override void Start()
    {
        base.Start();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        suicideShoot?.SetCharacterStat(characterStat);
    }

    protected override void Update()
    {
        base.Update();

        // 넉백 중        
        if (knockBack.IsKnockBacking)
        {
            if (agent.enabled)
                agent.enabled = false;

            if (!movement.enabled)
                movement.enabled = true;

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

        if (!isDead && useSuicideBombing && target &&
            Vector2.Distance(target.TargetPosition, transform.position) < minSuicideDistance)
        {
            Suicide();
        }
    }

    public IEnumerator MoveAndAttackRoutine()
    {
        while (true)
        {
            // 이동
            agent.enabled = true;
            yield return StartCoroutine(MoveRoutine(moveTime));
            agent.enabled = false;

            // 공격
            weapon.StartTrigger();
            yield return new WaitForSeconds(attackTime);
            weapon.StopTrigger();
        }
    }

    public IEnumerator MoveRoutine(float moveTime = -1)
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;

            if (moveTime > 0 && timer > moveTime) break;

            if (isDead) break;

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
            }

            yield return null;
        }

        agent.ResetPath();
    }

    private void Suicide()
    {
        // Shoot 공격
        suicideShoot.StartShoot();
        suicideShoot.StopShoot();

        // 죽인다
        Hit(currentHp * 2, 0, transform.position);
    }

    public override void Hit(float attack, float knockBack, Vector3 hitPosition)
    {
        if (isDead) return;

        animator.Play("Hit", -1);
        base.Hit(attack, knockBack, hitPosition);
    }

    protected override void OnDead()
    {
        if (isDead) return;
        base.OnDead();

        try { weapon.StopTrigger(); } catch { }
        try { StopAllCoroutines(); } catch { }
        try { agent.ResetPath(); } catch { }

        agent.enabled = false;

        animator.Play("Death", -1);
        Invoke(INACTIVE, 1.05f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        detector.onUpdatedClosestTarget.AddListener((transform, _) =>
        {
            target = Player.Main;
            animator.SetBool("isMoving", true);

            if (useWeapon)
                StartCoroutine(MoveAndAttackRoutine());
            else
                StartCoroutine(MoveRoutine());

            detector.onUpdatedClosestTarget.RemoveAllListeners();
        });

        movement.enabled = true;
        agent.enabled = true;

        Transform targetTransform = detector.Target;
        if (targetTransform)
            target = Player.Main;

        animator.SetBool("isMoving", false);
        animator.SetBool("isDead", false);
    }
}

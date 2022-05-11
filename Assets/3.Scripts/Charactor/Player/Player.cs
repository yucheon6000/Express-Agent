using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerType { Main, Sidekick }

public class Player : Character
{
    private static Player main;
    private new static int currentHp = 0;
    public new static int CurrentHp => currentHp;
    private static int maxHp = 0;
    public static int MaxHp => maxHp;
    private static int currentCointCount = 0;
    public static int CurrentCoinCount => currentCointCount;
    private static int currentStaminaCount = 0;
    public static int CurrentStaminaCount => currentStaminaCount;
    private static int maxStaminaCount = 0;
    public static int MaxStaminaCount => maxStaminaCount;

    public static Player Main => main;

    [Header("[Player]")]
    [SerializeField]
    private PlayerType playerType;      // 플레이어 타입

    [Header("[Stamina]")]
    [SerializeField]
    private int maxStamina = 200;

    private NavMeshAgent agent;
    private PlayerAngleDetector angleDetector;
    private PlayerCollision playerCollision;
    private PlayerGodMode godMode;
    private PlayerAnimator[] animators;

    // 사이드킥 자동 공격
    private CollisionDetector monsterDetector;
    private Transform targetMonster;
    private bool isSidekickAttacking = false;

    [Header("[Change Attck]")]
    [SerializeField]
    private Shoot changeAttackShoot;

    public Vector3 TargetPosition => playerCollision.ColliderPosition;


    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        angleDetector = GetComponent<PlayerAngleDetector>();
        playerCollision = GetComponentInChildren<PlayerCollision>();
        godMode = GetComponent<PlayerGodMode>();
        animators = GetComponentsInChildren<PlayerAnimator>();
        monsterDetector = GetComponentInChildren<CollisionDetector>();

        maxHp = Mathf.Max(maxHp, characterStat.Health);
        currentHp = maxHp;
        maxStaminaCount = Mathf.Max(maxStamina, maxStaminaCount);

        // 변경 공격
        changeAttackShoot.SetCharacterStat(characterStat);
    }

    protected override void Start()
    {
        base.Start();

        monsterDetector.AddCollisionDetectAction((Transform other, string otherTag, DetectType detectType) =>
        {
            if (!targetMonster || !targetMonster.gameObject.activeSelf)
            {
                targetMonster = other.transform.parent.transform;
                return;
            }

            float curMonsterDistance = Vector3.Distance(targetMonster.position, TargetPosition);
            float newMonsterDistance = Vector3.Distance(other.position, TargetPosition);

            if (newMonsterDistance < curMonsterDistance)
                targetMonster = other.transform.parent.transform;
        });

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        UpdatePlayerType(playerType, true);
    }

    private float lastStaminaDown = 0;
    private void Update()
    {
        if (playerType == PlayerType.Main)
        {
            UpdateMainPlayerMovement();
            UpdateMainPlayerAttack();
        }
        else
        {
            UpdateSidekickPlayerMovement();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            List<Coin> coins = ObjectPooler.GetAllPools<Coin>("Coin", true);
            foreach (Coin coin in coins)
                coin.StartTargeting();
        }

        if (playerType == PlayerType.Main && Time.timeScale < 1)
        {
            if (Time.time - lastStaminaDown > 0.3f)
            {
                currentStaminaCount -= 10;
                lastStaminaDown = Time.time;

                if (currentStaminaCount <= 0)
                {
                    Time.timeScale = 1;
                    currentStaminaCount = 0;
                }
            }
        }

        if (playerType == PlayerType.Main && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Time.timeScale = 0.4f;
        }
        else if (playerType == PlayerType.Main && Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1;
        }
    }

    private void UpdateMainPlayerMovement()
    {
        if (knockBack.IsKnockBacking) return;

        if (!movement.enabled)
            movement.enabled = true;

        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (Vector3.right * hor) + (Vector3.up * ver);

        movement.SetMoveDirection(moveDir);

        // 애니메이션
        foreach (PlayerAnimator animator in animators)
            animator.SetState(PlayerAnimator.IsMoving, moveDir.Equals(Vector3.zero) ? false : true);
    }

    private void UpdateMainPlayerAttack()
    {
        if (!knockBack.IsKnockBacking && !weapon.IsTrigger && Input.GetMouseButton(0))
        {
            weapon.StartTrigger();

            // 애니메이션
            foreach (PlayerAnimator animator in animators)
                animator.SetState(PlayerAnimator.IsAttacking, true);
        }
        else if ((weapon.IsTrigger && Input.GetMouseButtonUp(0)) || (weapon.IsTrigger && knockBack.IsKnockBacking))
        {
            weapon.StopTrigger();

            // 애니메이션
            foreach (PlayerAnimator animator in animators)
                animator.SetState(PlayerAnimator.IsAttacking, false);
        }
    }

    private void UpdateSidekickPlayerMovement()
    {
        if (!agent || !Player.main) return;

        // 넉백 중        
        if (knockBack.IsKnockBacking)
        {
            if (!agent.isStopped)
                agent.isStopped = true;
            if (!movement.enabled)
                movement.enabled = true;
            return;
        }
        // 넉백 중 아님
        else
        {
            if (agent.isStopped)
                agent.isStopped = false;
            if (movement.enabled)
                movement.enabled = false;
        }

        agent.speed = characterStat.MoveSpeed;
        agent.SetDestination(Player.main.transform.position);

        if (isSidekickAttacking) return;

        // 애니메이션
        foreach (PlayerAnimator animator in animators)
            animator.SetState(PlayerAnimator.IsMoving, agent.velocity.Equals(Vector3.zero) ? false : true);

        if (!agent.velocity.Equals(Vector3.zero))
            angleDetector.SetAngleIndexByDirection(agent.velocity);
    }

    private void StartSidekickRoutine()
    {
        StartCoroutine(UPDATE_SIDEKICK_PLAYER_ATTACK_ROUTINE);
    }

    private void StopSidekickRoutine()
    {
        StopCoroutine(UPDATE_SIDEKICK_PLAYER_ATTACK_ROUTINE);
        isSidekickAttacking = false;
        weapon.StopTrigger();
    }

    private static readonly string UPDATE_SIDEKICK_PLAYER_ATTACK_ROUTINE = "UpdateSidekickPlayerAttackRoutine";
    private IEnumerator UpdateSidekickPlayerAttackRoutine()
    {
        float timer = 0;
        float attackDelayTime = 5;
        float attackTime = 3;
        float minMainPlayerDistance = 4f;

        while (true)
        {
            if (knockBack.IsKnockBacking)
                timer = 0;
            timer += Time.deltaTime;

            if (timer >= attackDelayTime
                && targetMonster && targetMonster.gameObject.activeSelf
                && Vector2.Distance(TargetPosition, Player.main.TargetPosition) <= minMainPlayerDistance)
            {
                // 공격 시작
                isSidekickAttacking = true;
                timer = 0;
                foreach (PlayerAnimator animator in animators)
                    animator.SetState(PlayerAnimator.IsAttacking, true);

                // 공격 중
                while (timer < attackTime
                        && targetMonster && targetMonster.gameObject.activeSelf
                        && Vector2.Distance(TargetPosition, Player.main.TargetPosition) <= minMainPlayerDistance)
                {
                    timer += Time.deltaTime;

                    // 몬스터 방향 바라보기
                    angleDetector.SetAngleIndexByDirection(targetMonster.position - TargetPosition);

                    // 공격
                    weapon.StartTrigger();

                    // 부딪히면 공격 종료
                    if (knockBack.IsKnockBacking)
                        break;

                    yield return null;
                }

                // 공격 끝
                isSidekickAttacking = false;
                timer = 0;
                foreach (PlayerAnimator animator in animators)
                    animator.SetState(PlayerAnimator.IsAttacking, false);
                weapon.StopTrigger();
                print("START ATTAK");
            }

            yield return null;
        }
    }

    public override void Hit(float attack, float knockBack, Vector3 hitPosition)
    {
        if (godMode.IsGodMode) return;

        if (playerType == PlayerType.Main)
            IncreaseHp(-attack);

        godMode.StartGodMode();

        KnockBack(hitPosition, knockBack);

        // 애니메이션
        foreach (PlayerAnimator animator in animators)
            animator.Hit(hitPosition.x > TargetPosition.x ? PlayerAnimator.HitLeft : PlayerAnimator.HitRight);
    }

    public void UpdatePlayerType(PlayerType playerType, bool init = false)
    {
        // 스태틱 변수 main 지정
        if (playerType == PlayerType.Main) main = this;

        // 자신의 플레이어 타입 저장
        this.playerType = playerType;

        // 컴포넌트 활성화/비활성화
        movement.SetMoveDirection(Vector2.zero);
        agent.enabled = playerType == PlayerType.Sidekick;
        if (playerType == PlayerType.Main)
            angleDetector.StartMouseTargeting();
        else
            angleDetector.StopMouseTargeting();

        // 사이드킥이 되었을 때, 공격 중지
        if (playerType == PlayerType.Sidekick && weapon.IsTrigger)
        {
            weapon.StopTrigger();

            // 애니메이션
            foreach (PlayerAnimator animator in animators)
                animator.SetState(PlayerAnimator.IsAttacking, false);
        }

        // 사이드킥 공격 루틴 시작 또는 정지
        if (playerType == PlayerType.Sidekick)
            StartSidekickRoutine();
        else
            StopSidekickRoutine();

        if (!init && playerType == PlayerType.Main)
        {
            print("asdasdsa");
            changeAttackShoot.StartShoot();
        }
    }

    public static void IncreaseCurrentHp(float amount)
    {
        currentHp += (int)amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }

    public static void IncreaseMaxHp(float amount)
    {
        maxHp += (int)amount;
    }

    public override void IncreaseHp(float amount)
    {
        if (playerType == PlayerType.Sidekick) return;

        IncreaseCurrentHp(amount);
        if (currentHp <= 0) OnDead();
    }

    public static void IncreaseCoinCount(int amount)
    {
        currentCointCount += amount;
    }

    public static void IncreaseStaminaCount(int amount)
    {
        currentStaminaCount += amount;
        currentStaminaCount = Mathf.Clamp(currentStaminaCount, 0, maxStaminaCount);
    }

    protected override void OnDead() { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerSex { Man, Woman }
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
    [SerializeField]
    private PlayerSex playerSex;
    [SerializeField]
    private ParticleSystem focusModeParticle;

    [Header("[Change Attck]")]
    [SerializeField]
    private Shoot changeAttackShoot;

    [Header("[Stamina]")]
    [SerializeField]
    private int maxStamina = 200;

    private NavMeshAgent agent;
    private PlayerAngleDetector angleDetector;
    private PlayerCollision playerCollision;
    private PlayerGodMode godMode;
    private PlayerAnimator[] animators;
    [Header("[Component]")]
    [SerializeField]
    private PlayerAnimator bottomAnimator;

    // 사이드킥 자동 공격
    private CollisionDetector monsterDetector;
    private Transform targetMonster;
    private bool isSidekickAttacking = false;

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

        // 집중 모드 정지
        StopFocusMode();
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

    private void Update()
    {
        if (playerType == PlayerType.Main)
        {
            UpdateMainPlayerMovement();             // 메인 플레이어 이동
            UpdateMainPlayerAttack();               // 메인 플레이어 공격
            UpdateMainPlayerFocusMode();            // 메인 플레이어 집중 모드
        }
        else
        {
            UpdateSidekickPlayerMovement();         // 사이드킥 플레이어 이동
        }

        // 뒤로 뛰는지 확인 -> 속도 변경
        CheckRunnigBackward();
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

    private float lastStaminaDown = 0;
    private float focusMoveSpeedPercent = 1;
    private bool isFocusMode = false;
    private void UpdateMainPlayerFocusMode()
    {
        if (playerType != PlayerType.Main) return;

        if (isFocusMode)
        {
            if (Time.time - lastStaminaDown > 0.3f)
            {
                currentStaminaCount -= 10;
                lastStaminaDown = Time.time;

                if (currentStaminaCount <= 0)
                {
                    StopFocusMode();
                    currentStaminaCount = 0;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartFocusMode();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopFocusMode();
        }
    }

    private void StartFocusMode()
    {
        if (playerType != PlayerType.Main) return;
        if (currentStaminaCount < 10) return;

        isFocusMode = true;

        if (playerSex == PlayerSex.Man)
        {
            focusMoveSpeedPercent = 1.5f;
            var emission = focusModeParticle.emission;
            emission.rateOverDistance = 3;
        }
        else
        {
            Time.timeScale = 0.4f;
            focusModeParticle.gameObject.SetActive(true);
        }
    }

    private void StopFocusMode()
    {
        isFocusMode = false;
        Time.timeScale = 1;
        focusMoveSpeedPercent = 1;

        if (playerSex == PlayerSex.Man)
        {
            var emission = focusModeParticle.emission;
            emission.rateOverDistance = 0;
        }
        else
        {
            focusModeParticle.gameObject.SetActive(false);
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

    private void CheckRunnigBackward()
    {
        // 실제 보고 있는 방향
        PlayerAngle directionAngle = angleDetector.PlayerAngle;

        // 걷는 방향
        PlayerAngle moveAngle;
        if (playerType == PlayerType.Main)
            moveAngle = PlayerAngleDetector.DirectionToPlayerAngle(movement.MoveDirection);
        else
            moveAngle = PlayerAngleDetector.DirectionToPlayerAngle(agent.velocity);

        // 방향 차이
        int diff = Mathf.Min((moveAngle - directionAngle + 8) % 8, (directionAngle - moveAngle + 8) % 8);

        // 속도 퍼센트
        float speedPercent = diff >= 3 ? 0.6f : 1f;
        speedPercent *= focusMoveSpeedPercent;  // 집중 모드 속도 퍼센트 곱

        // 이동 속도 변경
        movement.SetMoveSpeedPercent(speedPercent);

        // 애니메이션 속도 변경
        bottomAnimator.SetAnimationSpeedPercent(speedPercent);
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

        // 플레이어 변경 공격
        if (!init && playerType == PlayerType.Main)
        {
            changeAttackShoot.StartShoot();
        }

        // 집중 모드 정지
        StopFocusMode();
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

    public void MoveTo(Vector3 position)
    {
        agent.enabled = false;
        transform.position = position;

        if (playerType == PlayerType.Sidekick)
            agent.enabled = true;
    }
}

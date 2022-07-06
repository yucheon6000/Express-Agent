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
    private static int currentRubyCount = 0;
    public static int CurrentRubyCount => currentRubyCount;
    private static int maxStaminaCount = 0;
    public static int MaxStaminaCount => maxStaminaCount;
    private static bool cantAttack = false;
    public static bool CantAttack(bool cant) => cantAttack = cant;

    public static Player Main => main;

    private static List<Player> players = new List<Player>();
    public static List<Player> Players => new List<Player>(players);

    [Header("[Player]")]
    [SerializeField]
    private PlayerType playerType;      // 플레이어 타입
    [SerializeField]
    private PlayerSex playerSex;
    [SerializeField]
    private GhostTrail ghostTrail;
    [SerializeField]
    private ParticleSystem focusModeParticle;

    [Header("[Change Attck]")]
    [SerializeField]
    private Shoot changeAttackShoot;

    [Header("[Stamina]")]
    [SerializeField]
    private int maxStamina = 100;

    private NavMeshAgent agent;
    private PlayerAngleDetector angleDetector;
    private PlayerCollision playerCollision;
    private PlayerGodMode godMode;
    private PlayerAnimator animator;
    [Header("[Component]")]
    [SerializeField]
    private PlayerAnimator bottomAnimator;

    // 사이드킥 자동 공격
    [Header("[Sidekick Auto Attack]")]
    [SerializeField]
    private float sidekickAttackDelayTime = 5;
    [SerializeField]
    private float sidekickAttackTime = 3;
    [SerializeField]
    private float sidekickMinMainPlayerDistance = 4;
    private CollisionDetector monsterDetector;
    private Transform targetMonster;
    private bool isSidekickAttacking = false;

    public override Vector3 TargetPosition => playerCollision.ColliderPosition;

    [Header("[Audio]")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip hitAudioClip;
    [SerializeField]
    private AudioClip dieAudioClip;
    [SerializeField]
    private AudioClip changeAudioClip;
    [SerializeField]
    private AudioClip powerUpAudioClip;
    [SerializeField]
    private AudioSource runAudioSource;

    [Header("[UI]")]
    [SerializeField]
    private PlayerAttackListDisplayer playerAttackListDisplayer;
    [SerializeField]
    private GameObject playerPortrait;
    [SerializeField]
    private GameObject GameOverUI;
    [SerializeField]
    private GameObject canvasGameOver;

    [Header("@TEST")]
    [SerializeField]
    private bool mouseDirectionMode = false;

    protected override void Awake()
    {
        /* Init Static Variables */
        currentHp = 0;
        maxHp = 0;
        currentCointCount = 0;
        currentRubyCount = PlayerPrefs.GetInt("rubyCount", 0);
        currentStaminaCount = 0;
        maxStaminaCount = 0;
        cantAttack = false;
        if (players.Count == 2)
            players = new List<Player>();
        /*************************/

        base.Awake();

        players.Add(this);

        agent = GetComponent<NavMeshAgent>();
        angleDetector = GetComponent<PlayerAngleDetector>();
        playerCollision = GetComponentInChildren<PlayerCollision>();
        godMode = GetComponent<PlayerGodMode>();
        animator = GetComponent<PlayerAnimator>();
        monsterDetector = GetComponentInChildren<CollisionDetector>();

        maxHp = Mathf.Max(maxHp, characterStat.Health);
        currentHp = maxHp;
        maxStaminaCount = Mathf.Max(maxStamina, maxStaminaCount);
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

        // 변경 공격
        changeAttackShoot.SetCharacterStat(characterStat);

        // 집중 모드 정지
        StopFocusMode();
    }

    private void Update()
    {
        if (isDead) return;

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
        if (knockBack.IsKnockBacking)
        {
            if (runAudioSource.enabled)
                runAudioSource.enabled = false;

            return;
        }

        if (!movement.enabled)
            movement.enabled = true;

        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (Vector3.right * hor) + (Vector3.up * ver);

        movement.SetMoveDirection(moveDir);

        // 애니메이션
        animator.SetState(PlayerAnimator.IsMoving, moveDir.Equals(Vector3.zero) ? false : true);

        // 달리는 소리 재생
        runAudioSource.enabled = !moveDir.Equals(Vector3.zero);
    }

    private void UpdateMainPlayerAttack()
    {
        // 공격 시작
        if (!knockBack.IsKnockBacking && !weapon.IsTrigger && Input.GetMouseButton(0) && !cantAttack)
        {
            weapon.StartTrigger();

            // 애니메이션
            animator.SetState(PlayerAnimator.IsAttacking, true);
        }

        // 공격 중지
        else if (weapon.IsTrigger && (Input.GetMouseButtonUp(0) || knockBack.IsKnockBacking || cantAttack))
        {
            weapon.StopTrigger();

            // 애니메이션
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
            // var emission = focusModeParticle.emission;
            // emission.rateOverDistance = 3;
            ghostTrail.enabled = true;
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
            // var emission = focusModeParticle.emission;
            // emission.rateOverDistance = 0;
            ghostTrail.enabled = false;
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
            if (runAudioSource.enabled)
                runAudioSource.enabled = false;
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

        while (true)
        {
            if (knockBack.IsKnockBacking)
                timer = 0;
            timer += Time.deltaTime;

            if (timer >= sidekickAttackDelayTime
                && targetMonster && targetMonster.gameObject.activeSelf
                && Vector2.Distance(TargetPosition, Player.main.TargetPosition) <= sidekickMinMainPlayerDistance)
            {
                // 공격 시작
                isSidekickAttacking = true;
                timer = 0;
                animator.SetState(PlayerAnimator.IsAttacking, true);

                // 공격 중
                while (timer < sidekickAttackTime
                        && targetMonster && targetMonster.gameObject.activeSelf
                        && Vector2.Distance(TargetPosition, Player.main.TargetPosition) <= sidekickMinMainPlayerDistance)
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
                animator.SetState(PlayerAnimator.IsAttacking, false);
                weapon.StopTrigger();
                print("START ATTAK");
            }

            yield return null;
        }
    }

    public override void Hit(float attack, float knockBack, Vector3 hitPosition)
    {
        if (isDead) return;
        if (godMode.IsGodMode) return;

        if (playerType == PlayerType.Main)
            IncreaseHp(-attack);

        if (isDead) return;

        godMode.StartGodMode();

        KnockBack(hitPosition, knockBack);

        // 애니메이션 및 사운드
        animator.Hit(hitPosition.x > TargetPosition.x ? PlayerAnimator.HitLeft : PlayerAnimator.HitRight);
        if (playerType == PlayerType.Main)
        {
            audioSource.PlayOneShot(hitAudioClip);
            TargetCamera.Shake(0.1f, 0.05f);
        }
    }

    public void UpdatePlayerType(PlayerType playerType, bool init = false)
    {
        if (isDead) return;

        bool isMainPlayer = playerType == PlayerType.Main;
        bool isSidekickPlayer = playerType == PlayerType.Sidekick;

        // 스태틱 변수 main 지정
        if (isMainPlayer) main = this;

        // 자신의 플레이어 타입 저장
        this.playerType = playerType;

        // 컴포넌트 활성화/비활성화
        movement.SetMoveDirection(Vector2.zero);
        agent.enabled = isSidekickPlayer;
        if (isMainPlayer)
            angleDetector.StartMouseTargeting();
        else
            angleDetector.StopMouseTargeting();

        // 사이드킥이 되었을 때, 공격 중지
        if (isSidekickPlayer && weapon.IsTrigger)
        {
            weapon.StopTrigger();

            // 애니메이션
            animator.SetState(PlayerAnimator.IsAttacking, false);
        }

        // 사이드킥 공격 루틴 시작 또는 정지
        if (isSidekickPlayer)
            StartSidekickRoutine();
        else
            StopSidekickRoutine();

        // 플레이어 변경 공격
        if (!init && isMainPlayer)
        {
            changeAttackShoot.StartShoot();
        }

        // 집중 모드 정지
        StopFocusMode();

        // UI 업데이트
        playerAttackListDisplayer?.Display(isMainPlayer);
        playerPortrait?.SetActive(isMainPlayer);

        // 마우스 추적 모드
        if (mouseDirectionMode)
            weapon.ActiveMouseDirectionMode(isMainPlayer);

        // 사이드킥 플레이어 공격 레벨 0으로 변경
        // 메인 플레이어 공격 레벨 정상화
        weapon.SetMinimumMode(isSidekickPlayer);

        // 애니메이션
        animator.Change();

        // 사운드
        if (isMainPlayer && !init)
            audioSource.PlayOneShot(changeAudioClip);
        runAudioSource.enabled = false;
    }

    public static void IncreaseCurrentHp(float amount)
    {
        currentHp += (int)amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        // 죽음 처리
        if (currentHp <= 0)
            foreach (var player in players)
                player.OnDead();
    }

    public static void IncreaseMaxHp(float amount)
    {
        maxHp += (int)amount;
    }

    public override void IncreaseHp(float amount)
    {
        // 체력 감소
        if (playerType == PlayerType.Main)
            IncreaseCurrentHp(amount);
    }

    public static void IncreaseCoinCount(int amount)
    {
        currentCointCount += amount;
    }

    public static void IncreaseRubyCount(int amount)
    {
        currentRubyCount = currentRubyCount + amount;

        if (currentRubyCount < 0) currentRubyCount = 0;

        PlayerPrefs.SetInt("rubyCount", currentRubyCount);
    }

    public static void IncreaseStaminaCount(int amount)
    {
        currentStaminaCount += amount;
        currentStaminaCount = Mathf.Clamp(currentStaminaCount, 0, maxStaminaCount);
    }

    protected override void OnDead()
    {
        print(gameObject.name);
        if (isDead) return;
        isDead = true;
        print(gameObject.name);

        // 멈춤
        movement.MoveSpeedType = MoveSpeedType.Manual;
        movement.SetMoveSpeed(0);
        movement.SetMoveDirection(Vector2.zero);
        knockBack.enabled = false;
        agent.enabled = false;

        // 사이드킥 공격 멈춤
        StopSidekickRoutine();

        // 애니메이션 및 사운드
        animator.Dead();
        runAudioSource.enabled = false;
        if (playerType == PlayerType.Main)
        {
            audioSource.PlayOneShot(dieAudioClip);
            GameOverUI.SetActive(true);
            Invoke("ShowCanvasGameOver", 2);
        }
    }

    private void ShowCanvasGameOver()
    {
        GameOverUI.SetActive(false);
        canvasGameOver.SetActive(true);
    }

    public void MoveTo(Vector3 position)
    {
        agent.enabled = false;
        transform.position = position;

        if (playerType == PlayerType.Sidekick)
            agent.enabled = true;
    }

    public void PowerUp()
    {
        animator.PowerUp();
        audioSource.PlayOneShot(powerUpAudioClip);
    }
}

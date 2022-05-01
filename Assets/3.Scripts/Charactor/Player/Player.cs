using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerType { Main, Sidekick }

public class Player : Character
{
    private static Player main;
    private static int currentCointCount = 0;
    private static int currentStaminaCount = 0;

    public static Player Main => main;

    [Header("[Player]")]
    [SerializeField]
    private PlayerType playerType;      // 플레이어 타입

    private NavMeshAgent agent;
    private PlayerAngleDetector angleDetector;
    private PlayerCollision playerCollision;

    public Vector3 TargetPosition => playerCollision.ColliderPosition;

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        angleDetector = GetComponent<PlayerAngleDetector>();
        playerCollision = GetComponentInChildren<PlayerCollision>();
    }

    protected override void Start()
    {
        base.Start();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        UpdatePlayerType(playerType);
    }

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
    }

    private void UpdateMainPlayerAttack()
    {
        if (!weapon.IsTrigger && Input.GetMouseButton(0))
        {
            weapon.StartTrigger();
        }
        else if (weapon.IsTrigger && Input.GetMouseButtonUp(0))
        {
            weapon.StopTrigger();
        }
    }

    private void UpdateSidekickPlayerMovement()
    {
        if (!agent || !Player.main) return;

        if (movement.enabled)
            movement.enabled = false;

        // 넉백 중        
        if (knockBack.IsKnockBacking)
        {
            if (!agent.isStopped) agent.isStopped = true;
            return;
        }

        // 넉백 중 아님
        if (!knockBack.IsKnockBacking)
        {
            if (agent.isStopped)
                agent.isStopped = false;
        }

        agent.speed = characterStat.MoveSpeed;
        agent.SetDestination(Player.main.transform.position);
    }

    public void UpdatePlayerType(PlayerType playerType)
    {
        // 스태틱 변수 main 지정
        if (playerType == PlayerType.Main) main = this;

        // 자신의 플레이어 타입 저장
        this.playerType = playerType;

        // 컴포넌트 활성화/비활성화
        movement.SetMoveDirection(Vector2.zero);
        agent.enabled = playerType == PlayerType.Sidekick;
        angleDetector.enabled = playerType == PlayerType.Main;

        // 사이드킥이 되었을 때, 공격 중지
        if (playerType == PlayerType.Sidekick && weapon.IsTrigger)
            weapon.StopTrigger();
    }

    protected override void OnDead() { }

    public static void IncreaseCoinCount(int amount)
    {
        currentCointCount += amount;
    }

    public static void IncreaseStaminaCount(int amount)
    {
        currentStaminaCount += amount;
    }
}

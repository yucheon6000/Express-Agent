using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerType { Main, Sidekick }

public class Player : Character
{

    [Header("[Player]")]
    [SerializeField]
    private PlayerType playerType;      // 플레이어 타입
    [SerializeField]
    private Player mainPlayer;          // 메인 플레이어

    private NavMeshAgent agent;
    private PlayerAngleDetector angleDetector;

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        angleDetector = GetComponent<PlayerAngleDetector>();
    }

    protected override void Start()
    {
        base.Start();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        UpdatePlayerType(playerType, mainPlayer);

        weapon.StartTrigger();
    }

    private void Update()
    {
        if (playerType == PlayerType.Main)
        {
            UpdateMainPlayerMovement();
        }
        else
        {
            UpdateSidekickPlayerMovement();
        }
    }

    private void UpdateMainPlayerMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (Vector3.right * hor) + (Vector3.up * ver);

        movement.SetMoveDirection(moveDir);
    }

    private void UpdateSidekickPlayerMovement()
    {
        if (!agent || !mainPlayer) return;

        agent.speed = characterStat.MoveSpeed;
        agent.SetDestination(mainPlayer.transform.position);
    }

    public void UpdatePlayerType(PlayerType playerType, Player mainPlayer)
    {
        this.playerType = playerType;
        this.mainPlayer = mainPlayer;

        movement.SetMoveDirection(Vector2.zero);
        agent.enabled = playerType == PlayerType.Sidekick;
        angleDetector.enabled = playerType == PlayerType.Main;
    }

    protected override void OnDead() { }
}

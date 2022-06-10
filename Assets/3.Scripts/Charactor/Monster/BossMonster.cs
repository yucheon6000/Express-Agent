using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IOnEndAnimation
{
    public void OnEndAnimation();
}

public class BossMonster : Monster, IOnEndAnimation
{
    private enum BossState
    {
        Breath = 0, Run,
        Attack1 = 100, Attack2, Attack3, Attack4,
        Death = 200
    }

    private BossState bossState = BossState.Breath;

    private MonsterSpawnController monsterSpawnController;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Animator animator;

    [Header("[Property]")]
    [SerializeField]
    private float minBreathTime;
    [SerializeField]
    private float maxBreathTime;
    [SerializeField]
    private float minRunTime;
    [SerializeField]
    private float maxRunTime;

    [Header("[Weapon]")]
    [Header("Attack1")]
    [SerializeField]
    private Attack attack1;
    [SerializeField]
    private float minAttack1Time;
    [SerializeField]
    private float maxAttack1Time;
    [Header("Attack2")]
    [SerializeField]
    private Shoot attack2;
    [Header("Attack3")]
    [SerializeField]
    private MonsterSpawnControlInfo attack3;
    [Header("Attack4")]
    [SerializeField]
    private GameObject attack4Bullet;
    [SerializeField]
    private int attack4Count;
    [SerializeField]
    private float attack4DeltaTime;

    protected override void Awake()
    {
        base.Awake();
        monsterSpawnController = GameObject.FindObjectOfType<MonsterSpawnController>();
    }

    protected override void Start()
    {
        base.Start();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        attack1.SetCharacterStat(CharacterStat);
        attack2.SetCharacterStat(CharacterStat);
        // attack4.SetCharacterStat(CharacterStat);

        ChangeBossState(bossState, forcedChange: true);
    }

    private void ChangeBossState(BossState bossState, bool forcedChange = false)
    {
        if (this.bossState == BossState.Death) return;
        if (this.bossState == bossState && !forcedChange) return;

        endAnimation = false;

        StopCoroutine($"{this.bossState.ToString()}Routine");
        this.bossState = bossState;
        StartCoroutine($"{this.bossState.ToString()}Routine");
    }

    bool endAnimation = false;
    public void OnEndAnimation()
    {
        endAnimation = true;
    }

    private IEnumerator BreathRoutine()
    {
        animator.Play("Breath", -1);

        yield return new WaitForSeconds(Random.Range(minBreathTime, maxBreathTime));

        ChangeBossState(BossState.Run);
    }

    private IEnumerator RunRoutine()
    {
        animator.Play("Run", -1);

        // 추적 시작
        float runTime = Random.Range(minRunTime, maxRunTime);
        float timer = 0;
        while (timer / runTime < 1)
        {
            timer += Time.deltaTime;

            agent.SetDestination(Player.Main.TargetPosition);

            yield return null;
        }

        // 추적 종료
        agent.ResetPath();

        // 공격 시작
        ChangeBossState((BossState)Random.Range(100, 104));
    }

    private IEnumerator Attack1Routine()
    {
        animator.Play("Attack1", -1);

        attack1.StartAttack();
        yield return new WaitForSeconds(Random.Range(minAttack1Time, maxAttack1Time));
        attack1.StopAttack();

        ChangeBossState((BossState)Random.Range(0, 2));
    }

    private IEnumerator Attack2Routine()
    {
        animator.Play("Attack2", -1);

        while (!endAnimation)
            yield return null;

        attack2.StartShoot();
        attack2.StopShoot();

        ChangeBossState(BossState.Breath);
    }

    private IEnumerator Attack3Routine()
    {
        animator.Play("Attack3", -1);

        while (!endAnimation)
            yield return null;

        // 몬스터 소환
        if (monsterSpawnTrigger)
        {
            attack3.targetTilePositions = monsterSpawnTrigger.TargetTilePositions;
            monsterSpawnController.Spawn(attack3);
        }

        ChangeBossState(BossState.Breath);
    }

    private IEnumerator Attack4Routine()
    {
        animator.Play("Attack4", -1);

        for (int i = 0; i < attack4Count; i++)
        {
            Bullet bullet = ObjectPooler.SpawnFromPool<Bullet>(attack4Bullet.name, Player.Main.TargetPosition);
            bullet.Init(new BulletInitInfo(this.transform, Vector3.zero, characterStat, 0));
            yield return new WaitForSeconds(attack4DeltaTime);
        }

        ChangeBossState((BossState)Random.Range(0, 2));
    }

    protected override void OnDead()
    {
        if (isDead) return;
        base.OnDead();

        agent.ResetPath();
        agent.enabled = false;

        movement.SetMoveDirection(Vector3.zero);
        movement.MoveSpeedType = MoveSpeedType.Manual;
        movement.SetMoveSpeed(0);

        characterStat.KnockBackResistance = 1;

        ChangeBossState(BossState.Death);
    }


    private IEnumerator DeathRoutine()
    {
        animator.Play("Death", -1);
        yield break;
    }
}

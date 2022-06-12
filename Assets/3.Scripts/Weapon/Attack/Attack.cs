using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour, NeedCharacterStat
{
    private CharacterStat characterStat = CharacterStat.Default;

    [Header("@Debug")]
    [SerializeField]
    private bool debugMode = false;

    [Header("[Attack Stat]")]
    [SerializeField]
    private AttackStat[] attackStats;
    private AttackStat attackStat;
    private Stepper<AttackStat> attackStepper;
    [SerializeField]
    private int attackLevel = 0;
    private bool isMinimumMode = false;
    public bool IsMinimumMode => isMinimumMode;

    [Header("[Shoot]")]
    [SerializeField]
    private Shoot shoot;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    private bool attacked = false;              // 실제로 공격을 했는지 확인하는 변수
    private float lastAttackTime;               // 마지막으로 공격이 끝난 시점 (장전을 시작한 시점)

    [Header("[UI]")]
    [SerializeField]
    private Image imageAttackGage;

    private Coroutine coroutine;

    private void Awake()
    {
        attackStepper = new Stepper<AttackStat>(attackStats);
        UpdateAttackStat();
        lastAttackTime = -attackStat.ShootDeltaTime;
    }

    [ContextMenu("Start Attack")]
    public void StartAttack()
    {
        if (isAttacking) return;

        UpdateAttackStat();
        attacked = false;

        float leftTime = attackStat.ShootDeltaTime - (Time.time - lastAttackTime);

        isAttacking = true;
        coroutine = StartCoroutine(AttackRoutine(leftTime));
    }

    [ContextMenu("Stop Attack")]
    public void StopAttack()
    {
        if (!isAttacking) return;
        if (coroutine == null) return;

        isAttacking = false;
        shoot.StopShoot();
        StopCoroutine(coroutine);
        coroutine = null;

        // 실제로 공격 안했으면 리턴
        if (!attacked) return;
        // 이미 지금 장전 중이면 리턴
        if (attackStat.ShootDeltaTime - (Time.time - lastAttackTime) > 0) return;

        // 실제로 공격했으면 마지막 공격 시간 저장 및 게이징
        lastAttackTime = Time.time;
        StartCoroutine(FillAttackGageUI(attackStat.ShootDeltaTime));
    }

    private IEnumerator AttackRoutine(float delay)
    {
        if (!shoot) yield break;

        bool firstShoot = true;
        bool waited = false;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        // Shoot 처음 쉬는 시간
        if (attackStat.ShootDelayTimeAtStart > 0)
        {
            StartCoroutine(FillAttackGageUI(attackStat.ShootDelayTimeAtStart));
            yield return new WaitForSeconds(attackStat.ShootDelayTimeAtStart);
        }

        while (true)
        {
            // Shoot이 진행 중이면 한 프레임 쉼
            if (shoot.IsShooting)
            {
                yield return null;
                waited = true;
                continue;
            }

            // Shoot 사이 시간
            if (attackStat.ShootDeltaTime > 0 && firstShoot == false)
            {
                lastAttackTime = Time.time;
                StartCoroutine(FillAttackGageUI(attackStat.ShootDeltaTime));
                yield return new WaitForSeconds(attackStat.ShootDeltaTime);
            }

            // 해당 분기가 없으면 시작과 동시에 끝나는 Shoot일 경우, 무한루프 문제 발생
            // 최소 한 프레임은 쉬어줘야 함
            else if (attackStat.ShootDeltaTime == 0 && waited == false && firstShoot == false)
            {
                yield return null;
                waited = true;
            }

            attacked = true;
            shoot.ActiveMouseDirectionMode(mouseDirectionMode);
            shoot.StartShoot();
            firstShoot = false;
            waited = false;
        }
    }

    private IEnumerator FillAttackGageUI(float time)
    {
        if (!imageAttackGage) yield break;

        float timer = 0;

        while (timer < time)
        {
            timer += Time.deltaTime;

            if (imageAttackGage)
                imageAttackGage.fillAmount = timer / time;

            yield return null;
        };

        imageAttackGage.fillAmount = 1;
    }

    public void SetCharacterStat(CharacterStat characterStat)
    {
        if (this.characterStat == characterStat) return;

        this.characterStat = characterStat;
        attackStat.SetCharacterStat(characterStat);

        if (shoot)
            shoot.SetCharacterStat(characterStat);
    }

    public int GetAttackLevel() => attackLevel;

    public void IncreaseAttackLevel(int level)
    {
        SetAttackLevel(GetAttackLevel() + level);
    }

    public void SetAttackLevel(int level)
    {
        attackLevel = level;
        UpdateAttackStat();

        if (shoot)
            shoot.SetAttackLevel(attackLevel);
    }

    private void UpdateAttackStat()
    {
        if (debugMode)
        {
            shoot.SetDebugMode(true);
            attackStepper = new Stepper<AttackStat>(attackStats);
        }

        attackStat = attackStepper.GetStep(attackLevel);
    }

    protected bool mouseDirectionMode = false;
    public void ActiveMouseDirectionMode(bool enable)
    {
        mouseDirectionMode = enable;
    }

    public void SetMinimumMode(bool value)
    {
        isMinimumMode = value;
        if (isMinimumMode) SetAttackLevel(0);
        else SetAttackLevel(attackLevel);
    }

    public void FillAttackGageUI()
    {
        StartCoroutine(FillAttackGageUI(0));
    }
}

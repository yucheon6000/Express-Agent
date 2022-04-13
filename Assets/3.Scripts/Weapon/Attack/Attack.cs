using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, NeedCharacterStat
{
    private CharacterStat characterStat = CharacterStat.Default;

    [Header("[Attack Stat]")]
    [SerializeField]
    private AttackStat attackStat = AttackStat.Default;

    [Header("[Shoot]")]
    [SerializeField]
    private Shoot shoot;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;
    private Coroutine coroutine;

    [ContextMenu("Start Attack")]
    public void StartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        coroutine = StartCoroutine(AttackRoutine());
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
    }

    private IEnumerator AttackRoutine()
    {
        bool firstShoot = true;

        yield return new WaitForSeconds(attackStat.ShootDelayTimeAtStart);

        while (true)
        {
            if (shoot.IsShooting)
            {
                yield return null;
                continue;
            }

            if (attackStat.ShootDeltaTime > 0 && firstShoot == false)
                yield return new WaitForSeconds(attackStat.ShootDeltaTime);

            shoot.StartShoot();
            firstShoot = false;
        }
    }

    public void SetCharacterStat(CharacterStat characterStat)
    {
        if (this.characterStat == characterStat) return;

        this.characterStat = characterStat;
        attackStat.SetCharacterStat(characterStat);
        shoot.SetCharacterStat(characterStat);
    }
}

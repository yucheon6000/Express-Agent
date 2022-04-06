using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("[Shoot]")]
    [SerializeField]
    private Shoot shoot;

    [Header("[Property]")]
    [Tooltip("Shoot 사이 시간")]
    [SerializeField]
    private float shootDeltaTime = 0;
    [SerializeField]
    [Tooltip("시작 시, Shoot 딜레이 시간")]
    private float shootDelayTimeAtStart = 0;

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
        WaitForSeconds wait = new WaitForSeconds(shootDeltaTime);
        bool firstShoot = true;

        yield return new WaitForSeconds(shootDelayTimeAtStart);

        while (true)
        {
            if (shoot.IsShooting)
            {
                yield return null;
                continue;
            }

            if (shootDeltaTime > 0 && firstShoot == false)
                yield return wait;

            shoot.StartShoot();
            firstShoot = false;
        }
    }
}

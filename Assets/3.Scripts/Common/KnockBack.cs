using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KnockBack : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField]
    private Movement movement;

    [Header("[NavMeshAgent]")]
    [SerializeField]
    private NavMeshAgent agent;

    [Header("[Property]")]
    [SerializeField]
    private float knockBackTime = 0.3f;

    private bool isKnockBacking = false;
    public bool IsKnockBacking => isKnockBacking;

    private Coroutine coroutine = null;

    public void StartKnockBack(Vector2 direction, float force)
    {
        if (isKnockBacking) return;
        coroutine = StartCoroutine(KnockBackRoutine(direction.normalized, force));
    }

    public void StopKnockBack()
    {
        if (!isKnockBacking || coroutine == null) return;

        isKnockBacking = false;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator KnockBackRoutine(Vector2 direction, float force)
    {
        isKnockBacking = true;

        float timer = 0;
        float percent = 0;

        while (percent < 0.8f)
        {
            timer += Time.deltaTime;
            percent = timer / knockBackTime;

            float newForce = (force / 2) * (1 + Mathf.Cos(Mathf.Lerp(0, Mathf.PI, percent)));

            if (movement && movement.enabled)
                movement.Move(newForce, direction);

            yield return null;
        }

        isKnockBacking = false;
        coroutine = null;
    }

    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            isKnockBacking = false;
        }
    }
}

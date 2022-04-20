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
    private float knockBackTime = 1;

    public void StartKnockBack(Vector2 direction, float force)
    {
        StartCoroutine(KnockBackRoutine(direction.normalized, force));
    }

    private IEnumerator KnockBackRoutine(Vector2 direction, float force)
    {
        movement.enabled = false;
        agent.isStopped = true;

        float timer = 0;
        while (timer < knockBackTime)
        {
            transform.Translate(direction * force * Time.deltaTime);

            timer += Time.deltaTime;

            yield return null;
        }

        agent.isStopped = false;
        movement.enabled = true;
    }
}

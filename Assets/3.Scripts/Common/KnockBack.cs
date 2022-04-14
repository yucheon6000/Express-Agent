using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField]
    private Movement movement;

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

        float timer = 0;
        while (timer < knockBackTime)
        {
            transform.Translate(direction * force * Time.deltaTime);

            timer += Time.deltaTime;

            yield return null;
        }

        movement.enabled = true;
    }
}

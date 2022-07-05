using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform pivot;
    [SerializeField]
    private Transform target;

    private void Update()
    {
        if (!Player.Main) return;

        Vector2 dir = Player.Main.transform.position - pivot.position;
        dir.Normalize();

        target.position = (Vector2)pivot.position + dir;
    }
}

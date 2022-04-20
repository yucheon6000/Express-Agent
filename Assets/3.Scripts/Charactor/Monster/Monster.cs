using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Monster : Character
{
    [Header("[Monster]")]
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private CollisionDetector detector;

    private Transform target;

    protected override void Start()
    {
        base.Start();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        detector.AddCollisionDetectAction((transform, _, _) =>
        {
            target = transform;
            weapon.StartTrigger();
            detector.SetActive(false);
        });
    }

    private void Update()
    {
        if (!target) return;

        agent.SetDestination(target.transform.position);

        // Vector2 moveDir = target.transform.position - transform.position;

        // if (moveDir.sqrMagnitude <= 1)
        // {
        //     movement.SetMoveDirection(Vector2.zero);
        //     return;
        // }

        // movement.SetMoveDirection(moveDir);
    }

    protected override void OnDead() { }
}
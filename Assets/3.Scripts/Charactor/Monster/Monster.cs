using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Monster : Character
{
    [SerializeField]
    private CollisionDetector detector;

    private Transform target;

    protected override void Start()
    {
        base.Start();

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

        Vector2 moveDir = target.transform.position - transform.position;

        if (moveDir.sqrMagnitude <= 1)
        {
            movement.SetMoveDirection(Vector2.zero);
            return;
        }

        movement.SetMoveDirection(moveDir);
    }

    protected override void OnDead() { }
}
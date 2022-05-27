using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CollisionDetector))]
public class ClosestTargetDetector : MonoBehaviour
{
    public class OnUpdatedClosestTarget : UnityEvent<Transform, float> { }      // 타겟 트랜스폼, 현재 거리
    [HideInInspector]
    public OnUpdatedClosestTarget onUpdatedClosestTarget = new OnUpdatedClosestTarget();

    [SerializeField]
    private CollisionDetector collisionDetector;
    [SerializeField]
    private Transform pivot;

    [Header("@Debug")]
    [SerializeField]
    private Transform target;
    public Transform Target => target;

    private void Awake()
    {
        collisionDetector = GetComponent<CollisionDetector>();
        if (!pivot) pivot = collisionDetector.transform;
    }

    private void Start()
    {
        collisionDetector.AddCollisionDetectAction(CheckClosestTarget);
    }

    private void CheckClosestTarget(Transform targetTransform, string tag, DetectType detectType)
    {
        if (!target && detectType != DetectType.Exit)
        {
            target = targetTransform;
            return;
        }

        if (target && detectType != DetectType.Exit)
        {
            float curDist = (pivot.position - target.position).sqrMagnitude;
            float newDist = (pivot.position - targetTransform.position).sqrMagnitude;

            if (newDist < curDist)
            {
                target = targetTransform;
                onUpdatedClosestTarget.Invoke(target, newDist);
            }
        }

        if (target && detectType == DetectType.Exit && target == targetTransform)
        {
            target = null;
            onUpdatedClosestTarget.Invoke(target, 0);
            return;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveDirection = Vector2.right;  // 이동 방향

    [Header("[Property]")]
    [SerializeField]
    [Tooltip("이동 속도")]
    private float moveSpeed;        // 이동 속도
    [SerializeField]
    [Tooltip("이동할 거리 (0은 무제한)")]
    private float moveDistance;     // 이동할 거리 (0일 경우 무제한)
    private float movedDistance;    // 이동한 거리

    public void Init(Vector2 spawnPosition, Vector2 moveDirection)
    {
        movedDistance = 0;
        transform.position = spawnPosition;
        this.moveDirection = moveDirection.normalized;
    }

    private void Update()
    {
        if (movedDistance >= moveDistance && moveDistance > 0) return;

        float moveAmount = moveSpeed * Time.deltaTime;

        if (moveDistance > 0)
        {
            movedDistance += moveAmount;
            if (movedDistance >= moveDistance)
            {
                moveAmount -= movedDistance - moveDistance;
            }
        }

        transform.Translate(moveDirection * moveAmount);
    }
}

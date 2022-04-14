using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float moveSpeed = 0;
    private Vector2 moveDirection = Vector2.zero;
    private StatHasMoveSpeed stat = null;

    private float MoveSpeed => (stat == null) ? moveSpeed : stat.GetMoveSpeed();

    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        this.moveDirection = moveDirection.normalized;
    }

    public void SetStat(StatHasMoveSpeed stat)
    {
        this.stat = stat;
    }

    private void Update()
    {
        transform.Translate(moveDirection * MoveSpeed * Time.deltaTime);
    }
}

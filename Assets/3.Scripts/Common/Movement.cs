using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveSpeedType { StatBased, Manual }

public class Movement : MonoBehaviour
{

    [Header("[ObstacleDetector (Nullable)]")]
    [SerializeField]
    private ObstacleDetector detector;

    public MoveSpeedType MoveSpeedType { get; set; }

    private float moveSpeed = 0;
    public float MoveSpeed =>
        (stat == null || MoveSpeedType == MoveSpeedType.Manual)
        ? moveSpeed
        : stat.GetMoveSpeed();

    private Vector2 moveDirection = Vector2.zero;
    public Vector2 MoveDirection => moveDirection;

    private Vector3 movedForce = Vector2.zero;
    public Vector3 MovedForce => movedForce;

    private StatHasMoveSpeed stat = null;

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
        Vector2 moveForce = moveDirection * MoveSpeed * Time.deltaTime;

        if (detector)
            moveForce = detector.GetMovableForce(moveForce);

        transform.Translate(moveForce, Space.World);

        movedForce = moveForce;
    }
}

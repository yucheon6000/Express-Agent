using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    [SerializeField]
    private new Collider2D collider2D;
    private ColliderCorner colliderCorner;
    private Bounds bounds;

    [SerializeField]
    private LayerMask layer;

    private int horizontalRayCount = 4;
    private int verticalRayCount = 4;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    ObstacleChecker checker;

    private readonly float skinWidth = 0.015f;

    private void Awake()
    {
        CalculateRaySpacing();
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    private void UpdateColliderCorner()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        colliderCorner.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        colliderCorner.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        colliderCorner.topLeft = new Vector2(bounds.min.x, bounds.max.y);
    }

    public Vector2 GetMovableForce(Vector2 moveForce)
    {
        checker.Reset();
        UpdateColliderCorner();

        if (moveForce.x != 0)
            RaycastsHorizontal(ref moveForce);
        if (moveForce.y != 0)
            RaycastsVertical(ref moveForce);

        return moveForce;
    }

    private void RaycastsHorizontal(ref Vector2 moveForce)
    {
        float direction = Mathf.Sign(moveForce.x);
        float distance = Mathf.Abs(moveForce.x) + skinWidth;
        Vector2 rayPosition = Vector2.zero;

        for (int i = 0; i < horizontalRayCount; ++i)
        {
            rayPosition = (direction == 1) ? colliderCorner.bottomRight : colliderCorner.bottomLeft;
            rayPosition += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.right * direction, distance, layer);

            if (hit)
            {
                moveForce.x = (hit.distance - skinWidth) * direction;
                distance = hit.distance;

                if ((direction == 1 && moveForce.x < 0) || (direction == -1 && moveForce.x > 0))
                    moveForce.x = 0;

                checker.left = direction == -1;
                checker.right = direction == 1;

                break;
            }

            // Debug.DrawLine(rayPosition, rayPosition + Vector2.right * direction * distance, Color.yellow);
        }
    }

    private void RaycastsVertical(ref Vector2 moveForce)
    {
        float direction = Mathf.Sign(moveForce.y);
        float distance = Mathf.Abs(moveForce.y) + skinWidth;
        Vector2 rayPosition = Vector2.zero;

        for (int i = 0; i < verticalRayCount; ++i)
        {
            rayPosition = (direction == 1) ? colliderCorner.topLeft : colliderCorner.bottomLeft;
            rayPosition += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.up * direction, distance, layer);

            if (hit)
            {
                moveForce.y = (hit.distance - skinWidth) * direction;
                distance = hit.distance;

                if ((direction == 1 && moveForce.y < 0) || (direction == -1 && moveForce.y > 0))
                    moveForce.y = 0;

                checker.up = direction == 1;
                checker.down = direction == -1;

                break;
            }

            // Debug.DrawLine(rayPosition, rayPosition + Vector2.up * direction * distance, Color.yellow);
        }
    }

    private struct ColliderCorner
    {
        public Vector2 topLeft;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    private struct ObstacleChecker
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public void Reset()
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }
    }
}

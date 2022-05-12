using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerAngle { Right = 0, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight }

public class PlayerAngleDetector : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    private PlayerAngle playerAngle;
    public PlayerAngle PlayerAngle => playerAngle;

    private bool isMouseTargeting = true;

    private class PlayerAngleEvent : UnityEvent<PlayerAngle> { }
    private PlayerAngleEvent playerAngleEvent = new PlayerAngleEvent();

    private void Awake()
    {
        if (targetTransform) return;
        targetTransform = transform;
    }

    public void AddPlayerAngleAction(UnityAction<PlayerAngle> action)
    {
        playerAngleEvent.AddListener(action);
    }

    public void RemovePlayerAngleAction(UnityAction<PlayerAngle> action)
    {
        playerAngleEvent.RemoveListener(action);
    }

    public void StartMouseTargeting()
    {
        isMouseTargeting = true;
    }

    public void StopMouseTargeting()
    {
        isMouseTargeting = false;
    }

    private void Update()
    {
        if (!isMouseTargeting) return;

        // 마우스 방향으로 PlayerAngle 지정
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - targetTransform.position;
        SetAngleIndexByDirection(direction);
    }

    public void SetAngleIndexByDirection(Vector3 direction)
    {
        // 계산된 PlayerAngle 지정
        SetAngleIndex(DirectionToPlayerAngle(direction));
    }

    public void SetAngleIndex(PlayerAngle angle)
    {
        // 기존과 같은 상태면 리턴
        if (angle == playerAngle) return;

        // 새로운 상태면 각 액션에 알림
        playerAngle = angle;
        playerAngleEvent.Invoke(playerAngle);
    }

    public static PlayerAngle DirectionToPlayerAngle(Vector3 direction)
    {
        float degree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        degree = (degree + 360) % 360;  // -180~180 -> 0~360
        int playerAngleIndex = (int)(degree / 22.5);
        playerAngleIndex = ((playerAngleIndex + 1) / 2) % 8;
        PlayerAngle playerAngle = (PlayerAngle)playerAngleIndex;
        return playerAngle;
    }
}

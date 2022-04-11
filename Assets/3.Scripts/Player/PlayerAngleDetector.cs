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

    private void Update()
    {
        // 현재 PlayerAngle 상태 확인
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - targetTransform.position;
        float degree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        degree = (degree + 360) % 360;  // -180~180 -> 0~360
        int playerAngleIndex = (int)(degree / 22.5);
        playerAngleIndex = ((playerAngleIndex + 1) / 2) % 8;
        PlayerAngle newPlayerAngle = (PlayerAngle)playerAngleIndex;

        // 기존과 같은 상태면 리턴
        if (newPlayerAngle == playerAngle) return;

        // 새로운 상태면 각 액션에 알림
        playerAngle = newPlayerAngle;
        playerAngleEvent.Invoke(playerAngle);
    }
}

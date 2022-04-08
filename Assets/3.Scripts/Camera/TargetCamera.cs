using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    private Camera camera;
    private Vector3 halfScreenSize;     // 화면크기의 절반 크기
    private int halfScreenMaxSize;      // 화면의 긴 부분의 절반 크기

    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    [Tooltip("세기\n(높을수록 마우스쪽을 더 바라봄)")]
    private float strength;

    [SerializeField]
    [Tooltip("민감도\n(높을수록 따라가는 속도 빠름)\n(0이면 바로 따라감)")]
    private float sensitivity;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        halfScreenSize = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        halfScreenMaxSize = Screen.width > Screen.height ? Screen.width / 2 : Screen.height / 2;
    }

    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }

    private void LateUpdate()
    {
        // 화면 기준 마우스 위치
        Vector3 mousePosition = Input.mousePosition - halfScreenSize;
        float mouseDistance = mousePosition.magnitude;  // 화면 중심으로부터 거리
        float mouseDistanceRatio = Mathf.Clamp(mouseDistance / halfScreenMaxSize, 0, 1);  // 마우스 거리 비율 (화면 중앙: 0, 끝: 1)

        // 플레이어 기준 카메라 위치 설정
        Vector3 targetPosition = targetTransform.position;
        Vector3 newCameraPosition = targetPosition + mousePosition.normalized * mouseDistanceRatio * strength;
        if (sensitivity > 0)
            newCameraPosition = Vector3.Lerp(transform.position, newCameraPosition, sensitivity * Time.deltaTime);
        newCameraPosition.z = camera.transform.position.z;
        transform.position = newCameraPosition;
    }

    public void StartFollowing()
    {
        this.enabled = true;
    }

    public void StopFollowing()
    {
        this.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformChanger : MonoBehaviour
{
    [SerializeField]
    private PlayerAngleDetector playerAngleDetector;

    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private Transform[] transforms;

    private void Awake()
    {
        targetTransform.position = transforms[0].position;
    }

    private void Start()
    {
        playerAngleDetector.AddPlayerAngleAction(OnPlayerAngleChanged);
    }

    private void OnPlayerAngleChanged(PlayerAngle playerAngle)
    {
        if ((int)playerAngle >= transforms.Length) return;
        targetTransform.position = transforms[(int)playerAngle].position;
    }
}

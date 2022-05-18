using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBullet : StraightBullet
{
    [Header("[Switch Bullet]")]
    [SerializeField]
    private float enableDistance = 10;
    private float distance = 0;
    private Vector3 startPosition;
    private bool enable = false;

    [SerializeField]
    private Behaviour[] enableComponents;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public override void Init(BulletInitInfo info)
    {
        base.Init(info);
        distance = 0;
        startPosition = transform.position;
        enable = false;
        SetColorA(0);
        SetEnableComponent(false);
    }

    protected override void Update()
    {
        base.Update();

        if (enable) return;

        float distance = Vector3.Distance(startPosition, transform.position);

        SetColorA(distance / enableDistance);

        if (distance >= enableDistance)
        {
            enable = true;
            SetColorA(1);
            SetEnableComponent(enable);
        }
    }

    private void SetEnableComponent(bool value)
    {
        foreach (var component in enableComponents)
            component.enabled = value;
    }

    private void SetColorA(float a)
    {
        if (spriteRenderer)
        {
            Color color = spriteRenderer.color;
            color.a = a;
            spriteRenderer.color = color;
        }
    }
}

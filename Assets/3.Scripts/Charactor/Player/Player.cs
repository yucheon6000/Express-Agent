using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    protected override void Start()
    {
        base.Start();
        weapon.StartTrigger();
    }

    private void Update()
    {
        /* 이동 */
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (Vector3.right * hor) + (Vector3.up * ver);

        movement.SetMoveDirection(moveDir);
    }

    protected override void OnDead() { }
}

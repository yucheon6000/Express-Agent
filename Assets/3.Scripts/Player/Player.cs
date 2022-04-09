using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    private void Update()
    {
        /* 이동 */
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (Vector3.right * hor) + (Vector3.up * ver);
        moveDir.Normalize();

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }
}

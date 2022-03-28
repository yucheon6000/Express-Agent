using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField]
    private SpriteRenderer weaponSpriteRenderer;

    private void Update()
    {
        /* 이동 */
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (Vector3.right * hor) + (Vector3.up * ver);
        moveDir.Normalize();

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        if (hor < 0)
        {
            weaponSpriteRenderer.sortingOrder = -100;
            playerSpriteRenderer.flipX = true;
        }
        else if (hor > 0)
        {
            weaponSpriteRenderer.sortingOrder = 100;
            playerSpriteRenderer.flipX = false;
        }

        /* 손 이동 */
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(weaponSpriteRenderer.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;
        weaponSpriteRenderer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        weaponSpriteRenderer.flipY = 90 < angle && angle < 270;

        Debug.Log(angle);
    }
}

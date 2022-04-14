using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, NeedCharacterStat
{
    private Vector2 moveDirection = Vector2.right;  // 이동 방향

    [Header("[Bullet Stat]")]
    [SerializeField]
    private BulletStat bulletStat;

    [Header("[Rigidbody2D]")]
    [SerializeField]
    private Rigidbody2D rigidbody;

    [Header("[Trail Renderer]")]
    [SerializeField]
    private TrailRenderer trailRenderer;

    public void Init(Vector2 moveDirection, CharacterStat characterStat)
    {
        bulletStat.SetCharacterStat(characterStat);
        this.moveDirection = ConvertMoveDirection(moveDirection);
    }

    private Vector3 ConvertMoveDirection(Vector2 moveDirection)
    {
        float standardAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        standardAngle = (standardAngle + 360) % 360;

        float newAngle = Random.Range(standardAngle - bulletStat.Scatter, standardAngle + bulletStat.Scatter);
        newAngle = (newAngle + 360) % 360;

        float theta = newAngle * Mathf.PI / 180;
        Vector3 newMoveDir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));

        return newMoveDir.normalized;
    }

    private void FixedUpdate()
    {
        float moveAmount = bulletStat.MoveSpeed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + moveDirection * moveAmount);

        Vector3 pos = rigidbody.position;
        if (pos.y < -30 || pos.y > 30 || pos.x > 30 || pos.x < -30)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (trailRenderer != null)
            trailRenderer.Clear();
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }

    public void SetCharacterStat(CharacterStat characterStat)
    {
        bulletStat.SetCharacterStat(characterStat);
    }
}

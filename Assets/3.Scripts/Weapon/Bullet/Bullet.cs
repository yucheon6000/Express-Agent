using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, NeedCharacterStat
{
    [Header("[Bullet Stat]")]
    [SerializeField]
    private BulletStat bulletStat;

    [Header("[Movement]")]
    [SerializeField]
    private Movement movement;

    [Header("[Trail Renderer]")]
    [SerializeField]
    private TrailRenderer trailRenderer;

    private void Start()
    {
        movement.SetStat(bulletStat);
    }

    public void Init(Vector2 moveDirection, CharacterStat characterStat)
    {
        bulletStat.SetCharacterStat(characterStat);
        movement.SetMoveDirection(ConvertMoveDirection(moveDirection));
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

    private void Update()
    {
        Vector3 pos = transform.position;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Monster"))
        {
            Character character = other.GetComponent<Character>();
            character.Hit(bulletStat.Attack);
            character.KnockBack(transform.position, bulletStat.KnockBack);
            gameObject.SetActive(false);
        }
    }
}

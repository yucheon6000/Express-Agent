using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, NeedCharacterStat
{
    [Header("[Bullet Stat]")]
    [SerializeField]
    protected BulletStat bulletStat;

    [Header("[Movement]")]
    [SerializeField]
    private Movement movement;

    [Header("[Trail Renderer]")]
    [SerializeField]
    private TrailRenderer trailRenderer;

    protected virtual void Start()
    {
        if (movement)
            movement.SetStat(bulletStat);
    }

    public virtual void Init(Vector2 moveDirection, CharacterStat characterStat)
    {
        bulletStat.SetCharacterStat(characterStat);
        Vector3 newMoveDir = ConvertMoveDirection(moveDirection);
        RotateBullet(newMoveDir);

        if (movement)
            movement.SetMoveDirection(newMoveDir);
    }

    protected Vector3 ConvertMoveDirection(Vector2 moveDirection)
    {
        float standardAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        standardAngle = (standardAngle + 360) % 360;

        float newAngle = Random.Range(standardAngle - bulletStat.Scatter, standardAngle + bulletStat.Scatter);
        newAngle = (newAngle + 360) % 360;

        float theta = newAngle * Mathf.PI / 180;
        Vector3 newMoveDir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));

        return newMoveDir.normalized;
    }

    protected void RotateBullet(Vector2 moveDirection)
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void Update()
    {
        Vector3 pos = transform.position;
        if (pos.y < -30 || pos.y > 30 || pos.x > 30 || pos.x < -30)
            gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        if (trailRenderer)
            trailRenderer.Clear();
    }

    protected virtual void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }

    public virtual void SetCharacterStat(CharacterStat characterStat)
    {
        bulletStat.SetCharacterStat(characterStat);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Player -> Monster
        // Monster -> Player
        if (bulletStat.CharacterStat.CharacterType == CharacterType.Player && other.tag.Equals(MonsterCollision.TAG)
            || bulletStat.CharacterStat.CharacterType == CharacterType.Monster && other.tag.Equals(PlayerCollision.TAG))
        {
            CharacterCollision character = other.GetComponent<CharacterCollision>();
            character.Hit(
                bulletStat.Attack,
                bulletStat.KnockBack,
                transform.position
            );
            gameObject.SetActive(false);
        }

        // Bullet -> Obstacle
        else if (other.tag.Equals("Obstacle"))
        {
            gameObject.SetActive(false);
        }
    }

    public CharacterType GetOwner() => bulletStat.CharacterStat.CharacterType;
}

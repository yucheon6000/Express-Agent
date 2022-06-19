using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeAttackBullet : Bullet
{
    [Header("[PlayerChangeAttackBullet]")]
    [SerializeField]
    private GameObject particlePrefab;
    [SerializeField]
    private CircleCollider2D circleCollider;
    [SerializeField]
    private LayerMask layerMask;

    private bool first = true;
    private int frameCount = 0;

    protected void OnEnable()
    {
        if (first)
        {
            first = false;
            return;
        }

        frameCount = 0;

        // 파티클 생성
        Vector3 pos = transform.position;
        pos.z = -1;
        ObjectPooler.SpawnFromPool(particlePrefab.name, pos);
    }

    protected override void Update()
    {
        frameCount++;
        if (frameCount < 3) return;

        // 충동 범위 안에 있는 오브젝트
        Collider2D[] colliders = Physics2D.OverlapCircleAll(circleCollider.bounds.center, circleCollider.bounds.extents.x, layerMask);

        // 공격
        foreach (var other in colliders)
        {
            if (other.CompareTag("Bullet"))
            {
                Bullet bullet = other.GetComponent<Bullet>();

                if (!bullet) continue;
                if (bullet.GetOwner() == CharacterType.Player) continue;

                Movement bulletMovement = bullet.GetComponent<Movement>();

                if (!bulletMovement || !bulletMovement.gameObject.activeSelf) continue;

                bulletMovement.SetMoveDirection(bullet.transform.position - transform.position);
            }
            else if (other.CompareTag(MonsterCollision.TAG))
            {
                MonsterCollision monster = other.GetComponent<MonsterCollision>();

                if (!monster) continue;
                if (!monster.transform.parent.gameObject.activeSelf) continue;

                monster.Hit(bulletStat.Attack, bulletStat.KnockBack, transform.position);
            }
        }

        // 총알 비활성화 (오브젝트풀에 반납)
        gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // base.OnTriggerEnter2D(other);
    }
}

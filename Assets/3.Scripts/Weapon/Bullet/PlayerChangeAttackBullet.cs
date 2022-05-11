using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeAttackBullet : Bullet
{
    private List<MonsterCollision> monsters = new List<MonsterCollision>();
    private List<Bullet> bullets = new List<Bullet>();

    private int targetFrame = 3;
    private int frame = 0;      // 총알 생성 후, 몬스터 및 총알의 충돌을 위해 쉬는 프레임 계산기


    [SerializeField]
    private GameObject particlePrefab;

    protected override void OnEnable()
    {
        base.OnEnable();
        frame = 0;
        monsters.Clear();
        bullets.Clear();

        // 파티클 생성
        ObjectPooler.SpawnFromPool(particlePrefab.name, transform.position);
    }

    protected override void Update()
    {
        frame++;
        if (frame < targetFrame) return;

        foreach (MonsterCollision monster in monsters)
        {
            if (!monster.transform.parent.gameObject.activeSelf) continue;

            monster.Hit(bulletStat.Attack, bulletStat.KnockBack, transform.position);
        }

        foreach (Bullet bullet in bullets)
        {
            Movement bulletMovement = bullet.GetComponent<Movement>();
            if (!bulletMovement || !bulletMovement.gameObject.activeSelf) continue;

            bulletMovement.SetMoveDirection(bullet.transform.position - transform.position);
        }

        gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (bullet.GetOwner() == CharacterType.Player) return;

            if (bullet)
                bullets.Add(bullet);
        }
        else if (other.CompareTag(MonsterCollision.TAG))
        {
            MonsterCollision monster = other.GetComponent<MonsterCollision>();
            if (monster)
                monsters.Add(monster);
        }
    }
}

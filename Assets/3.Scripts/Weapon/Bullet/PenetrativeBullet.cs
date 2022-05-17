using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrativeBullet : StraightBullet
{
    [Header("[PenetrativeBullet]")]
    [SerializeField]
    private bool isEnterTrigger = true;

    // 다단 히트
    [SerializeField]
    private float multipleAttackDeltaTime = 1f;
    private float multipleAttackTimer = 0;
    private Dictionary<Collider2D, CharacterCollision> enemies = new Dictionary<Collider2D, CharacterCollision>();
    private bool isAttackStarted = false;

    public override void Init(BulletInitInfo info)
    {
        base.Init(info);
        isAttackStarted = false;
        multipleAttackTimer = 0;
    }

    protected override void Update()
    {
        base.Update();

        multipleAttackTimer += Time.deltaTime;
        if (multipleAttackTimer >= multipleAttackDeltaTime)
        {
            foreach (var character in enemies.Values)
                HitEnemy(character);

            multipleAttackTimer = 0;
        }
    }

    protected override void OnTriggerEnterEnemy(Collider2D enemy)
    {
        CharacterCollision character = enemy.GetComponentInParent<CharacterCollision>();
        HitEnemy(character);

        if (!isEnterTrigger && !enemies.ContainsKey(enemy))
        {
            enemies.Add(enemy, character);

            if (!isAttackStarted)
                isAttackStarted = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isEnterTrigger) return;

        if (IsEnemy(other) && enemies.ContainsKey(other))
        {
            enemies.Remove(other);
        }
    }
}

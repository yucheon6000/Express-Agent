using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("[Character Stat]")]
    [SerializeField]
    protected CharacterStat characterStat;
    public CharacterStat CharacterStat => characterStat;

    [SerializeField]
    protected int currentHp;

    [Header("[Movement]")]
    [SerializeField]
    protected Movement movement;

    [Header("[Knock Back]")]
    [SerializeField]
    protected KnockBack knockBack;

    [Header("[Weapon]")]
    [SerializeField]
    protected Weapon weapon;

    protected virtual void Awake()
    {
        currentHp = characterStat.Health;
        knockBack = GetComponent<KnockBack>();
    }

    protected virtual void Start()
    {
        movement.SetStat(characterStat);
        weapon.SetCharacterStat(CharacterStat);
    }

    public virtual void Hit(float attack)
    {
        IncreaseHp(-attack);
    }

    public virtual void KnockBack(Vector2 hitPosition, float knockBack)
    {
        if (!this.knockBack || characterStat.KnockBackResistance >= 1) return;

        float force = knockBack - (knockBack * characterStat.KnockBackResistance);
        Vector2 dir = (Vector2)transform.position - hitPosition;

        this.knockBack.StartKnockBack(dir, force);
    }

    protected virtual void IncreaseHp(float attack)
    {
        currentHp += (int)attack;
        if (currentHp <= 0) OnDead();
    }

    protected abstract void OnDead();
}

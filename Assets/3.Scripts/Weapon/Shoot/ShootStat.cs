using System;
using UnityEngine;

[Serializable]
public class ShootStat : StatWithCharacterStat
{
    [SerializeField]
    [Tooltip("중간에 정지가 가능한지 여부")]
    protected bool breakable = true;

    public bool Breakable => breakable;
}
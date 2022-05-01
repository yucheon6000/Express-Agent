using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, NeedCharacterStat
{
    private CharacterStat characterStat = CharacterStat.Default;

    [Header("[Attacks]")]
    [SerializeField]
    private List<Attack> attacks;
    private bool isTrigger = false;
    public bool IsTrigger => isTrigger;

    /* Debug */
    [Header("[@Debug]")]
    [SerializeField]
    private bool debugMode = false;
    [SerializeField]
    private CharacterStat debugCharactorStat;

    private void Start()
    {
        // Debug
        if (debugMode)
        {
            SetDebugCharacterStat();
            StartTrigger();
        }
    }

    [ContextMenu("Start Trigger")]
    public void StartTrigger()
    {
        isTrigger = true;
        foreach (Attack attack in attacks)
            attack.StartAttack();
    }

    [ContextMenu("Stop Trigger")]
    public void StopTrigger()
    {
        isTrigger = false;
        foreach (Attack attack in attacks)
            attack.StopAttack();
    }

    [ContextMenu("Set Debug CharacterStat")]
    private void SetDebugCharacterStat()
    {
        SetCharacterStat(debugCharactorStat);
    }

    public void SetCharacterStat(CharacterStat characterStat)
    {
        if (this.characterStat == characterStat) return;

        this.characterStat = characterStat;
        foreach (Attack attack in attacks)
            attack.SetCharacterStat(characterStat);
    }

    public void AddAttack(Attack attack)
    {
        attacks.Add(attack);

        if (isTrigger) attack.StartAttack();
    }
}

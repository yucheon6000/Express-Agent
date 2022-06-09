using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class Monster : Character
{
    [Header("[Monster]")]
    [Header("[Points]")]
    [SerializeField]
    private int coinAmout = 50;
    [SerializeField]
    private int staminaAmount = 100;

    [Header("[UI]")]
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Transform hpSliderTransform;

    protected virtual void Update()
    {
        UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        // HP UI 업데이트
        if (isDead && hpSlider.gameObject.activeSelf)
        {
            hpSlider.gameObject.SetActive(false);
        }
        else if (!isDead && !hpSlider.gameObject.activeSelf)
        {
            hpSlider.gameObject.SetActive(true);
        }

        hpSlider.transform.position = Camera.main.WorldToScreenPoint(hpSliderTransform.position);
        hpSlider.value = currentHp / (float)characterStat.Health;
    }

    protected override void OnDead()
    {
        if (isDead) return;
        isDead = true;
        CreateCoinAndStamina();
    }

    protected void CreateCoinAndStamina()
    {
        int staminaCount = (int)(staminaAmount / Stamina.StaminaAmount);
        for (int i = 0; i < staminaCount; i++)
        {
            ObjectPooler.SpawnFromPool("Stamina", transform.position);
        }

        int coinCount = (int)(coinAmout / Coin.CointAmount);
        for (int i = 0; i < coinCount; i++)
        {
            ObjectPooler.SpawnFromPool("Coin", transform.position);
        }
    }

    protected static string INACTIVE = "Inactive";
    protected void Inactive()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        isDead = false;
        currentHp = characterStat.Health;
        UpdateUI();
    }

    protected virtual void OnDisable()
    {
        weapon.StopTrigger();
        ObjectPooler.ReturnToPool(this.gameObject);
    }

    protected MonsterSpawnTrigger monsterSpawnTrigger;
    public void SetMonsterSpawnTrigger(MonsterSpawnTrigger trigger)
    {
        monsterSpawnTrigger = trigger;
    }
}
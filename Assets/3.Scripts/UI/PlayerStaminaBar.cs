using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField]
    private GameObject[] barItems;
    private int targetCount;
    private int currentCount;
    private float lastCountUpdate;

    private void Start()
    {
        foreach (GameObject barItem in barItems)
            barItem.SetActive(false);
    }

    private void Update()
    {
        float HpPerItem = (float)Player.MaxStaminaCount / (float)barItems.Length;
        targetCount = (int)((float)Player.CurrentStaminaCount / HpPerItem);
        if (targetCount == currentCount) return;

        if (Time.time - lastCountUpdate >= 0.03f)
        {
            currentCount += (int)Mathf.Sign(targetCount - currentCount);
            lastCountUpdate = Time.time;

            for (int i = 0; i < barItems.Length; i++)
            {
                if (i < currentCount)
                    barItems[i].SetActive(true);
                else
                    barItems[i].SetActive(false);
            }
        }
    }
}

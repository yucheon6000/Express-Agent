using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField]
    private Image barImage;

    private void Update()
    {
        barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, Player.CurrentStaminaCount / (float)Player.MaxStaminaCount, Time.deltaTime * 4f);
    }
}

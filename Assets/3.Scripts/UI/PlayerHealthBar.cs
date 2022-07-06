using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField]
    private Image barImage;

    private void Update()
    {
        barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, Player.CurrentHp / (float)Player.MaxHp, Time.deltaTime * 4f);
    }
}

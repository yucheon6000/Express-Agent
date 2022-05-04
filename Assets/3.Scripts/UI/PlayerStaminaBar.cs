using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        float value = (float)Player.CurrentStaminaCount / (float)Player.MaxStaminaCount;

        slider.value = Mathf.Lerp(slider.value, value, Time.deltaTime * 6f);
    }
}

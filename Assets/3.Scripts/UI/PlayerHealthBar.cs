using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        float value = (float)Player.CurrentHp / (float)Player.MaxHp;
        value = Mathf.Lerp(slider.value, value, Time.deltaTime * 6f);

        slider.value = value;
    }
}

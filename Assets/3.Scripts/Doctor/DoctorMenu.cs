using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoctorMenu : MonoBehaviour
{
    [SerializeField]
    private Button buyButton;

    [SerializeField]
    private DoctorMenuStack[] menuStacks;

    private void Start()
    {
        UpdateBuyButton();
        PlayerPrefs.SetInt("rubyCount", 100);
    }

    public void UpdateBuyButton()
    {
        buyButton.interactable = false;

        if (Player.CurrentRubyCount < 20)
            return;

        foreach (var stack in menuStacks)
            if (stack.Increasable())
                buyButton.interactable = true;
    }

    public void Buy()
    {
        List<DoctorMenuStack> increasableStacks = new List<DoctorMenuStack>();

        foreach (var stack in menuStacks)
            if (stack.Increasable())
                increasableStacks.Add(stack);

        if (increasableStacks.Count == 0) return;

        int idx = Random.Range(0, increasableStacks.Count);
        increasableStacks[idx].IncreaseLevel();

        Player.IncreaseRubyCount(-20);

        UpdateBuyButton();
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        UpdateBuyButton();
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}

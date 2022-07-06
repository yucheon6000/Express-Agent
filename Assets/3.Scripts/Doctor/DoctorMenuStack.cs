using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorMenuStack : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> levelUIs;

    [SerializeField]
    private CharacterStatType statType;
    [SerializeField]
    private int level = 0;

    private void Awake()
    {
        level = PlayerPrefs.GetInt(statType.ToString(), 0);
        UpdateUI();
    }

    public bool Increasable()
    {
        return level < 3; // 0, 1, 2, 3
    }

    public void IncreaseLevel()
    {
        level++;
        level = Mathf.Clamp(level, 0, 3);
        PlayerPrefs.SetInt(statType.ToString(), level);
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (GameObject ui in levelUIs)
        {
            ui.SetActive(false);
        }

        for (int i = 1; i <= level; i++)
        {
            levelUIs[i - 1].SetActive(true);
        }
    }
}

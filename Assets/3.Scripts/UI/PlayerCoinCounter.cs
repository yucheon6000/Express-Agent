using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCoinCounter : MonoBehaviour
{
    private TextMeshProUGUI text;
    private int prevCnt = 0;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (prevCnt == Player.CurrentCoinCount) return;

        prevCnt = Player.CurrentCoinCount;
        text.text = prevCnt.ToString();
    }
}

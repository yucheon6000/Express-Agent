using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerRubyCounter : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI text;
    private int prevCnt = -1;

    [SerializeField]
    private bool fadeOut = false;

    private void Update()
    {
        if (prevCnt == Player.CurrentRubyCount) return;

        prevCnt = Player.CurrentRubyCount;
        text.text = prevCnt.ToString();

        if (!fadeOut) return;

        image.color = Color.white;
        text.color = Color.white;

        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(1f);

        float timer = 0;
        float percent = 0;
        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / 2f;

            Color c = image.color;
            c.a = Mathf.Lerp(c.a, 0, percent);

            image.color = c;
            text.color = c;

            yield return null;
        }
    }
}

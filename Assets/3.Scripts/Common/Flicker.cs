using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flicker : MonoBehaviour
{
    [SerializeField]
    private Image target;
    [SerializeField]
    private float time;
    [SerializeField]
    private float minA;
    [SerializeField]
    private float maxA;

    private void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(maxA, minA));
            yield return StartCoroutine(Fade(minA, maxA));
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float timer = 0;
        float percent = 0;
        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / time;

            Color color = target.color;
            color.a = Mathf.Lerp(from, to, percent);

            target.color = color;

            yield return null;
        }
    }
}

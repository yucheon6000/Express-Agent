using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flicker : MonoBehaviour
{
    [SerializeField]
    private Image targetImage;
    [SerializeField]
    private SpriteRenderer targetSpriteRenderer;
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

            if (targetImage)
            {
                Color color = targetImage.color;
                color.a = Mathf.Lerp(from, to, percent);

                targetImage.color = color;
            }

            if (targetSpriteRenderer)
            {
                Color color = targetSpriteRenderer.color;
                color.a = Mathf.Lerp(from, to, percent);

                targetSpriteRenderer.color = color;
            }

            yield return null;
        }
    }
}

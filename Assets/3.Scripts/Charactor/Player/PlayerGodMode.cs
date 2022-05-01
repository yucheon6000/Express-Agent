using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGodMode : MonoBehaviour
{
    [SerializeField]
    private float godModeTime = 0.5f;
    [SerializeField]
    private int godModeFadeCount = 5;

    [SerializeField]
    private float minAlpha = 0.3f;
    [SerializeField]
    private float maxAlpha = 1f;

    private bool isGodMode = false;
    public bool IsGodMode => isGodMode;

    private Coroutine coroutine;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void StartGodMode()
    {
        if (isGodMode) return;

        coroutine = StartCoroutine(GodModeRoutine());
    }

    public void StopGodMode()
    {
        if (!isGodMode || coroutine == null) return;

        isGodMode = false;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator GodModeRoutine()
    {
        isGodMode = true;

        float timeOnce = godModeTime / (godModeFadeCount * 2);

        for (int i = 0; i < godModeFadeCount; ++i)
        {
            yield return StartCoroutine(ImageFadeRoutine(maxAlpha, minAlpha, timeOnce));
            yield return StartCoroutine(ImageFadeRoutine(minAlpha, maxAlpha, timeOnce));
        }

        SetAlpha(1);

        isGodMode = false;
        coroutine = null;
    }

    private IEnumerator ImageFadeRoutine(float start, float end, float time)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            float alpha = Mathf.Lerp(start, end, percent);
            SetAlpha(alpha);


            yield return null;
        }
    }

    private void SetAlpha(float a)
    {
        Color color = spriteRenderer.color;
        color.a = a;

        spriteRenderer.color = color;
    }
}

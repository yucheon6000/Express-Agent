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
    private float hitTime = 0.3f;
    [SerializeField]
    private float hitCount = 1;

    [SerializeField]
    private float minAlpha = 0.3f;
    [SerializeField]
    private float maxAlpha = 1f;

    [SerializeField]
    private bool isGodMode = false;
    public bool IsGodMode => isGodMode;

    private Coroutine coroutine;

    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;

    [SerializeField]
    private SpriteRenderer[] spriteRenderers;

    private void Start()
    {
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = spriteRenderers.Length > 0 ? spriteRenderers[0].material.shader : Shader.Find("Sprites/Default");
    }

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

        float timeOnce = hitTime / (hitCount * 3);

        for (int i = 0; i < hitCount; ++i)
        {
            yield return StartCoroutine(SetColor(Color.red, timeOnce));
            yield return StartCoroutine(SetColor(Color.white, timeOnce));
            yield return StartCoroutine(SetOrigin(timeOnce));
        }

        SetOrigin();
        SetAlpha(1);

        timeOnce = (godModeTime - hitCount) / (godModeFadeCount * 3);
        for (int i = 0; i < godModeFadeCount; ++i)
        {
            yield return StartCoroutine(ImageFadeRoutine(maxAlpha, minAlpha, timeOnce));
            yield return StartCoroutine(ImageFadeRoutine(minAlpha, maxAlpha, timeOnce));
        }

        SetOrigin();
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
        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = a;

            spriteRenderer.color = color;
        }
    }

    private IEnumerator SetColor(Color color, float time)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
            spriteRenderer.material.shader = shaderGUItext;
        }
        yield return new WaitForSeconds(time);
    }

    private IEnumerator SetOrigin(float time)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.material.shader = shaderSpritesDefault;
        }
        yield return new WaitForSeconds(time);
    }

    private void SetOrigin()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.shader = shaderSpritesDefault;
            spriteRenderer.color = Color.white;
        }
    }
}

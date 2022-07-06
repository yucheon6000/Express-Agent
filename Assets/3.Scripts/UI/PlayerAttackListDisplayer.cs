using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackListDisplayer : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private bool on;

    [SerializeField]
    private float onTime;
    [SerializeField]
    private AnimationCurve onCurve;
    [SerializeField]
    private Vector3 onPosition;

    [SerializeField]
    private float offTime;
    [SerializeField]
    private AnimationCurve offCurve;
    [SerializeField]
    private Vector3 offPosition;

    private void Awake()
    {
        rectTransform.anchoredPosition = on ? onPosition : offPosition;
    }

    public void Display(bool enable)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayRoutine(enable));
    }

    private IEnumerator DisplayRoutine(bool enable)
    {
        Vector3 firstPosition = rectTransform.anchoredPosition;

        Vector3 lastPosition = enable ? onPosition : offPosition;
        AnimationCurve curve = enable ? onCurve : offCurve;
        float time = enable ? onTime : offTime;
        float timer = 0;
        float percent = 0;

        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = curve.Evaluate(timer / time);

            Vector3 newPos = Vector3.Lerp(firstPosition, lastPosition, percent);
            rectTransform.anchoredPosition = newPos;

            yield return null;
        }

        rectTransform.anchoredPosition = lastPosition;
    }
}

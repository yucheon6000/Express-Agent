using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Doctor : MonoBehaviour
{
    public UnityEvent onClickedDoctor = new UnityEvent();

    [Header("[Explain]")]
    [SerializeField]
    private TextMeshProUGUI textExplain;
    [SerializeField]
    private Image imageExplain;
    [SerializeField]
    private Transform explainTransform;
    [SerializeField]
    private float charDelayTime = 0.1f;
    [SerializeField]
    private float spaceDelayTime = 0.2f;
    [SerializeField]
    private float clickableMouseDistance = 0.3f;

    private void Start()
    {
        Explain("준비되었는가?\n위쪽 문을 통해 적진으로 출발하게!\n혹시, 너희의 능력을 키우고 싶다면\n나를 클릭해보게나.");
    }

    private void Update()
    {
        imageExplain.transform.position = Camera.main.WorldToScreenPoint(explainTransform.position);

        if (Input.GetMouseButtonDown(0))
        {
            // 마우스와 박사의 거리
            float dist = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // 클릭이 안되었으면 리턴
            if (dist > clickableMouseDistance) return;

            onClickedDoctor.Invoke();
        }
    }

    public void Explain(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ExplainRoutine(message));
    }

    private IEnumerator ExplainRoutine(string message)
    {
        textExplain.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            textExplain.text += message[i];

            if (message[i].Equals(" ")) yield return new WaitForSeconds(spaceDelayTime);
            else yield return new WaitForSeconds(charDelayTime);
        }
    }
}

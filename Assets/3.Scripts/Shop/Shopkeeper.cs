using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shopkeeper : MonoBehaviour
{
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

    [Header("Display")]
    [SerializeField]
    private Transform[] tableTransforms;
    [SerializeField]
    private GameObject[] shopItemPrefabs;
    [SerializeField]
    private int minItemCount = 0;
    [SerializeField]
    private int maxItemCount = 5;

    [SerializeField]
    private GameObject soldoutPrefab;

    private void Start()
    {
        Display();
    }

    private void Update()
    {
        imageExplain.transform.position = Camera.main.WorldToScreenPoint(explainTransform.position);
    }

    private void Display()
    {
        int count = Random.Range(minItemCount, maxItemCount + 1);
        for (int i = 0; i < tableTransforms.Length; i++)
        {
            if (count > 0)
            {
                int idx = Random.Range(0, shopItemPrefabs.Length);
                GameObject item = Instantiate(shopItemPrefabs[idx], tableTransforms[i].transform.position, Quaternion.identity);
                ShopItem shopItem = item.GetComponent<ShopItem>();
                shopItem.Init(this);
                count--;
            }
            else
            {
                Instantiate(soldoutPrefab, tableTransforms[i].transform.position, Quaternion.identity);
            }
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

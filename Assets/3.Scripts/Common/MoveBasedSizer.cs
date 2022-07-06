using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBasedSizer : MonoBehaviour
{
    [SerializeField]
    private float addAmountPerUnit = 0;

    private Vector3 originScale;
    private Vector3 prevPosition;

    private void Awake()
    {
        prevPosition = transform.position;
        originScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = originScale;
        prevPosition = transform.position;
    }

    private void Update()
    {
        float moveDist = Vector3.Distance(prevPosition, transform.position);

        transform.localScale += (addAmountPerUnit * moveDist * Vector3.one);

        prevPosition = transform.position;
    }
}

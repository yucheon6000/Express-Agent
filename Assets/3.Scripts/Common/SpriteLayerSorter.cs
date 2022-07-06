using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerSorter : MonoBehaviour
{
    [SerializeField]
    private bool isStatic = false;
    [SerializeField]
    private Transform pivot = null;

    private IEnumerator Start()
    {
        UpdateZ();

        while (!isStatic)
        {
            UpdateZ();
            yield return null;
        }
    }

    private void UpdateZ()
    {
        Vector3 pos = transform.position;
        pos.z = pivot ? pivot.transform.position.y : pos.y;
        pos.z = (pos.z - 1000f) / 1000f;
        transform.position = pos;
    }
}

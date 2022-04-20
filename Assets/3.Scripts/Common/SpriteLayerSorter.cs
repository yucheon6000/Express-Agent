using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerSorter : MonoBehaviour
{
    [SerializeField]
    private bool isStatic = false;

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
        pos.z = pos.y / 1000f;
        transform.position = pos;
    }
}

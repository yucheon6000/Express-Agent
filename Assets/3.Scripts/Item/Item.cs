using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected virtual void OnDisable()
    {
        ObjectPooler.ReturnToPool(this.gameObject);
    }
}

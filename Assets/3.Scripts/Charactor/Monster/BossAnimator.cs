using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    [SerializeField]
    private BossMonster boss;

    public void EndAnimation()
    {
        boss.OnEndAnimation();
    }
}

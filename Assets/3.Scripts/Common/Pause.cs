using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    bool IsPause;
    float originTimeScale = 0;

    [SerializeField]
    private GameObject canvasPause;

    void Start()
    {
        IsPause = false;
        canvasPause.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            /*일시정지 활성화*/
            if (IsPause == false)
            {
                canvasPause.SetActive(true);
                originTimeScale = Time.timeScale;
                Time.timeScale = 0;
                IsPause = true;
                return;
            }

            /*일시정지 비활성화*/
            if (IsPause == true)
            {
                canvasPause.SetActive(false);
                Time.timeScale = originTimeScale;
                IsPause = false;
                return;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletDisabler : MonoBehaviour
{
    public class BulletDisableEvent : UnityEvent { }
    [HideInInspector]
    public BulletDisableEvent bulletDisableEvent = new BulletDisableEvent();

    [SerializeField]
    private bool once = false;

    [Header("[Trigger]")]
    [SerializeField]
    private bool timeTrigger = false;
    [SerializeField]
    private float timeLimit = 0;
    private float timer = 0;

    [SerializeField]
    private bool distanceTrigger = false;
    [SerializeField]
    private float distanceLimit = 0;
    private float distance = 0;
    private Vector3 prevPosition;

    private bool disable = false;

    private void OnEnable()
    {
        disable = false;
        timer = 0;
        distance = 0;
        prevPosition = transform.position;
    }

    private void Update()
    {
        if (timeTrigger)
        {
            timer += Time.deltaTime;
            if (timer >= timeLimit)
                Disable();
        }

        if (distanceTrigger)
        {
            distance += Vector3.Distance(prevPosition, transform.position);
            prevPosition = transform.position;
            if (distance >= distanceLimit)
                Disable();
        }
    }

    private void Disable()
    {
        if (disable) return;

        disable = true;
        bulletDisableEvent.Invoke();

        gameObject.SetActive(false);

        if (once)
            this.enabled = false;
    }
}

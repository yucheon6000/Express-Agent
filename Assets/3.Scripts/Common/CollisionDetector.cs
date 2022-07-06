using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DetectType { Enter, Stay, Exit }

public class CollisionDetector : MonoBehaviour
{
    [SerializeField]
    private DetectItem[] targetItems;

    private class CollisionDetectEvent : UnityEvent<Transform, string, DetectType> { }

    private CollisionDetectEvent detectEvent = new CollisionDetectEvent();

    private bool active = true;

    public void AddCollisionDetectAction(UnityAction<Transform, string, DetectType> action)
    {
        detectEvent.AddListener(action);
    }

    public void RemoveCollisionDetectAction(UnityAction<Transform, string, DetectType> action)
    {
        detectEvent.RemoveListener(action);
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        foreach (DetectItem targetItem in targetItems)
        {
            if (!targetItem.enter) continue;
            if (other.tag.Equals(targetItem.tag) == false) continue;

            detectEvent.Invoke(other.transform, other.tag, DetectType.Enter);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!active) return;

        foreach (DetectItem targetItem in targetItems)
        {
            if (!targetItem.stay) continue;
            if (other.tag.Equals(targetItem.tag) == false) continue;

            detectEvent.Invoke(other.transform, other.tag, DetectType.Stay);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!active) return;

        foreach (DetectItem targetItem in targetItems)
        {
            if (!targetItem.exit) continue;
            if (other.tag.Equals(targetItem.tag) == false) continue;

            detectEvent.Invoke(other.transform, other.tag, DetectType.Exit);
        }
    }

    [System.Serializable]
    private struct DetectItem
    {
        [SerializeField]
        public string tag;
        [SerializeField]
        public bool enter;
        [SerializeField]
        public bool stay;
        [SerializeField]
        public bool exit;
    }
}

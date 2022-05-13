using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorCollision : Collision
{
    public class OnEnterDoor : UnityEvent<Vector3, GameObject, GameObject> { }      // targetPosition, from, to
    public OnEnterDoor onEnterDoorEvent = new OnEnterDoor();

    [SerializeField]
    private GameObject fromMap;
    [SerializeField]
    private GameObject toMap;
    [SerializeField]
    private Transform targetPosition;
    [SerializeField]
    private bool isOpen = false;
    private bool playerIsEnter = false;
    [SerializeField]
    private MonsterSpawnController monsterSpawnController;

    private void Start()
    {
        monsterSpawnController.onClearStageEvent.AddListener(this.Open);
    }

    private void Open()
    {
        isOpen = true;
        print("Door is opened");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isOpen) return;
        if (playerIsEnter) return;
        if (!other.CompareTag(PlayerCollision.TAG)) return;
        if (other.GetComponentInParent<Player>() != Player.Main) return;

        playerIsEnter = true;
        onEnterDoorEvent.Invoke(targetPosition.position, fromMap, toMap);
    }
}

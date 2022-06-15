using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorCollision : Collision
{
    public class OnEnterDoor : UnityEvent<Vector3, GameObject, GameObject> { }      // targetPosition, from, to
    public OnEnterDoor onEnterDoorEvent = new OnEnterDoor();

    [SerializeField]
    private int stageIndex;

    [SerializeField]
    private GameObject fromMap;
    [SerializeField]
    private GameObject toMap;
    [SerializeField]
    private Transform targetPosition;
    private bool playerIsEnter = false;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float animateTime;

    [SerializeField]
    private bool changeScene = false;
    [SerializeField]
    private string sceneName;

    [Header("@Debug")]
    [SerializeField]
    private bool isOpen = false;
    [SerializeField]
    private bool finishedAnimation = false;

    private void Start()
    {
        MonsterSpawnController.onStartStageEvent.AddListener(this.Close);
        MonsterSpawnController.onClearStageEvent.AddListener(this.Open);
    }

    private void Close(int stageIndex)
    {
        if (this.stageIndex != stageIndex) return;

        animator.Play("Close", -1);
    }

    private void Open(int stageIndex)
    {
        if (this.stageIndex != stageIndex) return;

        isOpen = true;
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        animator.Play("Open", -1);

        yield return new WaitForSeconds(animateTime);

        finishedAnimation = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isOpen || !finishedAnimation) return;
        if (playerIsEnter) return;
        if (!other.CompareTag(PlayerCollision.TAG)) return;
        if (other.GetComponentInParent<Player>() != Player.Main) return;

        if (changeScene)
        {
            LoadingSceneManager.LoadScene(sceneName);
            return;
        }

        playerIsEnter = true;
        onEnterDoorEvent.Invoke(targetPosition.position, fromMap, toMap);
    }
}

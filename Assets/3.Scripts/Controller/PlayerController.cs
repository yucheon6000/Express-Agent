using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("[Player]")]
    [SerializeField]
    private Player[] players;
    [SerializeField]
    private int mainPlayerIndex = 0;
    [SerializeField]
    private KeyCode KeyCodePlayerChange = KeyCode.Space;
    [SerializeField]
    private float minChangeModeDistance = 3f;
    [SerializeField]
    private int changeStaminaAmount = 25;

    [Header("[Cameara]")]
    [SerializeField]
    private TargetCamera targetCamera;

    [Header("[Doors]")]
    [SerializeField]
    private DoorCollision[] doors;

    private void Start()
    {
        foreach (DoorCollision door in doors)
        {
            door.onEnterDoorEvent.AddListener((Vector3 targetPosition, GameObject fromMap, GameObject toMap) =>
            {
                toMap.SetActive(true);
                foreach (Player player in players)
                {
                    player.MoveTo(targetPosition);
                }
                fromMap.SetActive(false);
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCodePlayerChange))
        {
            ChangeMainPlayer(1);
        }
    }

    private void ChangeMainPlayer(int step = 1)
    {
        if (Player.Main.IsDead) return;
        if (Player.CurrentStaminaCount < changeStaminaAmount) return;

        foreach (Player current in players)
        {
            foreach (Player other in players)
            {
                if (current == other) continue;


                // 최소 거리 안 될 경우, 리턴
                if ((current.TargetPosition - other.TargetPosition).sqrMagnitude > Mathf.Pow(minChangeModeDistance, 2))
                    return;
            }
        }

        mainPlayerIndex += step;
        if (mainPlayerIndex >= players.Length) mainPlayerIndex = 0;
        if (mainPlayerIndex < 0) mainPlayerIndex = players.Length - 1;

        Player mainPlayer = players[mainPlayerIndex];

        foreach (Player player in players)
        {
            player.UpdatePlayerType(
                player == mainPlayer ? PlayerType.Main : PlayerType.Sidekick
            );
        }

        Player.IncreaseStaminaCount(-changeStaminaAmount);

        targetCamera.SetTargetTransform(mainPlayer.transform);
    }
}

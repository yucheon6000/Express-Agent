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

    [Header("[Cameara]")]
    [SerializeField]
    private TargetCamera targetCamera;

    private void Start()
    {
        ChangeMainPlayer(0);
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
        if (Player.CurrentStaminaCount < (Player.MaxStaminaCount / 4)) return;

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

        Player.IncreaseStaminaCount(-(Player.MaxStaminaCount / 4));

        targetCamera.SetTargetTransform(mainPlayer.transform);
    }
}

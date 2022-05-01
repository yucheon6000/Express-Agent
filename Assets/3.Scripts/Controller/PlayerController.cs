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

        targetCamera.SetTargetTransform(mainPlayer.transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawnTrigger : MonoBehaviour
{
    [SerializeField]
    private MonsterSpawnController monsterSpawnController;
    [SerializeField]
    private MonsterSpawnControlInfo monsterSpawnControlInfo;
    [SerializeField]
    private Tilemap targetTilemap;
    [SerializeField]
    private string targetTileName;

    private bool init = false;
    private bool isTrigger = false;

    private new Collider2D collider2D;

    private void Start()
    {
        // 타켓 타일 위치 목록 저장
        List<Vector2> targetTilePositions = new List<Vector2>();

        for (int i = 0; i < targetTilemap.transform.childCount; i++)
        {
            Transform tile = targetTilemap.transform.GetChild(i);
            if (tile.name.IndexOf(targetTileName) != -1)
            {
                targetTilePositions.Add(tile.transform.position);
            }
        }

        monsterSpawnControlInfo.targetTilePositions = targetTilePositions;

        collider2D = GetComponent<Collider2D>();

        init = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!init) return;
        if (isTrigger) return;
        if (!other.CompareTag(PlayerCollision.TAG)) return;

        isTrigger = true;
        monsterSpawnController.StartSpawn(monsterSpawnControlInfo);
        collider2D.enabled = false;
    }
}

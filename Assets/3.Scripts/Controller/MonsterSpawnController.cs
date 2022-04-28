using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawnController : MonoBehaviour
{
    [Header("[KeyCode]")]
    [SerializeField]
    private KeyCode spawnKeyCode = KeyCode.M;

    [Header("[Tilemap]")]
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private string targetTileName;
    private List<Vector2> targetTilePositions;

    [Header("[Monster]")]
    [SerializeField]
    private GameObject monsterPrefab;
    [SerializeField]
    private int targetCount;


    private void Start()
    {
        // 타켓 타일 위치 목록 저장
        targetTilePositions = new List<Vector2>();

        for (int i = 0; i < tilemap.transform.childCount; i++)
        {
            Transform tile = tilemap.transform.GetChild(i);
            if (tile.name.IndexOf(targetTileName) != -1)
            {
                targetTilePositions.Add(tile.transform.position);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(spawnKeyCode))
        {
            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
        List<Vector2> positions = new List<Vector2>(targetTilePositions);

        for (int i = 0; i < targetCount; i++)
        {
            if (positions.Count <= 0) break;

            int idx = Random.Range(0, positions.Count);
            Vector3 pos = positions[idx];
            positions.RemoveAt(idx);
            ObjectPooler.SpawnFromPool(monsterPrefab.name, pos, Quaternion.identity);
        }
    }
}

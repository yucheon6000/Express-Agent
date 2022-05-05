using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

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

    [Header("[Wave]")]
    [SerializeField]
    private MonsterSpawnWave[] monsterSpawnWaves;
    [SerializeField]
    private float waveDeltaTime = 5;
    [SerializeField]
    private int minWaveCount = 0;
    [SerializeField]
    private int maxWaveCount = 5;

    private bool isStarted = false;

    private bool playerIsReady = false;

    [Header("[Controller]")]
    [SerializeField]
    private PlayerAbilityController playerAbilityController;

    [Header("[UI]")]
    [SerializeField]
    private TextMeshProUGUI textWave;

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
            StartSpawn();
        }
    }

    public void PlayerIsReady()
    {
        playerIsReady = true;
    }

    public void StartSpawn()
    {
        if (isStarted) return;

        isStarted = true;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        int minWaveCnt = Mathf.Min(Mathf.Min(minWaveCount, maxWaveCount), monsterSpawnWaves.Length);
        int maxWaveCnt = Mathf.Min(Mathf.Max(minWaveCount, maxWaveCount), monsterSpawnWaves.Length);
        int waveCnt = Random.Range(minWaveCnt, maxWaveCnt + 1);

        for (int i = 0; i < waveCnt; i++)
        {
            // UI
            textWave.gameObject.SetActive(true);
            textWave.text = $"<size=40>WAVE</size>\n{i + 1}";
            yield return new WaitForSeconds(0.5f);
            textWave.gameObject.SetActive(false);

            // 웨이브 시작
            MonsterSpawnWave wave = monsterSpawnWaves[i];
            yield return StartCoroutine(WaveRoutine(wave));

            // 웨이브 종료
            print($"End Wave {i}");
            playerAbilityController.StartDisplay(waveDeltaTime);

            float timer = 0;
            playerIsReady = false;
            while (timer < waveDeltaTime)
            {
                timer += Time.deltaTime;

                if (playerIsReady) break;

                yield return null;
            }
        }
    }

    private IEnumerator WaveRoutine(MonsterSpawnWave wave)
    {

        List<Vector2> spawnableTilePositions = new List<Vector2>(targetTilePositions);
        List<GameObject> spawnedMonsterList = new List<GameObject>();

        foreach (MonsterSpawnWaveUnit wUnit in wave.waveUnits)
        {
            if (!wUnit.Monster) continue;

            SpawnMonster(wUnit.Monster.gameObject.name, wUnit.getSpawnCount(), ref spawnableTilePositions, ref spawnedMonsterList);
        }

        while (true)
        {
            bool flag = true;

            foreach (GameObject monster in spawnedMonsterList)
                if (monster.activeSelf)
                {
                    flag = false;
                    break;
                }

            if (flag)
                break;
            else
                yield return null;
        }
    }

    private void SpawnMonster(string monsterName, int targetCount, ref List<Vector2> spawnableTilePositions, ref List<GameObject> spawnedMonsterList)
    {
        for (int i = 0; i < targetCount; i++)
        {
            if (spawnableTilePositions.Count <= 0) break;

            int idx = Random.Range(0, spawnableTilePositions.Count);
            Vector3 pos = spawnableTilePositions[idx];

            spawnableTilePositions.RemoveAt(idx);

            GameObject monster = ObjectPooler.SpawnFromPool(monsterName, pos, Quaternion.identity);
            spawnedMonsterList.Add(monster);
        }
    }
}

[System.Serializable]
public class MonsterSpawnWave
{
    public MonsterSpawnWaveUnit[] waveUnits;
}

[System.Serializable]
public class MonsterSpawnWaveUnit
{
    [SerializeField]
    private Monster monsterPrefab;
    [SerializeField]
    private int minMonsterCount;
    [SerializeField]
    private int maxMonsterCount;

    public Monster Monster => monsterPrefab;

    public int getSpawnCount()
    {
        int min = Mathf.Min(minMonsterCount, maxMonsterCount);
        int max = Mathf.Max(minMonsterCount, maxMonsterCount);

        return Random.Range(min, max + 1);
    }
}
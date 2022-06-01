using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using TMPro;

public class MonsterSpawnController : MonoBehaviour
{
    public class OnClearWave : UnityEvent<float> { }    // waveDeltaTime
    [HideInInspector]
    public OnClearWave onClearWaveEvent = new OnClearWave();

    public class OnClearStage : UnityEvent { }
    [HideInInspector]
    public OnClearStage onClearStageEvent = new OnClearStage();

    [Header("[KeyCode]")]
    [SerializeField]
    private KeyCode spawnKeyCode = KeyCode.M;

    [Header("[Controller]")]
    [SerializeField]
    private PlayerAbilityController playerAbilityController;

    [Header("[UI]")]
    [SerializeField]
    private TextMeshProUGUI textWave;

    private bool isStarted = false;
    private bool skipWaveDeltaTime = false;
    private MonsterSpawnControlInfo controlInfo;

    private void Update()
    {
        if (Input.GetKeyDown(spawnKeyCode))
        {
            if (controlInfo != null)
                StartSpawn(controlInfo);
        }
    }

    public void SkipWaveDeltaTime()
    {
        // 어빌리티를 선택했을 경우
        skipWaveDeltaTime = true;
    }

    public void StartSpawn(MonsterSpawnControlInfo info)
    {
        if (isStarted) return;
        controlInfo = info;
        isStarted = true;

        StartCoroutine(SpawnRoutine(info));
    }

    private IEnumerator SpawnRoutine(MonsterSpawnControlInfo info)
    {
        int minWaveCnt = Mathf.Min(Mathf.Min(info.minWaveCount, info.maxWaveCount), info.monsterSpawnWaves.Length);
        int maxWaveCnt = Mathf.Min(Mathf.Max(info.minWaveCount, info.maxWaveCount), info.monsterSpawnWaves.Length);
        int waveCnt = Random.Range(minWaveCnt, maxWaveCnt + 1);

        // stageDelayTimeAtStart초 만큼 쉬면서
        // 스테이지 시작 UI 활성화
        textWave.gameObject.SetActive(true);
        textWave.text = $"<size=40>STAGE</size>\nSTART!";
        yield return new WaitForSeconds(info.stageDelayTimeAtStart);
        textWave.gameObject.SetActive(false);

        // 스테이지 시작
        for (int i = 0; i < waveCnt; i++)
        {
            // UI
            textWave.gameObject.SetActive(true);
            textWave.text = $"<size=40>WAVE</size>\n{i + 1}";
            yield return new WaitForSeconds(2f);
            textWave.gameObject.SetActive(false);

            // 웨이브 시작
            MonsterSpawnWave wave = info.monsterSpawnWaves[i];
            yield return StartCoroutine(WaveRoutine(wave, info.targetTilePositions));

            // 웨이브 종료
            onClearWaveEvent.Invoke(info.waveDeltaTime);

            // 웨이브 사이 시간동안 기다림
            // 중간에 플레이어가 어빌리티 선택을 완료해 준비되면 바로 다음 웨이브 시작
            float timer = 0;
            skipWaveDeltaTime = false;
            while (timer < info.waveDeltaTime)
            {
                timer += Time.deltaTime;
                Player.CantAttack(true);    // 공격 금지

                if (skipWaveDeltaTime) break;

                yield return null;
            }
            Player.CantAttack(false);
        }

        // 스테이지 클리어 UI 활성화
        textWave.gameObject.SetActive(true);
        textWave.text = $"<size=40>STAGE</size>\nCLEAR!";

        // 스테이지 끝나면 코인 획득
        List<GameObject> coins = ObjectPooler.GetAllPools("Coin", true);
        foreach (GameObject coin in coins)
        {
            coin.GetComponent<Coin>().StartTargeting();
        }

        // stageDelayTimeAtEnd초 만큼 쉬고
        // 스테이지 클리어 UI 비활성화
        yield return new WaitForSeconds(info.stageDelayTimeAtEnd);
        textWave.gameObject.SetActive(false);

        // 이벤트 호출
        onClearStageEvent.Invoke();
        isStarted = false;
    }

    private IEnumerator WaveRoutine(MonsterSpawnWave wave, List<Vector2> targetTilePositions)
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
public class MonsterSpawnControlInfo
{
    public float stageDelayTimeAtStart;
    public float stageDelayTimeAtEnd;
    public MonsterSpawnWave[] monsterSpawnWaves;
    public float waveDeltaTime = 5;
    public int minWaveCount = 0;
    public int maxWaveCount = 5;
    [HideInInspector]
    public List<Vector2> targetTilePositions;
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
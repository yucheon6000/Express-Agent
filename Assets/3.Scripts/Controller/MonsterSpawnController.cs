using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using TMPro;

public class MonsterSpawnController : MonoBehaviour
{
    public class OnStartStage : UnityEvent<int> { }     // stageIndex
    [HideInInspector]
    public static OnStartStage onStartStageEvent = new OnStartStage();

    public class OnClearWave : UnityEvent<float> { }    // waveDeltaTime
    [HideInInspector]
    public static OnClearWave onClearWaveEvent = new OnClearWave();

    public class OnClearStage : UnityEvent<int> { }     // stageIndex
    [HideInInspector]
    public static OnClearStage onClearStageEvent = new OnClearStage();

    [Header("[KeyCode]")]
    [SerializeField]
    private KeyCode spawnKeyCode = KeyCode.M;

    [Header("[Controller]")]
    [SerializeField]
    private PlayerAbilityController playerAbilityController;
    [SerializeField]
    private GameObject monsterSpawnEffectPrefab;

    [Header("[UI]")]
    [SerializeField]
    private TextMeshProUGUI textWave;

    private bool isStarted = false;
    private static bool skipWaveDeltaTime = false;
    private MonsterSpawnControlInfo controlInfo;

    private MonsterSpawnTrigger currentTrigger = null;

    public static void SkipWaveDeltaTime()
    {
        // 어빌리티를 선택했을 경우
        skipWaveDeltaTime = true;
    }

    public void Spawn(MonsterSpawnControlInfo info)
    {
        if (info.monsterSpawnWaves.Length == 0) return;

        MonsterSpawnWave wave = info.monsterSpawnWaves[0];
        List<Vector2> targetTilePositions = info.targetTilePositions;

        StartCoroutine(SpawnOnce(wave, targetTilePositions));
    }

    public void StartSpawn(MonsterSpawnControlInfo info, MonsterSpawnTrigger trigger)
    {
        if (isStarted) return;
        controlInfo = info;
        isStarted = true;
        currentTrigger = trigger;

        StartCoroutine(SpawnRoutine(info));
    }

    private IEnumerator SpawnRoutine(MonsterSpawnControlInfo info)
    {
        int minWaveCnt = Mathf.Min(Mathf.Min(info.minWaveCount, info.maxWaveCount), info.monsterSpawnWaves.Length);
        int maxWaveCnt = Mathf.Min(Mathf.Max(info.minWaveCount, info.maxWaveCount), info.monsterSpawnWaves.Length);
        int waveCnt = Random.Range(minWaveCnt, maxWaveCnt + 1);

        onStartStageEvent.Invoke(info.stageIndex);

        // stageDelayTimeAtStart초 만큼 쉬면서
        // 스테이지 시작 UI 활성화
        textWave.gameObject.SetActive(true);
        textWave.text = $"<size=40>STAGE {info.stageIndex + 1}</size>\nSTART!";
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
        onClearStageEvent.Invoke(info.stageIndex);
        isStarted = false;
    }

    private IEnumerator WaveRoutine(MonsterSpawnWave wave, List<Vector2> targetTilePositions)
    {
        List<Vector2> allSpawnableTilePositions = new List<Vector2>(targetTilePositions);
        List<Vector2> spawnableTilePositions = new List<Vector2>();
        List<GameObject> spawnedMonsterList = new List<GameObject>();

        // 이펙트 생성
        foreach (MonsterSpawnWaveUnit wUnit in wave.waveUnits)
        {
            if (!wUnit.Monster) continue;

            int count = wUnit.getRandomSpawnCount();
            for (int i = 0; i < count; i++)
            {
                if (allSpawnableTilePositions.Count == 0) break;

                // 생성할 위치 지정
                int idx = Random.Range(0, allSpawnableTilePositions.Count);
                Vector3 pos = allSpawnableTilePositions[idx];
                spawnableTilePositions.Add(pos);

                allSpawnableTilePositions.RemoveAt(idx);

                // 이펙트 생성
                ObjectPooler.SpawnFromPool(monsterSpawnEffectPrefab.name, pos, Quaternion.identity);
            }
        }

        // 이펙트 사라질 때까지 기다림
        yield return new WaitForSeconds(1.5f);

        // 몬스터 생성
        foreach (MonsterSpawnWaveUnit wUnit in wave.waveUnits)
        {
            if (!wUnit.Monster) continue;

            SpawnMonster(wUnit.Monster.gameObject.name, wUnit.getLastSpawnCount(), ref spawnableTilePositions, ref spawnedMonsterList);
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

            Monster monster = ObjectPooler.SpawnFromPool<Monster>(monsterName, pos, Quaternion.identity);
            monster.SetMonsterSpawnTrigger(currentTrigger);
            spawnedMonsterList.Add(monster.gameObject);
        }
    }

    private IEnumerator SpawnOnce(MonsterSpawnWave wave, List<Vector2> targetTilePositions)
    {
        List<Vector2> allSpawnableTilePositions = new List<Vector2>(targetTilePositions);
        List<Vector2> spawnableTilePositions = new List<Vector2>();
        List<GameObject> spawnedMonsterList = new List<GameObject>();

        // 이펙트 생성
        foreach (MonsterSpawnWaveUnit wUnit in wave.waveUnits)
        {
            if (!wUnit.Monster) continue;

            int count = wUnit.getRandomSpawnCount();
            for (int i = 0; i < count; i++)
            {
                if (allSpawnableTilePositions.Count == 0) break;

                // 생성할 위치 지정
                int idx = Random.Range(0, allSpawnableTilePositions.Count);
                Vector3 pos = allSpawnableTilePositions[idx];
                spawnableTilePositions.Add(pos);

                allSpawnableTilePositions.RemoveAt(idx);

                // 이펙트 생성
                ObjectPooler.SpawnFromPool(monsterSpawnEffectPrefab.name, pos, Quaternion.identity);
            }
        }

        // 이펙트 사라질 때까지 기다림
        yield return new WaitForSeconds(1.5f);

        // 몬스터 생성
        foreach (MonsterSpawnWaveUnit wUnit in wave.waveUnits)
        {
            if (!wUnit.Monster) continue;

            SpawnMonster(wUnit.Monster.gameObject.name, wUnit.getLastSpawnCount(), ref spawnableTilePositions, ref spawnedMonsterList);
        }
    }
}

[System.Serializable]
public class MonsterSpawnControlInfo
{
    public int stageIndex;
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

    private int count = -1;

    public int getLastSpawnCount()
    {
        if (count != -1) return count;

        return getRandomSpawnCount();
    }

    public int getRandomSpawnCount()
    {
        int min = Mathf.Min(minMonsterCount, maxMonsterCount);
        int max = Mathf.Max(minMonsterCount, maxMonsterCount);

        count = Random.Range(min, max + 1);

        return count;
    }
}
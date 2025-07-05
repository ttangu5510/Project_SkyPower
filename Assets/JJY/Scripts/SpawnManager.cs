using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using KYG_skyPower;
using Unity.VisualScripting;

public class SpawnManager : MonoBehaviour
{
    private int currentSequenceLevel = 0;
    public int CurSeqLevel
    {
        get { return currentSequenceLevel; }
        set
        {
            currentSequenceLevel = value;
            hud.curSeq = value;
            hud.onSeqChanged?.Invoke(value);
        }
    }
    static public int enemyCount = 0;
    [SerializeField] private SpawnSequenceEnemy spawnSequenceEnemy;
    [SerializeField] private ObjectPool[] objectPools;
    [SerializeField] private HUDPresenter hud;
    private Dictionary<EnemyType, ObjectPool> poolDic;
    private Coroutine playRoutine;
    void Awake()
    {
        poolDic = new Dictionary<EnemyType, ObjectPool>();

        foreach (ObjectPool pool in objectPools)
        {
            if (poolDic.ContainsKey(pool.enemyType))
            {
                // Debug.LogError($"중복된 EnemyType이 ObjectPool에 있습니다: {pool.enemyType}");
                continue;
            }
            poolDic.TryAdd(pool.enemyType, pool); // pool 스크립트에 public EnemyType enemyType; 정의필요.
        }
    }

    private void Start()
    {
        playRoutine = StartCoroutine(PlayStage());
    }

    private IEnumerator PlayStage()
    {
        StageEnemyData currentStage = Manager.SDM.runtimeData[Manager.Game.selectWorldIndex - 1].subStages[Manager.Game.selectStageIndex - 1].stageEnemyData;

        // 시퀀스가 종료될 때까지 반복
        for (CurSeqLevel = 0; CurSeqLevel < currentStage.sequence.Count; CurSeqLevel++)
        {
            EnemySpawnInfo sequence = currentStage.sequence[CurSeqLevel];

            yield return StartCoroutine(spawnSequenceEnemy.SpawnSequence(sequence));

            // 현재 시퀀스의 모든 적이 처치될 때까지 반복
            while (enemyCount > 0) yield return null;

            Debug.Log($"Sequence {CurSeqLevel} cleared!");
        }

        // 시퀀스 모두 클리어 시, 보스 1마리 스폰
        Debug.Log("Boss appears!");
        GameObject bossObj = Instantiate(currentStage.bossPrefab, currentStage.bossSpawnPos, Quaternion.Euler(0, 180f, 0));
        Enemy enemy = bossObj.GetComponent<Enemy>();
        EnemyType type = enemy.enemyData.enemyType;

        if (poolDic.TryGetValue(type, out ObjectPool pool))
        {
            enemy.Init(pool);
        }
        enemyCount++;
        Debug.Log($"Total Enemies: {enemyCount}");

        // 보스 소환 후, 보스가 처치될 때까지 안벗어남
        while (enemyCount > 0) yield return null;

        CompleteStage();
        StopCoroutine(playRoutine);
        playRoutine = null;
    }
    private void CompleteStage()
    {
        Manager.Game.SetGameClear();

    }
}

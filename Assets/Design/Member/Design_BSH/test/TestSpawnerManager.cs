using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;

namespace JJY_Test
{
    public class TestSpawneManager : MonoBehaviour
    {
        private int curSequenceLevel = 0;
        static public int enemyCount = 0;
        [SerializeField] private TestSpawnSeqence spawnSequenceEnemy;
        [SerializeField] private ObjectPool[] objectPools;
        private Dictionary<EnemyType, ObjectPool> poolDic;
        private Coroutine playRoutine;
        [SerializeField] StageEnemyData currentStage;
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
            // 시퀀스가 종료될 때까지 반복
            for (curSequenceLevel = 0; curSequenceLevel < currentStage.sequence.Count; curSequenceLevel++)
            {
                EnemySpawnInfo sequence = currentStage.sequence[curSequenceLevel];

                yield return StartCoroutine(spawnSequenceEnemy.SpawnSequence(sequence));

                // 현재 시퀀스의 모든 적이 처치될 때까지 반복
                while (enemyCount > 0) yield return null;

                Debug.Log($"Sequence {curSequenceLevel} cleared!");
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
            TestSpawneManager.enemyCount++;
            Debug.Log($"Total Enemies: {enemyCount}");

            // 보스 소환 후, 보스가 처치될 때까지 안벗어남
            while (enemyCount > 0) yield return null;

            Debug.Log("All Stages Complete!");

            StopCoroutine(playRoutine);
            playRoutine = null;
        }
    }
}

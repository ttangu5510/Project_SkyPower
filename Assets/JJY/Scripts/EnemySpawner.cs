using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnDelay = 2f; // 적 소환 간격
    public ObjectPool objectPool;
    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }
    public IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            GameObject enemyobj = Instantiate(enemyPrefabs[i], transform.position, Quaternion.identity);
            enemyobj.transform.SetParent(transform);
            Enemy enemy = enemyobj.GetComponent<Enemy>();
            enemy.objectPool = objectPool;
            yield return new WaitForSeconds(spawnDelay);
        }
        // 다 소환하면 그 이후엔 어떻게 할 것인가?
        // 앞 스테이지 몬스터가 다 죽지 않았으면, 스폰 정지. => 몬스터가 다 죽으면 스폰 이어하기.
        // if (enemyCount < enemyPrefabs.Length)
        // {
        //     for (int i = 0; i < enemyPrefabs.Length; i++)
        //     {
        //         GameObject enemy = Instantiate(enemyPrefabs[i], transform.position, Quaternion.identity);
        //         enemy.transform.SetParent(transform);
        //         yield return new WaitForSeconds(spawnDelay);
        //         enemyCount++;   
        //     }
        // }
        // 다 소환하고, 현재 맵에 적이 없으면(또는 처치 시 enemyCount--;) enemyCount = 0;으로 초기화
        // 
        // or 
        //
        // 이 스포너를 여러개 만들기.
    }
}

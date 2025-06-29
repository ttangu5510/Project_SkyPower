using System;
using JYL;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public EnemyDropItemData dropItem;
    [SerializeField] private int currentHP;
    public bool isMoving = false; // 몬스터는 맵 밖에서 소환되어, 특정 위치로 애니메이터를 통해 이동된다. 이동중에는 공격을 하면 안되기 때문에 공격은 isMoving이 false일 때만 기능한다.
    // private float fireTimer;
    public Transform firePoint;
    public static event Action<Vector3> OnEnemyDied; // 죽었을 때 사용되는 이벤트
    public BulletPatternData BulletPattern;
    private Coroutine curFireCoroutine;

    // Enemy의 특성대로 총알 속도와 발사 간격을 조절.
    public float bulletSpeed = 10f;
    // public float fireDelay = 0.5f;

    void Start()
    {
        currentHP = enemyData.maxHP;
        Fire();
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
        // TODO : Sprite 색 변경
    }
    private void Update()
    {
        if (isMoving) return; // 이동중에는 발사하지 않음. => 애니메이터에서 움직임을 구현할 때, false로 바꿔줘야함.
    }
    private void Fire()
    {
        curFireCoroutine = StartCoroutine(BulletPattern.Shoot(firePoint, enemyData.bulletPrefab, bulletSpeed));
    }
    private void Die()
    {
        // 여기도 GameManager에서 이벤트
        OnEnemyDied?.Invoke(transform.position);
        // GameManager에서는 죽었다는 이벤트를 받아서 => 아이템 드롭 => 아이템 먹으면 점수 증가
        StopCoroutine(curFireCoroutine);
        // 죽는 애니메이션 실행.
        Destroy(gameObject);
    }
}

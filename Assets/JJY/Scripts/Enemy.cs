using JYL;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public static event Action<Vector3> OnEnemyDied;

    [Header("Enemy State")]
    public EnemyData enemyData;
    [SerializeField] private int currentHP;
    public bool autoFire;

    [Header("Enemy Shot Info")]
    public Transform[] firePoints;
    public BulletPatternData[] BulletPattern;
    private Coroutine curFireCoroutine;
    public ObjectPool curObjectPool;
    public float bulletSpeed = 1f;
    public float fireDelay = 1.5f;

    [Header("Hit Animation")]
    private Renderer modelRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    [SerializeField] private Color flashColor = Color.white; // 피격 시 변경될 색상
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Animator")]
    private Animator animator;

    void Awake()
    {
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            originalColor = modelRenderer.material.color;
        }
        animator = GetComponent<Animator>();
    }
    public void Init(ObjectPool objectPool)
    {
        curObjectPool = objectPool;
        currentHP = enemyData.maxHP;
        // autoFire = true;
        // if (autoFire) StartCoroutine(ChangeFireMode());
    }
    public void TakeDamage(int damage)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashEffectCoroutine());

        currentHP -= damage;
        if (currentHP <= 0) Die();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AnimationFire();
        }
    }
    private void Die()
    {
        SpawnManager.enemyCount--;

        OnEnemyDied?.Invoke(transform.position);

        if (curFireCoroutine != null)
        {
            StopCoroutine(curFireCoroutine);
            curFireCoroutine = null;
        }
        // TODO : 죽는 애니메이션 실행.
        // animator.SetBool("IsDead", true);
        Destroy(gameObject);
    }
    IEnumerator ChangeFireMode()
    {
        while (autoFire)
        {
            int ranNum = UnityEngine.Random.Range(0, BulletPattern.Length);
            curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, bulletSpeed, curObjectPool));
            yield return new WaitForSeconds(fireDelay);
            StopCoroutine(curFireCoroutine);
            curFireCoroutine = null;
        }
    }
    private IEnumerator FlashEffectCoroutine()
    {
        modelRenderer.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        modelRenderer.material.color = originalColor;

        flashCoroutine = null;
    }
    public void AnimationFire()
    {
        // Debug.Log("지금 공격함");
        // int ranNum = UnityEngine.Random.Range(0, BulletPattern.Length);
        // curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, bulletSpeed, curObjectPool));

        Debug.Log("AnimationFire 호출됨. 현재 autoFire: " + autoFire);
        if (!autoFire) // 현재 발사 중이 아니라면 (자동 발사 시작)
        {
            autoFire = true;
            StartAutoFire();
        }
        else // 현재 발사 중이라면 (자동 발사 중지)
        {
            autoFire = false; 
            StopAutoFire();
        }
    }
    private void StartAutoFire()
    {
        if (curFireCoroutine == null) // 이미 실행 중이 아니라면 시작
        {
            curFireCoroutine = StartCoroutine(ChangeFireMode());
        }
    }

    // 자동 발사 중지
    private void StopAutoFire()
    {
        if (curFireCoroutine != null) // 실행 중이라면 중지
        {
            StopCoroutine(curFireCoroutine);
            curFireCoroutine = null;
        }
    }
}

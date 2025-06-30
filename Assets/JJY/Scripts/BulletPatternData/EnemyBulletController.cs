using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public Rigidbody rb;
    public ObjectPool objectPool;
    public Enemy enemy;
    void Awake()
    {
        Init();
    }
    void Init()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        objectPool = enemy.objectPool;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            // objectPool.ReturnToPool(this);
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KYG_skyPower;

public class EnemyItem : MonoBehaviour
{
    // public EnemyDropItemData dropItem;

    // 자력 효과
    private Transform player;
    public float magneticRange = 3f;
    public float magneticSpeed = 5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.Log("EnemyItem : Player가 할당되지 않음.");
        }
    }
    void Update()
    {
        if (player == null)
        {
            Debug.Log("EnemyItem : Player가 할당되지 않음.");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= magneticRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, magneticSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }
    void Collect()
    {
        // ScoreManager.Instance.AddScore(1);
        Debug.Log($"{ScoreManager.Instance.Score}");
        Destroy(gameObject);
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemManager : MonoBehaviour
{
    // 아이템 드롭 매니저
    public GameObject itemPrefab;
    public int itemCount = 40; // 아이템 개수
    public float itemMoveLimitRange = 2f; // 아이템이 떨어지는 반경 제한
    public float dropItemSpeed = 0.5f; // 아이템이 퍼져나가는 속력 , 아이템이 떨어지는 속력.
    public float itemStopTime = 1f; // 몇 초 뒤에 멈출것인가.

    void OnEnable()
    {
        Enemy.OnEnemyDied += HandleEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDied -= HandleEnemyDeath;
    }
    void HandleEnemyDeath(Vector3 position)
    {
        StartCoroutine(DropItems(position));
    }
    IEnumerator DropItems(Vector3 position)
    {
        // 코루틴 이후 멈추기위해 Rigidbody들을 담을 리스트.
        List<Rigidbody> itemRbs = new List<Rigidbody>();

        for (int i = 0; i < itemCount; i++)
        {
            float angle = i * (360f / itemCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            GameObject item = Instantiate(itemPrefab, position, Quaternion.identity);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            itemRbs.Add(rb);
            rb.AddForce(dir * dropItemSpeed, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(itemStopTime);

        foreach (Rigidbody rb in itemRbs)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(new Vector3(dropItemSpeed, 0, 0), ForceMode.Impulse);
        }
    }
}

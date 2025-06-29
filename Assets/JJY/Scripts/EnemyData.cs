using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public int maxHP; // 몬스터 종류에 따라 최대 체력이 다른가?
    public Sprite enemyIcon;
    public GameObject bulletPrefab; // 몬스터가 발사하는 총알 프리팹. => BulletPatternData를 상속받은 스크립트로 변경할 것.

    // Binary Tree Pattern을 여기서 어떻게 사용하나?

    // 움직임은 Animator로 만들것. => Player기준 MainCamera의 위치 정보 필요.
}
public class EnemyDropItemData : ScriptableObject
{
    [Header("Enemy Drop Item")]
    public string itemName;
    public Sprite itemIcon; // 인게임 내에서 보일 모습.
    public float dropRate;
}

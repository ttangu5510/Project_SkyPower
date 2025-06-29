using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public string EnemyName;
    public int maxHP; // 몬스터 종류에 따라 최대 체력이 다른가?

    // Binary Tree Pattern을 여기서 어떻게 사용하나?

    // 움직임은 Animator로 만들것. => Player기준 MainCamera의 위치 정보 필요.
}
public class EnemyDropItemData : ScriptableObject
{
    [Header("Enemy Drop Item")]
    public string itemName;
    public Sprite itemIcon; // 인게임 내에서 보일 모습.
    public float dropRate;

    // 플레이어는 주변 아이템을 자력으로 흡수한다. => 아이템은 플레이어에게 어떻게 다가갈 것인가?
}

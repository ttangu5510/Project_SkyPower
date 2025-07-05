using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

[System.Serializable]
public partial class GameData : SaveData
{
    public string playerName;
    public int gold;

    public CharacterInventory characterInventory;
    public StageInfo[] stageInfo;
    public EquipSave[] equipInfo;
    public int[] wearingId;
    public bool isEmpty => string.IsNullOrEmpty(playerName);

    public GameData()
    {
        // Initialize character inventory
        characterInventory = new CharacterInventory();
        gold = 1000;
        wearingId = new int[3];
    }

}

// 직렬화를 해줘야 불러올 수 있음. json이 직렬화 저장방식이라 그러함.
[System.Serializable]
public struct StageInfo
{
    public int world;
    public int stage;
    public int score;
    public bool unlock;
    public bool isClear;
}
[System.Serializable]
public struct EquipSave
{
    public int id;
    public int level;
}
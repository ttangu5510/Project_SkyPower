using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
[CreateAssetMenu(fileName = "EquipmentDataSO", menuName = "Equipment/EquipmentDataSO")]
public class EquipmentDataSO : ScriptableObject
{
    public int id; 
    public string equipName;
    public EquipType type;
    public EquipGrade grade;
    public SetType setType;
    public Sprite icon;
    public int level;
    public int maxLevel;
    public int upgradeGold;
    public int upgradeGoldPlus;
    public int equipValue;
    public int equipValuePlus;
    public string Effect_Desc;
}
    public enum EquipType
    {
        Weapon,Armor,Accessory
    }
    public enum EquipGrade
    {
        Normal, Legend
    }
    public enum SetType
    { 
        충전의,응급의,전장의,맹공의,광속의
    }
}

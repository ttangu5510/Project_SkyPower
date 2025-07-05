using KYG_skyPower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
// 모든 장비를 한 번에 관리하는 컨테이너
[CreateAssetMenu(fileName = "EquipmentTableSO", menuName = "Equipment/EquipmentTableSO")]
public class EquipmentTableSO : ScriptableObject
{
    public List<EquipmentDataSO> equipmentList = new List<EquipmentDataSO>();
}
}
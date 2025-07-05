using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    public class EquipmentInvenManager : MonoBehaviour
    {
        public static EquipmentInvenManager Instance { get; private set; }

        public EquipmentTableSO equipmentTableSO; // 자료가 있는 SO들의 리스트

        // 캐릭터별 장착 슬롯
        private Dictionary<int, CharacterEquipmentSlots> characterEquipSlots = new Dictionary<int, CharacterEquipmentSlots>();

        //public List<EquipmentController> inventory = new List<EquipmentDataSO>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        // 타입별(무기/방어구/악세서리) 리스트 반환
        //public List<EquipmentDataSO> GetItemList(string equipType)
        //{
        //    //return equipmentTableSO.equipmentList.FindAll(e => e.Equip_Type == equipType);
        //}

        // 캐릭터 장비 장착
        public void EquipItem(int characterId, EquipmentDataSO equipment)
        {
            if (!characterEquipSlots.ContainsKey(characterId))
                characterEquipSlots[characterId] = new CharacterEquipmentSlots();

            //switch (equipment.Equip_Type)
            //{
            //    case "weapon": characterEquipSlots[characterId].weapon = equipment; break;
            //    case "armor": characterEquipSlots[characterId].armor = equipment; break;
            //    case "accessory": characterEquipSlots[characterId].accessory = equipment; break;
            //}
        }

        // 캐릭터 장착 정보 반환
        public CharacterEquipmentSlots GetEquippedItems(int characterId)
        {
            if (!characterEquipSlots.ContainsKey(characterId))
                characterEquipSlots[characterId] = new CharacterEquipmentSlots();
            return characterEquipSlots[characterId];
        }

        //public void AddToInventory(EquipmentDataSO equip)
        //{
        //    무기1, 무기1
        //    if (!inventory.Contains(equip)) inventory.Add(equip);
        //}
        //public void RemoveFromInventory(EquipmentDataSO equip)
        //{
        //    if (inventory.Contains(equip)) inventory.Remove(equip);
        //}
    }

    [System.Serializable]
    public class CharacterEquipmentSlots
    {
        public EquipmentDataSO weapon;
        public EquipmentDataSO armor;
        public EquipmentDataSO accessory;
    }
}

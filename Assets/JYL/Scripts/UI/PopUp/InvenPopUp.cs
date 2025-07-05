using KYG_skyPower;
using LJ2;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JYL
{
    public class InvenPopUp : BaseUI
    {
        // 게임매니저에서 캐릭터 정보를 불러와야 함
        private GameObject invenScroll => GetUI("InvenScroll");
        private TMP_Text invenCharName => GetUI<TMP_Text>("InvenCharNameText");
        private TMP_Text level => GetUI<TMP_Text>("InvenCharLevelText");
        private TMP_Text hp => GetUI<TMP_Text>("InvenCharHPText");
        private TMP_Text ap => GetUI<TMP_Text>("InvenCharAPText");
        private Image charImage;
        private CharacterSaveLoader characterLoader;
        private CharactorController mainController=>characterLoader.mainController;

        private new void Awake()
        {
            base.Awake();
            characterLoader = GetComponent<CharacterSaveLoader>();
            Init();
            
        }
        private void OnEnable()
        {
            Init();
        }
        void Start()
        {
            
            // 장비 클릭시 활성화

            invenScroll.SetActive(false);
            GetEvent("CharEnhanceBtn").Click += OpenCharEnhance;
            //GetEvent("WeaponBtn").Click += OpenWPInven;
            GetEvent("WPEnhanceBtn1").Click += OpenWPEnhance;
            //GetEvent("ArmorBtn").Click += OpenAMInven;
            GetEvent("AMEnhanceBtn2").Click += OpenAMEnhance;
            //GetEvent("AccessoryBtn").Click += OpenACInven;

            // 현재 캐릭터의 정보가 표시된다
            // index는 UIManager가 관리
            // GameManager.Instance.character[index]

        }
        private void Init()
        {
            characterLoader.GetCharPrefab();
            charImage = GetUI<Image>("InvenCharImage");
            charImage.sprite = mainController.image;
            Debug.Log($"{invenCharName}_{invenCharName.GetType()}_{invenCharName.GetType().Name}");
            Debug.Log($"{mainController.charName}");
            Debug.Log($"{invenCharName.text} : {mainController.charName}");
            invenCharName.text = $"{mainController.charName}";
            level.text = $"{mainController.level}";
            hp.text = $"{mainController.Hp}";
            ap.text = $"{mainController.attackDamage}";
        }
        private void OpenCharEnhance(PointerEventData eventData)
        {
            // 캐릭터 정보를 가지고 강화창 구현
            // UIManager에서 선택된 캐릭터의 인덱스 가지고 GameManager의 파티 구성원의 정보에 대한 캐릭터 컨트롤러 정보 불러옴
            // 해당 정보는 강화창에서 불러옴 여기서 안불러옴
            UIManager.Instance.selectIndexUI = 1;
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
            // UI 생성할 때, UI에다가 이벤트 다세요.
            // Image img = Instantiate();
            // GetEvent($"img.gameObject.name").Click += 이벤트함수;
        }

        private void OpenWPEnhance(PointerEventData eventData)
        {
            UIManager.Instance.selectIndexUI = 2;
            // 현재 무기의 정보를 가져가야함
            // 선택하는 UI 정보들은 UIManager를 통해 접근한다.
            // GameManager.Instance.Party[0].
            // UIManager.Instance. 현재 선택한 캐릭의정보 + 무기 -> Enhance 팝업이 불러와야 함
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
        }

        private void OpenAMEnhance(PointerEventData eventData)
        {
            UIManager.Instance.selectIndexUI = 3;
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
        }

        //private void OpenWPInven(PointerEventData eventData)
        //{
        //    invenScroll.SetActive(true);
        //    foreach (Transform child in invenScroll.transform)
        //        //Destroy(child.gameObject);

            // 무기 리스트 불러오기
            //var weaponList = EquipmentInvenManager.Instance.GetItemList("weapon");
            //Debug.Log("무기 개수: " + weaponList.Count);
            //GameObject itemSlotPrefab = Resources.Load<GameObject>("Inventory/WeaponSlot");

            //int idx = 0;
            //foreach (var weapon in weaponList)
            //{
            //    //Debug.Log($"슬롯 생성: {weapon.Equip_Name} ({idx++})");
            //    //GameObject slot = Instantiate(itemSlotPrefab, invenScroll.transform);
            //    //slot.GetComponentInChildren<TMP_Text>().text = weapon.Equip_Name;
            //    //slot.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Equipment/" + weapon.Equip_Img);

            //    //var capturedWeapon = weapon;
            //    //slot.GetComponent<Button>().onClick.AddListener(() =>
            //    //{
            //    //    // 선택 캐릭터에 장비 착용
            //    //    EquipmentInvenManager.Instance.EquipItem(mainController.id, capturedWeapon);
            //    //    mainController.ApplyEquipmentStat(); // (이 함수는 CharactorController에 추가 필요)
            //    //});
            //}
        //}
        
            // 인벤토리 매니저에서 무기로 타입 지정해서 정보들 가져옴
            // items = InvenManager.Instance.GetItemList(Type weapon)
            // items를 가지고 invenscroll 구현
            // 각 UI 요소들에 이벤트 달아야 함
            // foreach(Button ui in items)
            // { GetEvent("").Click += 아이템교체 함수 }
            // 해당 UI 클릭 시, 장착중이던 장비와 교환
            // InvenManager.Instance.Add()
            // InvenManager.Instance.RemoveAt(인벤ID값으로 찾아 지우기)
        

        // 아이템 교체 함수
        // 캐릭터 컨트롤러에 장비중인 아이템의 ID값 변경 필요

//        private void OpenAMInven(PointerEventData eventData)
//        {
//            invenScroll.SetActive(true);
//            foreach (Transform child in invenScroll.transform)
//                Destroy(child.gameObject);

//            var armorList = EquipmentInvenManager.Instance.GetItemList("armor");
//            GameObject itemSlotPrefab = Resources.Load<GameObject>("Inventory/ArmorSlot");

//            foreach (var armor in armorList)
//            {
//                GameObject slot = Instantiate(itemSlotPrefab, invenScroll.transform); 
//                //slot.GetComponentInChildren<TMP_Text>().text = armor.Equip_Name; 
//                //slot.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Equipment/" + armor.Equip_Img); 

//                var capturedArmor = armor; 
//                slot.GetComponent<Button>().onClick.AddListener(() =>
//                {
//                    EquipmentInvenManager.Instance.EquipItem(mainController.id, capturedArmor);
//                   // mainController.ApplyEquipmentStat();
//                });
//            }
//        }
//        private void OpenACInven(PointerEventData eventData)
//        {
//            invenScroll.SetActive(true);
//            foreach (Transform child in invenScroll.transform)
//                Destroy(child.gameObject);

//            var accessoryList = EquipmentInvenManager.Instance.GetItemList("accessory"); 
//            GameObject itemSlotPrefab = Resources.Load<GameObject>("Inventory/AccessorySlot"); 

//            foreach (var accessory in accessoryList)
//            {
//                GameObject slot = Instantiate(itemSlotPrefab, invenScroll.transform);
//                //slot.GetComponentInChildren<TMP_Text>().text = accessory.Equip_Name;
//                //slot.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Equipment/" + accessory.Equip_Img);

//                var capturedAccessory = accessory;
//                slot.GetComponent<Button>().onClick.AddListener(() =>
//                {
//                    EquipmentInvenManager.Instance.EquipItem(mainController.id, capturedAccessory);
//                   // mainController.ApplyEquipmentStat();
//                });
//            }
//        }
    }
}



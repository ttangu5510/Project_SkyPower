using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LJ2;
using TMPro;

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

        private void OnEnable()
        {
        }
        void Start()
        {
            
            // 장비 클릭시 활성화
            characterLoader = GetComponent<CharacterSaveLoader>();
            characterLoader.GetCharPrefab();
            Init();

            invenScroll.SetActive(false);
            GetEvent("CharEnhanceBtn1").Click += OpenCharEnhance;
            GetEvent("WeaponBtn").Click += OpenWPInven;
            GetEvent("WPEnhanceBtn2").Click += OpenWPEnhance;
            GetEvent("ArmorBtn").Click += OpenAMInven;
            GetEvent("AMEnhanceBtn3").Click += OpenAMEnhance;
            GetEvent("AccessoryBtn").Click += OpenACInven;

            // 현재 캐릭터의 정보가 표시된다
            // index는 UIManager가 관리
            // GameManager.Instance.character[index]
            invenCharName.text = "캐릭터1";
            level.text = "24";
            hp.text = "2040";
            ap.text = "332";
            Init();
        }
        private void Init()
        {
            charImage = GetUI<Image>("InvenCharImage");
            CharactorController charCont;
            foreach(CharactorController cont in characterLoader.charactorController)
            {
                if(cont.partySet == PartySet.Main)
                {
                    charCont = cont;
                    charImage.sprite = charCont.image;
                }
            }
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

        private void OpenWPInven(PointerEventData eventData)
        {
            invenScroll.SetActive(true);
            // 인벤토리 매니저에서 무기로 타입 지정해서 정보들 가져옴
            // items = InvenManager.Instance.GetItemList(Type weapon)
            // items를 가지고 invenscroll 구현
            // 각 UI 요소들에 이벤트 달아야 함
            // foreach(Button ui in items)
            // { GetEvent("").Click += 아이템교체 함수 }
            // 해당 UI 클릭 시, 장착중이던 장비와 교환
            // InvenManager.Instance.Add()
            // InvenManager.Instance.RemoveAt(인벤ID값으로 찾아 지우기)
        }

        // 아이템 교체 함수
        // 캐릭터 컨트롤러에 장비중인 아이템의 ID값 변경 필요

        private void OpenAMInven(PointerEventData eventData)
        {
            invenScroll.SetActive(true);
        }
        private void OpenACInven(PointerEventData eventData)
        {
            invenScroll.SetActive(true);
        }
    }
}



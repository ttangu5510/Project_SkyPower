using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace JYL
{
    public class EnhancePopUp : BaseUI
    {
        private Image enhanceTypeImg => GetUI<Image>("EnhanceMenuBack");
        [SerializeField] private Sprite charEnhanceImg;
        [SerializeField] private Sprite wpEnhanceImg;
        [SerializeField] private Sprite amEnhanceImg;
        // UIManager, GameManager 등을 퉁해 현재 켜진 강화 창이 캐릭강화창인지 장비 강화창인지 판별함
        // 캐릭 또는 장비에 재화 소모 후 Level을 올림.
        
        // 이 필드들은 선언하지말고 매니저에서 직접 가져온다.
        // private CharacterController CharacterController;
        // private Item item;

        void Start()
        {
            switch(UIManager.Instance.selectIndexUI)
            {
                case 0:
                    enhanceTypeImg.sprite = charEnhanceImg;
                    GetEvent("EnhanceBtn").Click += CharacterEnhance;
                    break;
                case 1:
                    enhanceTypeImg.sprite = wpEnhanceImg;
                    GetEvent("EnhanceBtn").Click += EquipEnhance;
                    break;
                case 2:
                    enhanceTypeImg.sprite = amEnhanceImg;
                    GetEvent("EnhanceBtn").Click += EquipEnhance;
                    break;

            }
            //GetEvent("EnhanceBtn").Click += data => item.level++; 또는 레벨증가 함수 수행
        }

        private void CharacterEnhance(PointerEventData eventData)
        {
            // TODO : 캐릭터 강화 구현
            // 재화가 충분할 시
             UIManager.Instance.ClosePopUp();
        }
        private void EquipEnhance(PointerEventData eventData)
        {
            // TODO : 장비 강화 구현
            // 재화가 충분할 시
            UIManager.Instance.ClosePopUp();
        }
    }
}


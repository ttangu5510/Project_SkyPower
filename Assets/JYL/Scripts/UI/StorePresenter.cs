using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using KYG_skyPower;

namespace JYL
{
    public class StorePresenter : BaseUI
    {

        void Start()
        {
            GetEvent("GachaChrBtn1").Click += CharacterGacha;
            GetEvent("GachaChrBtn5").Click += EquipmentGacha;
            GetEvent("GachaEquipBtn1").Click += CharacterGacha;
            GetEvent("GachaEquipBtn5").Click += EquipmentGacha;
            GetEvent("StoreItemImg").Click += ItemStore;
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log($"{PopUpUI.IsPopUpActive}, {Util.escPressed}");
            }
            if(Input.GetKeyDown(KeyCode.Escape)&&!PopUpUI.IsPopUpActive&&!Util.escPressed)
            {
                // 씬 전환
                SceneManager.LoadSceneAsync("bMainScene_JYL");
            }
        }
        
        private void CharacterGacha(PointerEventData eventData)
        {
            // TODO : 가챠 수행과 동시에, 인벤토리(캐릭터 목록)에 추가. 게임매니저에서 세이브함
            // 저장 완료 후, 가챠 연출 진행. 연출 완료후 결과 팝업 창을 띄움
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int num);
            switch (num)
            {
                case 1:
                    Manager.Game.CurrentSave.gold -= 200;
                    UIManager.Instance.ShowPopUp<GachaPopUp>();
                    break;
                case 5:
                    Manager.Game.CurrentSave.gold -= 500;
                    Manager.Game.SaveGameProgress();
                    UIManager.Instance.ShowPopUp<Gacha5PopUp>();
                    break;
            }
        }
        private void EquipmentGacha(PointerEventData eventData)
        {
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int num);
            switch (num)
            {
                case 1:
                    UIManager.Instance.ShowPopUp<GachaPopUp>();
                    break;
                case 5:
                    UIManager.Instance.ShowPopUp<Gacha5PopUp>();
                    break;
            }
        }

        // TODO : 아이템 추가 시 작업
        private void ItemStore(PointerEventData eventData)
        {

        }
    }
}



using KYG_skyPower;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace JYL
{
    public class SaveFilePanel : BaseUI
    {
        // 불러왔던 세이브 데이터 기준으로 데이터를 채운다
        GameData data;
        
        void Start()
        {
            data = Manager.Game.CurrentSave;
            GetUI<TMP_Text>("SaveFileData").text = $"{Manager.Game.CurrentSave.playerName}";
            GetEvent("SaveDelBtn").Click += OnDelClick;
            GetEvent("SaveStartBtn").Click += OnStartClick;
        }

        //세이브파일을 삭제
        private void OnDelClick(PointerEventData eventData)
        {
            Manager.Save.GameDelete(data, Manager.Game.currentSaveIndex + 1);
            UIManager.Instance.ClosePopUp();
            Manager.Game.ResetSaveRef();
        }

        //세이브 파일로 게임 시작
        private void OnStartClick(PointerEventData eventData)
        {
            // 씬 넘어감 -> mainScene
            // 이전 UI들로 인해서 세이브파일은 선택되어 있음.
            Manager.SDM.SyncRuntimeDataWithStageInfo();
            SceneManager.LoadSceneAsync("bMainScene_JYL");
        }

    }
}

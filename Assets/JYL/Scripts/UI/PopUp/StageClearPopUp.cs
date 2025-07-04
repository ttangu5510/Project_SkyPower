using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KYG_skyPower;

namespace JYL
{
    public class StageClearPopUp : BaseUI
    {
        private Button nextButton;
        // TODO : 해당 팝업은 게임 클리어 시점에서 팝업된다
        private void Start()
        {
            SetNextStageBtn();
            GetEvent("SCReBtn").Click += RestartStage;
            GetEvent("SCQuitBtn").Click += QuitStage;
        }
        private void SetNextStageBtn()
        {
            nextButton = GetUI<Button>("SCNextStageBtn");
            int worldIndex = Manager.Game.selectWorldIndex;
            int stageIndex = Manager.Game.selectStageIndex;
            if (stageIndex > 5)
            {
                worldIndex++;
                stageIndex = 1;
            }
            if (Manager.SDM.runtimeData[worldIndex].subStages[stageIndex] == null)
            {
                nextButton.interactable = false;
            }
            else
            {
                GetEvent("SCNextStageBtn").Click += NextStage;
            }
        }
        private void NextStage(PointerEventData eventData)
        {
            // 스테이지를 선택해서 로드하는 것과 같은 효과. 진행 상황 저장은 게임 클리어 시점에 자동으로 수행
            // 페이드인 페이드아웃 효과가 필요한데, 스테이지 매니저에서 해당 기능이 구현되어야 하지 않나 함
            Time.timeScale = 1.0f;
            UIManager.Instance.CleanPopUp();
            Manager.Game.selectStageIndex++;
            if(Manager.Game.selectStageIndex>5)
            {
                Manager.Game.selectWorldIndex++;
                Manager.Game.selectStageIndex = 1;
            }
            Manager.Score.RecordBestScore();
            Manager.Score.ResetScore();
            Manager.GSM.LoadGameSceneWithStage("dStageScene_JYL", Manager.Game.selectWorldIndex, Manager.Game.selectStageIndex);
        }
        private void RestartStage(PointerEventData eventData)
        {
            Time.timeScale = 1.0f;
            Manager.Score.ResetScore();
            Manager.GSM.LoadGameSceneWithStage("dStageScene_JYL",Manager.Game.selectWorldIndex,Manager.Game.selectStageIndex);
        }
        private void QuitStage(PointerEventData eventData)
        {
            Time.timeScale = 1.0f;
            Manager.Score.RecordBestScore();
            Manager.Score.ResetScore();
            Manager.GSM.LoadScene("bMainScene_JYL");
        }
    }

}

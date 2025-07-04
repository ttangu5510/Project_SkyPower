using KYG_skyPower;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace JYL
{
    public class StageInerSelectPopUp : BaseUI
    {
        private int stageNum = 5;
        private int worldNum;
        private int selectStageNum;
        private Button[] stageBtn;
        [SerializeField]private Sprite lockImg;
        [SerializeField] private Sprite unLockImg;
        
        void Start()
        {
            stageBtn = new Button[stageNum];
            SetStageButtons();
        }

        void Update() { }
        private void SetStageButtons()
        {
            worldNum = UIManager.Instance.selectIndexUI;
            for (int i = 0; i < stageNum; i++)
            {
                stageBtn[i] = GetUI<Button>($"StageBtn_{i + 1}");
                GetUI<TMP_Text>($"StageText_{i + 1}").text = $"Stage {worldNum + 1} - {i + 1}";
                if (Manager.SDM.runtimeData[worldNum].subStages[i].isUnlocked)
                {
                    stageBtn[i].GetComponent<Image>().sprite = unLockImg;
                    stageBtn[i].interactable = true;
                    GetEvent($"{stageBtn[i].gameObject.name}").Click += SetStageIndex;
                    GetEvent($"{stageBtn[i].gameObject.name}").Click += ChangeSceneToStage;
                }
                else
                {
                    stageBtn[i].GetComponent <Image>().sprite = lockImg;
                    stageBtn[i].interactable = false;
                }

            }
        }
        private void SetStageIndex(PointerEventData eventData)
        {
            Util.ExtractTrailNumber(eventData.pointerClick.gameObject.name, out selectStageNum);
            Manager.Game.selectWorldIndex = worldNum + 1;
            Manager.Game.selectStageIndex = selectStageNum;
        }
        private void ChangeSceneToStage(PointerEventData eventData)
        {
            UIManager.Instance.CleanPopUp();
            Manager.GSM.LoadGameSceneWithStage("dStageScene_JYL", Manager.Game.selectWorldIndex, Manager.Game.selectStageIndex);
        }
    }
}


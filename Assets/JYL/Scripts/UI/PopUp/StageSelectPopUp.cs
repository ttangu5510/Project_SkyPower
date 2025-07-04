using KYG_skyPower;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using YSK;

namespace JYL
{
    public class StageSelectPopUp : BaseUI
    {
        private List<StageRuntimeData> stageData;
        [SerializeField]private Sprite lockSprite;
        [SerializeField]private Sprite unLockSprite;
        // UIManager에 현재 선택한 월드의 인덱스를 참조할 필요가 있음.
        // 만약 해당 월드가 lock된 경우, 클릭을 막음. 해당 정보는 게임 매니저 또는 스테이지매니저, 씬매니저에 있음
        void Start()
        {
        }
        private void OnEnable()
        {
            stageData = Manager.SDM.runtimeData;
            for (int i = 0; i < stageData.Count; i++)
            {
                if (stageData[i].subStages[0].isUnlocked)
                {
                    GetUI<Image>($"Stage{i + 1}").sprite = unLockSprite;
                    GetEvent($"Stage{i + 1}").Click += OnStageClick;
                    if (i + 1 <= stageData.Count && !stageData[i + 1].subStages[0].isUnlocked)
                    {
                        GetUI($"World{i + 1}SelectIcon").gameObject.SetActive(true);
                    }
                }
                else
                {
                    GetUI<Image>($"Stage{i+1}").sprite = lockSprite;
                }
            }
        }
        void Update()
        {

        }

        private void OnStageClick(PointerEventData eventData)
        {
            Util.ExtractTrailNumber(eventData.pointerClick.gameObject.name, out int index);
            UIManager.Instance.selectIndexUI = index-1;
            UIManager.Instance.ShowPopUp<StageInerSelectPopUp>();

        }
    }
}


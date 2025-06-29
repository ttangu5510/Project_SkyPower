using UnityEngine;
using UnityEngine.UI;
using YSK;

namespace YSK
{
    /// <summary>
    /// 테스트용 UI (실제 씬 전환 사용)
    /// </summary>
    public class TestUI : MonoBehaviour
    {
        [Header("Test Buttons")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button stageSelectButton;
        [SerializeField] private Button gameButton;
        [SerializeField] private Button resultButton;
        [SerializeField] private Button quitButton;
        
        [Header("Stage Selection")]
        [SerializeField] private Button[] stageButtons = new Button[4]; // 4개의 스테이지 버튼
        
        #region Unity Lifecycle
        
        private void Start()
        {
            InitializeButtons();
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeButtons()
        {
            // 메인메뉴 버튼
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => GameSceneManager.Instance.LoadMainMenu());
            
            // 스테이지 선택 버튼
            if (stageSelectButton != null)
                stageSelectButton.onClick.AddListener(() => GameSceneManager.Instance.LoadStageSelect());
            
            // 게임 버튼
            if (gameButton != null)
                gameButton.onClick.AddListener(() => GameSceneManager.Instance.LoadGameScene(1));
            
            // 결과 버튼
            if (resultButton != null)
                resultButton.onClick.AddListener(() => GameSceneManager.Instance.LoadResultScene(1000, true));
            
            // 종료 버튼
            if (quitButton != null)
                quitButton.onClick.AddListener(() => GameSceneManager.Instance.QuitGame());
            
            // 스테이지 버튼들
            for (int i = 0; i < stageButtons.Length; i++)
            {
                int stageID = i + 1;
                if (stageButtons[i] != null)
                    stageButtons[i].onClick.AddListener(() => GameSceneManager.Instance.LoadGameScene(stageID));
            }
        }
        
        #endregion
    }
} 
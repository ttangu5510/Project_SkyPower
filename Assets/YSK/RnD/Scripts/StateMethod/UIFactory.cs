using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using YSK;
using System.Collections.Generic;

namespace YSK
{
    /// <summary>
    /// UI 생성 및 관리를 담당하는 팩토리 클래스
    /// 향후 UI 담당자가 실제 UI를 만들면 이 클래스는 초기화/활성화만 담당하게 됩니다.
    /// </summary>
    public class UIFactory : MonoBehaviour
    {
        [Header("Font Settings")]
        [SerializeField] private TMP_FontAsset notoSansKRFont; // Inspector에서 할당
        
        // 싱글톤 패턴
        public static UIFactory Instance { get; private set; }
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // 싱글톤 설정
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        #endregion
        
        #region UI Creation Methods
        
        /// <summary>
        /// 메인메뉴 UI를 생성합니다.
        /// </summary>
        public GameObject CreateMainMenuUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("MainMenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 1f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Sky Power";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 48;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.7f);
            titleRect.anchorMax = new Vector2(0.8f, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 버튼들 (순서: 메인스테이지 → 무한스테이지 → 상점 → 파티 → 게임종료)
            CreateButton(canvasObj, "MainStageButton", "메인스테이지", new Vector2(0.5f, 0.6f), () => {
                Debug.Log("메인스테이지 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainStageSelect();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });

            CreateButton(canvasObj, "EndlessStageButton", "무한스테이지", new Vector2(0.5f, 0.5f), () => {
                Debug.Log("무한스테이지 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadEndlessStage();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });

            CreateButton(canvasObj, "StoreButton", "상점", new Vector2(0.5f, 0.4f), () => {
                Debug.Log("상점 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadStore();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });

            CreateButton(canvasObj, "PartyButton", "파티", new Vector2(0.5f, 0.3f), () => {
                Debug.Log("파티 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadPartyScene();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });

            CreateButton(canvasObj, "QuitButton", "게임 종료", new Vector2(0.5f, 0.2f), () => {
                Debug.Log("게임 종료 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.QuitGame();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("메인메뉴 UI 생성 완료");
            return canvasObj;
        }
        
        /// <summary>
        /// 스테이지 선택 UI를 생성합니다.
        /// </summary>
        public GameObject CreateStageSelectUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("StageSelectCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 1f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "스테이지 선택";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.8f);
            titleRect.anchorMax = new Vector2(0.8f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 스테이지 버튼들
            for (int i = 1; i <= 4; i++)
            {
                int stageID = i;
                CreateButton(canvasObj, $"Stage{i}Button", $"Stage {i}", 
                    new Vector2(0.15f + (i-1) * 0.23f, 0.4f), 
                    () => {
                        Debug.Log($"스테이지 {stageID} 버튼 클릭됨");
                        if (GameSceneManager.Instance != null)
                        {
                            GameSceneManager.Instance.LoadGameScene(stageID);
                        }
                        else
                        {
                            Debug.LogError("GameSceneManager.Instance가 null입니다!");
                        }
                    });
            }
            
            // 뒤로가기 버튼
            CreateButton(canvasObj, "BackButton", "뒤로가기", new Vector2(0.5f, 0.1f), () => {
                Debug.Log("뒤로가기 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("스테이지 선택 UI 생성 완료");
            return canvasObj;
        }
        
        /// <summary>
        /// 게임 UI를 생성합니다.
        /// </summary>
        public GameObject CreateGameUI(int stageID = 1)
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("GameCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 게임 UI 패널
            GameObject panelObj = new GameObject("GamePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 1f);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.8f);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // 스테이지 정보 텍스트
            GameObject stageTextObj = new GameObject("StageText");
            stageTextObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform stageTextRect = stageTextObj.GetComponent<RectTransform>();
            if (stageTextRect == null)
            {
                stageTextRect = stageTextObj.AddComponent<RectTransform>();
            }
            
            stageTextRect.anchorMin = new Vector2(0.05f, 0.1f);
            stageTextRect.anchorMax = new Vector2(0.4f, 0.9f);
            stageTextRect.offsetMin = Vector2.zero;
            stageTextRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI stageText = stageTextObj.AddComponent<TextMeshProUGUI>();
            
            // 현재 선택된 스테이지 정보 가져오기
            int selectedMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int selectedSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            stageText.text = $"Stage {selectedMainStage}-{selectedSubStage}";
            
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                stageText.font = fontAsset;
            }
            
            stageText.fontSize = 18;
            stageText.color = Color.white;
            stageText.alignment = TextAlignmentOptions.Left;
            
            // 일시정지 버튼
            CreateButton(panelObj, "PauseButton", "일시정지", new Vector2(0.875f, 0.5f), () => {
                Debug.Log("일시정지 버튼 클릭됨");
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.TogglePause();
                }
                else
                {
                    Debug.LogError("GameStateManager.Instance가 null입니다!");
                }
            });
            
            // 결과 화면으로 넘어가는 버튼 (테스트용)
            CreateButton(panelObj, "ResultButton", "결과화면", new Vector2(0.875f, 0.3f), () => {
                Debug.Log("결과화면 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadResultScene(1500, true);
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log($"게임 UI 생성 완료 - 스테이지 {stageID}");
            return canvasObj;
        }
        
        /// <summary>
        /// 결과 UI를 생성합니다.
        /// </summary>
        public GameObject CreateResultUI(int score, bool isWin)
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("ResultCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 1f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 결과 제목
            GameObject resultTitleObj = new GameObject("ResultTitle");
            resultTitleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI resultTitleText = resultTitleObj.AddComponent<TextMeshProUGUI>();
            resultTitleText.text = isWin ? "승리!" : "패배...";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                resultTitleText.font = fontAsset;
            }
            
            resultTitleText.fontSize = 48;
            resultTitleText.color = isWin ? Color.yellow : Color.red;
            resultTitleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform resultTitleRect = resultTitleObj.GetComponent<RectTransform>();
            resultTitleRect.anchorMin = new Vector2(0.2f, 0.7f);
            resultTitleRect.anchorMax = new Vector2(0.8f, 0.9f);
            resultTitleRect.offsetMin = Vector2.zero;
            resultTitleRect.offsetMax = Vector2.zero;
            
            // 점수 표시
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
            scoreText.text = $"점수: {score}";
            
            // 폰트 설정
            TMP_FontAsset scoreFontAsset = LoadNotoSansKRFont();
            if (scoreFontAsset != null)
            {
                scoreText.font = scoreFontAsset;
            }
            
            scoreText.fontSize = 32;
            scoreText.color = Color.white;
            scoreText.alignment = TextAlignmentOptions.Center;
            
            RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.2f, 0.5f);
            scoreRect.anchorMax = new Vector2(0.8f, 0.65f);
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = Vector2.zero;
            
            // 버튼들
            int currentStageID = GameStateManager.Instance != null ? GameStateManager.Instance.CurrentStageID : 1;
            CreateButton(canvasObj, "RetryButton", "다시하기", new Vector2(0.3f, 0.3f), () => {
                Debug.Log("다시하기 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadGameScene(currentStageID);
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            CreateButton(canvasObj, "MainMenuButton", "메인메뉴", new Vector2(0.7f, 0.3f), () => {
                Debug.Log("메인메뉴 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log($"결과 UI 생성 완료 - 점수: {score}, 승리: {isWin}");
            return canvasObj;
        }
        
        /// <summary>
        /// 캐릭터 선택 UI를 생성합니다.
        /// </summary>
        public GameObject CreatePartyUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("PartyCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 1f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "캐릭터 선택";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.8f);
            titleRect.anchorMax = new Vector2(0.8f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 캐릭터 선택 버튼들 (3개)
            for (int i = 1; i <= 3; i++)
            {
                int charID = i;
                CreateButton(canvasObj, $"Character{i}Button", $"캐릭터 {i}", 
                    new Vector2(0.2f + (i-1) * 0.3f, 0.4f), 
                    () => {
                        Debug.Log($"캐릭터 {charID} 선택됨");
                        if (GameSceneManager.Instance != null)
                        {
                            GameSceneManager.Instance.LoadMainStageSelect();
                        }
                        else
                        {
                            Debug.LogError("GameSceneManager.Instance가 null입니다!");
                        }
                    });
            }
            
            // 뒤로가기 버튼
            CreateButton(canvasObj, "BackButton", "뒤로가기", new Vector2(0.5f, 0.1f), () => {
                Debug.Log("뒤로가기 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("캐릭터 선택 UI 생성 완료");
            return canvasObj;
        }
        
        /// <summary>
        /// 메인 스테이지 선택 UI를 생성합니다.
        /// </summary>
        public GameObject CreateMainStageSelectUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("MainStageSelectCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 1f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "메인 스테이지 선택";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.8f);
            titleRect.anchorMax = new Vector2(0.8f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 메인 스테이지 버튼들 (4개)
            for (int i = 1; i <= 4; i++)
            {
                int stageID = i;
                CreateButton(canvasObj, $"MainStage{i}Button", $"메인 스테이지 {i}", 
                    new Vector2(0.15f + (i-1) * 0.23f, 0.4f), 
                    () => {
                        Debug.Log($"메인 스테이지 {stageID} 선택됨");
                        if (GameSceneManager.Instance != null)
                        {
                            // 선택된 메인 스테이지 정보 저장
                            PlayerPrefs.SetInt("SelectedMainStage", stageID);
                            PlayerPrefs.Save();
                            GameSceneManager.Instance.LoadSubStageSelect();
                        }
                        else
                        {
                            Debug.LogError("GameSceneManager.Instance가 null입니다!");
                        }
                    });
            }
            
            // 뒤로가기 버튼
            CreateButton(canvasObj, "BackButton", "뒤로가기", new Vector2(0.5f, 0.1f), () => {
                Debug.Log("뒤로가기 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("메인 스테이지 선택 UI 생성 완료");
            return canvasObj;
        }
        
        /// <summary>
        /// 서브 스테이지 선택 UI를 생성합니다.
        /// </summary>
        public GameObject CreateSubStageSelectUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("SubStageSelectCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 1f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "서브 스테이지 선택";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.8f);
            titleRect.anchorMax = new Vector2(0.8f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 서브 스테이지 버튼들 (5개)
            for (int i = 1; i <= 5; i++)
            {
                int stageID = i;
                CreateButton(canvasObj, $"SubStage{i}Button", $"서브 스테이지 {i}", 
                    new Vector2(0.1f + (i-1) * 0.16f, 0.4f), 
                    () => {
                        Debug.Log($"서브 스테이지 {stageID} 선택됨");
                        if (GameSceneManager.Instance != null)
                        {
                            // 선택된 메인 스테이지 정보 가져오기 (기본값: 1)
                            int selectedMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
                            GameSceneManager.Instance.LoadBaseStage(selectedMainStage, stageID);
                        }
                        else
                        {
                            Debug.LogError("GameSceneManager.Instance가 null입니다!");
                        }
                    });
            }
            
            // 뒤로가기 버튼
            CreateButton(canvasObj, "BackButton", "뒤로가기", new Vector2(0.5f, 0.1f), () => {
                Debug.Log("뒤로가기 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainStageSelect();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("서브 스테이지 선택 UI 생성 완료");
            return canvasObj;
        }
        
        /// <summary>
        /// 3D 탄막 게임 UI를 생성합니다.
        /// </summary>
        public GameObject CreateBaseStageUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("BaseStageCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 게임 UI 패널
            GameObject panelObj = new GameObject("GamePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 1f);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.8f);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // 스테이지 정보 텍스트
            GameObject stageTextObj = new GameObject("StageText");
            stageTextObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform stageTextRect = stageTextObj.GetComponent<RectTransform>();
            if (stageTextRect == null)
            {
                stageTextRect = stageTextObj.AddComponent<RectTransform>();
            }
            
            stageTextRect.anchorMin = new Vector2(0.05f, 0.1f);
            stageTextRect.anchorMax = new Vector2(0.4f, 0.9f);
            stageTextRect.offsetMin = Vector2.zero;
            stageTextRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI stageText = stageTextObj.AddComponent<TextMeshProUGUI>();
            
            // 현재 선택된 스테이지 정보 가져오기
            int selectedMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int selectedSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            stageText.text = $"3D 탄막 게임\n스테이지 {selectedMainStage}-{selectedSubStage}";
            
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                stageText.font = fontAsset;
            }
            
            stageText.fontSize = 18;
            stageText.color = Color.white;
            stageText.alignment = TextAlignmentOptions.Left;
            
            // 일시정지 버튼
            CreateButton(panelObj, "PauseButton", "일시정지", new Vector2(0.875f, 0.5f), () => {
                Debug.Log("3D 탄막 게임 일시정지/재개 토글");
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.TogglePause();
                }
                else
                {
                    Debug.LogError("GameStateManager.Instance가 null입니다!");
                }
            });
            
            // 메인메뉴로 돌아가기 버튼
            CreateButton(panelObj, "MainMenuButton", "메인메뉴", new Vector2(0.875f, 0.3f), () => {
                Debug.Log("메인메뉴로 이동");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("3D 탄막 게임 UI 생성 완료");
            return canvasObj;
        }
        
        /// <summary>
        /// 무한 모드 UI를 생성합니다.
        /// </summary>
        public GameObject CreateEndlessStageUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("EndlessStageCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 게임 UI 패널
            GameObject panelObj = new GameObject("GamePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 1f);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.8f);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // 무한 모드 텍스트
            GameObject modeTextObj = new GameObject("ModeText");
            modeTextObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform modeTextRect = modeTextObj.GetComponent<RectTransform>();
            if (modeTextRect == null)
            {
                modeTextRect = modeTextObj.AddComponent<RectTransform>();
            }
            
            modeTextRect.anchorMin = new Vector2(0.05f, 0.1f);
            modeTextRect.anchorMax = new Vector2(0.4f, 0.9f);
            modeTextRect.offsetMin = Vector2.zero;
            modeTextRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI modeText = modeTextObj.AddComponent<TextMeshProUGUI>();
            modeText.text = "무한 모드";
            
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                modeText.font = fontAsset;
            }
            
            modeText.fontSize = 20;
            modeText.color = Color.white;
            modeText.alignment = TextAlignmentOptions.Left;
            
            // 일시정지 버튼
            CreateButton(panelObj, "PauseButton", "일시정지", new Vector2(0.875f, 0.5f), () => {
                Debug.Log("무한 모드 일시정지/재개 토글");
            });
            
            // 메인메뉴로 돌아가기 버튼
            CreateButton(panelObj, "MainMenuButton", "메인메뉴", new Vector2(0.875f, 0.3f), () => {
                Debug.Log("메인메뉴로 이동");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("무한 모드 UI 생성 완료");
            return canvasObj;
        }
        
        /// <summary>
        /// 상점 UI를 생성합니다.
        /// </summary>
        public GameObject CreateStoreUI()
        {
            // EventSystem 확인 및 생성
            EnsureEventSystemExists();
            
            // Canvas 생성
            GameObject canvasObj = new GameObject("StoreCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 1f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "상점";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.8f);
            titleRect.anchorMax = new Vector2(0.8f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 상점 아이템 버튼들 (예시)
            CreateButton(canvasObj, "Item1Button", "아이템 1 (100골드)", new Vector2(0.5f, 0.6f), () => {
                Debug.Log("아이템 1 구매");
            });
            
            CreateButton(canvasObj, "Item2Button", "아이템 2 (200골드)", new Vector2(0.5f, 0.5f), () => {
                Debug.Log("아이템 2 구매");
            });
            
            CreateButton(canvasObj, "Item3Button", "아이템 3 (300골드)", new Vector2(0.5f, 0.4f), () => {
                Debug.Log("아이템 3 구매");
            });
            
            // 뒤로가기 버튼
            CreateButton(canvasObj, "BackButton", "뒤로가기", new Vector2(0.5f, 0.1f), () => {
                Debug.Log("뒤로가기 버튼 클릭됨");
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameSceneManager.Instance가 null입니다!");
                }
            });
            
            Debug.Log("상점 UI 생성 완료");
            return canvasObj;
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// 스테이지 텍스트를 업데이트합니다.
        /// </summary>
        /// <param name="stageID">새로운 스테이지 ID</param>
        public void UpdateStageText(int stageID)
        {
            UpdateStageText($"Stage {stageID}");
        }
        
        /// <summary>
        /// 스테이지 텍스트를 업데이트합니다.
        /// </summary>
        /// <param name="stageText">표시할 스테이지 텍스트</param>
        public void UpdateStageText(string stageText)
        {
            // GameObject.Find 사용하여 StageText 찾기
            GameObject stageTextObj = GameObject.Find("StageText");
            if (stageTextObj != null)
            {
                TextMeshProUGUI stageTextComponent = stageTextObj.GetComponent<TextMeshProUGUI>();
                if (stageTextComponent != null)
                {
                    stageTextComponent.text = stageText;
                    Debug.Log($"스테이지 텍스트 업데이트: {stageText}");
                }
            }
            else
            {
                Debug.LogWarning("StageText 오브젝트를 찾을 수 없습니다! UI가 아직 생성되지 않았을 수 있습니다.");
            }
        }
        
        /// <summary>
        /// 버튼을 생성하는 헬퍼 메서드입니다.
        /// </summary>
        private void CreateButton(GameObject parent, string name, string text, Vector2 anchorPosition, System.Action onClick)
        {
            try
            {
                if (parent == null)
                {
                    Debug.LogError("CreateButton: parent가 null입니다!");
                    return;
                }
                
                // 1. 버튼 GameObject 생성
                GameObject buttonObj = new GameObject(name);
                buttonObj.transform.SetParent(parent.transform, false);
                
                // 2. RectTransform 설정
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                if (rectTransform == null)
                {
                    rectTransform = buttonObj.AddComponent<RectTransform>();
                }
                
                rectTransform.anchorMin = anchorPosition - new Vector2(0.1f, 0.05f);
                rectTransform.anchorMax = anchorPosition + new Vector2(0.1f, 0.05f);
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                
                // 3. Image 컴포넌트 추가 (버튼 배경)
                Image buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                
                // 4. Button 컴포넌트 추가
                Button button = buttonObj.AddComponent<Button>();
                
                // 5. Text GameObject 생성
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform, false);
                
                // 6. Text RectTransform 설정
                RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
                if (textRectTransform == null)
                {
                    textRectTransform = textObj.AddComponent<RectTransform>();
                }
                
                textRectTransform.anchorMin = Vector2.zero;
                textRectTransform.anchorMax = Vector2.one;
                textRectTransform.offsetMin = Vector2.zero;
                textRectTransform.offsetMax = Vector2.zero;
                
                // 7. Text 컴포넌트 추가
                TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
                buttonText.text = text;
                buttonText.fontSize = 18;
                buttonText.color = Color.white;
                buttonText.alignment = TextAlignmentOptions.Center;
                
                // 8. 폰트 설정
                TMP_FontAsset fontAsset = LoadNotoSansKRFont();
                if (fontAsset != null)
                {
                    buttonText.font = fontAsset;
                }
                
                // 9. Button의 targetGraphic 설정 (중요!)
                button.targetGraphic = buttonImage;
                
                // 10. 버튼 이벤트 설정 (안전한 방식)
                if (onClick != null)
                {
                    button.onClick.AddListener(() => {
                        try
                        {
                            Debug.Log($"버튼 클릭: {name} - {text}");
                            onClick();
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"버튼 클릭 오류 ({name}): {e.Message}");
                        }
                    });
                }
                
                Debug.Log($"버튼 생성 완료: {name} - {text}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CreateButton 오류 ({name}): {e.Message}");
            }
        }
        
        /// <summary>
        /// EventSystem이 존재하는지 확인하고, 없으면 생성합니다.
        /// </summary>
        private void EnsureEventSystemExists()
        {
            // 이미 EventSystem이 있는지 확인
            EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
            if (existingEventSystem == null)
            {
                Debug.Log("EventSystem이 없습니다. 생성합니다.");
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
            else
            {
                Debug.Log($"기존 EventSystem 발견: {existingEventSystem.name}");
            }
        }
        
        /// <summary>
        /// NotoSansKR 폰트를 로드하는 헬퍼 메서드입니다.
        /// </summary>
        private TMP_FontAsset LoadNotoSansKRFont()
        {
            // 방법 1: Inspector에서 할당된 폰트 사용
            if (notoSansKRFont != null)
            {
                return notoSansKRFont;
            }
            
            TMP_FontAsset fontAsset = null;
            
            // 방법 2: Resources 폴더에서 로드 시도
            fontAsset = Resources.Load<TMP_FontAsset>("RnD/Font/NotoSansKR-VariableFont_wght SDF");
            
            if (fontAsset == null)
            {
                fontAsset = Resources.Load<TMP_FontAsset>("Fonts/NotoSansKR-VariableFont_wght SDF");
            }
            
            if (fontAsset == null)
            {
                fontAsset = Resources.Load<TMP_FontAsset>("NotoSansKR-VariableFont_wght SDF");
            }
            
            if (fontAsset == null)
            {
                #if UNITY_EDITOR
                fontAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/YSK/RnD/Font/NotoSansKR-VariableFont_wght SDF.asset");
                #endif
            }
            
            if (fontAsset == null)
            {
                Debug.LogWarning("NotoSansKR 폰트를 찾을 수 없습니다. TMPro 기본 폰트를 사용합니다.");
                fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            }
            
            return fontAsset;
        }
        
        #endregion
    }
} 
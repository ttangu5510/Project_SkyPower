using KYG_skyPower;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace YSK
{
    /// <summary>
    /// 게임의 씬 전환을 관리하는 매니저 클래스입니다.
    /// </summary>
    public class GameSceneManager : Singleton<GameSceneManager>
    {
        [Header("Transition Settings")]
        [SerializeField] private bool enableTransition = false;
        [SerializeField] private bool showTransitionOnSceneStart;
        [SerializeField] private float minTransitionTime = 2f;

        [Header("Transition UI")]
        [SerializeField] private GameObject transitionScreenPrefab;
        [SerializeField] private string transitionPrefabPath = "YSK/UI/TransitionCanvas1";

        [Header("Transition Text Settings")]
        [SerializeField] private string[] transitionTexts = {
            "씬을 로딩 중입니다...",
            "잠시만 기다려주세요...",
            "곧 시작됩니다..."
        };
        
        [SerializeField] private float textChangeInterval = 0.8f;
        [SerializeField] private bool enableTextAnimation = true;

        [Header("Transition Font Settings")]
        [SerializeField] private TMP_FontAsset transitionFont;
        [SerializeField] private float fontSize = 56f;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;
        [SerializeField] private bool enableTextShadow = false;
        [SerializeField] private Color shadowColor = Color.black;
        [SerializeField] private Vector2 shadowOffset = new Vector2(1f, -1f);

        [Header("Transition Visual Settings")]
        [SerializeField] private Color transitionColor = Color.black;
        [SerializeField] private float fadeOutDuration = 1f;
        
        // 이벤트
        public static event Action<string> OnSceneLoadStarted;
        public static event Action<string> OnSceneLoadCompleted;
        public static event Action OnTransitionStarted;
        public static event Action OnTransitionCompleted;

        // 프로퍼티
        public bool IsLoading { get; private set; }
        public bool IsTransitioning { get; private set; }
        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        public bool IsTransitionEnabled => enableTransition;

        // 연출 화면 관련
        private GameObject transitionScreen;
        private CanvasGroup transitionCanvasGroup;
        private TextMeshProUGUI transitionText;
        private Image transitionBackground;
        private Coroutine textAnimationCoroutine;

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            // 전환 색상을 확실히 검은색으로 설정
            transitionColor = Color.black;
        }

        private void Start()
        {
            LoadTransitionPrefab();
            if (showTransitionOnSceneStart && enableTransition)
            {
                // 전환 화면 생성 및 표시
                ShowTransitionScreen();
                StartCoroutine(HideTransitionScreenAfterDelay(1f));
            }
        }



        #endregion

        #region Public API

        /// <summary>
        /// Unity의 기본 SceneManager.LoadScene을 사용합니다.
        /// </summary>
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
        public void LoadScene(int sceneBuildIndex) => SceneManager.LoadScene(sceneBuildIndex);

        /// <summary>
        /// 연출 효과와 함께 씬을 로드합니다.
        /// </summary>
        public void LoadGameScene(string sceneName, int mainStageID = 1, int subStageID = 1, int score = 0, bool isWin = true)
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
                StartCoroutine(LoadSceneAfterActivation(sceneName, mainStageID, subStageID, score, isWin));
                return;
            }

            HandleSceneSpecificData(sceneName, mainStageID, subStageID, score, isWin);
            StartCoroutine(LoadSceneWithTransition(sceneName));
        }

        /// <summary>
        /// 특정 스테이지와 함께 씬을 로드합니다.
        /// </summary>
        public void LoadGameSceneWithStage(string sceneName, int mainStageID, int subStageID)
        {
            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

            HandleSceneSpecificData(sceneName, mainStageID, subStageID, 0, false);
            StartCoroutine(LoadSceneWithTransitionAndStage(sceneName, mainStageID, subStageID));
        }

        /// <summary>
        /// 연출 효과 활성화/비활성화를 설정합니다.
        /// </summary>
        public void SetTransitionEnabled(bool enabled)
        {
            enableTransition = enabled;
            if (!enabled && transitionScreen != null) transitionScreen.SetActive(false);
        }

        /// <summary>
        /// 연출 화면을 수동으로 표시합니다.
        /// </summary>
        public void ShowTransitionScreen()
        {
            if (!enableTransition) 
            {
                Debug.Log("GameSceneManager: 전환 기능이 비활성화되어 전환 화면을 표시하지 않습니다.");
                return;
            }
            
            // 전환 화면이 없으면 생성
            if (transitionScreen == null)
            {
                CreateTransitionScreen();
            }
            
            if (transitionScreen == null) return;

            // 전환 화면 표시 전에 배경 색상 강제 설정
            Image[] allImages = transitionScreen.GetComponentsInChildren<Image>();
            foreach (var image in allImages)
            {
                // LoadingScreen의 배경 이미지인 경우에만 색상 변경
                if (image.gameObject.name == "LoadingScreen" || image.gameObject.name == "Image")
                {
                    image.color = transitionColor;
                    transitionBackground = image;
                }
            }

            transitionScreen.SetActive(true);
            if (transitionCanvasGroup != null) transitionCanvasGroup.alpha = 1f;
            if (enableTextAnimation && transitionText != null) StartTextAnimation();
            OnTransitionStarted?.Invoke();
            
            Debug.Log("GameSceneManager: 전환 화면 표시됨");
        }

        /// <summary>
        /// 연출 화면을 수동으로 숨깁니다.
        /// </summary>
        public void HideTransitionScreen()
        {
            // 전환 화면이 없으면 생성
            if (transitionScreen == null)
            {
                CreateTransitionScreen();
            }
            
            if (transitionScreen == null) return;
            StartCoroutine(FadeOutTransitionScreen());
        }

        /// <summary>
        /// 연출 텍스트를 설정합니다.
        /// </summary>
        public void SetTransitionText(string text)
        {
            if (transitionText != null) transitionText.text = text;
        }

        /// <summary>
        /// 연출 배경 색상을 설정합니다.
        /// </summary>
        public void SetTransitionColor(Color color)
        {
            transitionColor = color;
            if (transitionBackground != null) transitionBackground.color = color;
        }

        /// <summary>
        /// 전환 화면 폰트를 설정합니다.
        /// </summary>
        public void SetTransitionFont(TMP_FontAsset font)
        {
            transitionFont = font;
            if (transitionText != null)
            {
                transitionText.font = font;
            }
        }

        /// <summary>
        /// 전환 화면 폰트 크기를 설정합니다.
        /// </summary>
        public void SetTransitionFontSize(float size)
        {
            fontSize = size;
            if (transitionText != null)
            {
                transitionText.fontSize = size;
            }
        }

        /// <summary>
        /// 전환 화면 텍스트 색상을 설정합니다.
        /// </summary>
        public void SetTransitionTextColor(Color color)
        {
            textColor = color;
            if (transitionText != null)
            {
                transitionText.color = color;
            }
        }

        /// <summary>
        /// 전환 화면 텍스트 정렬을 설정합니다.
        /// </summary>
        public void SetTransitionTextAlignment(TextAlignmentOptions alignment)
        {
            textAlignment = alignment;
            if (transitionText != null)
            {
                transitionText.alignment = alignment;
            }
        }

        /// <summary>
        /// 전환 화면 텍스트 그림자를 설정합니다.
        /// </summary>
        public void SetTransitionTextShadow(bool enable, Color? shadowColor = null, Vector2? shadowOffset = null)
        {
            enableTextShadow = enable;
            if (shadowColor.HasValue) this.shadowColor = shadowColor.Value;
            if (shadowOffset.HasValue) this.shadowOffset = shadowOffset.Value;

            if (transitionText != null)
            {
                ApplyTextSettings(transitionText);
            }
        }


        public void ReloadCurrentStage(PointerEventData data) => ReloadCurrentStage();

        /// <summary>
        /// 현재 스테이지를 다시 로드합니다.
        /// </summary>
        public void ReloadCurrentStage()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            LoadGameSceneWithStage(CurrentSceneName, currentMainStage, currentSubStage);
        }

        public void QuitGame() => Application.Quit();

        #endregion

        #region Private Methods

        public override void Init()
        {

        }

        private void HandleSceneSpecificData(string sceneName, int mainStageID, int subStageID, int score, bool isWin)
        {
            if (sceneName == "dStageScene_JYL")
            {
                PlayerPrefs.SetInt("SelectedMainStage", mainStageID);
                PlayerPrefs.SetInt("SelectedSubStage", subStageID);
                PlayerPrefs.Save();
            }
        }

        private IEnumerator LoadSceneWithTransition(string sceneName)
        {
            if (IsLoading || string.IsNullOrEmpty(sceneName) || !DoesSceneExist(sceneName)) yield break;

            IsLoading = true;
            IsTransitioning = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            bool showTransition = enableTransition;
            if (showTransition) ShowTransitionScreen();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            if (asyncLoad == null)
            {
                IsLoading = false;
                IsTransitioning = false;
                yield break;
            }

            asyncLoad.allowSceneActivation = false;
            while (asyncLoad.progress < 0.9f) yield return null;

            if (showTransition) yield return StartCoroutine(EnsureMinimumTransitionTime(minTransitionTime));

            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone) yield return null;

            if (showTransition) HideTransitionScreen();

            IsLoading = false;
            IsTransitioning = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
        }

        private IEnumerator LoadSceneWithTransitionAndStage(string sceneName, int mainStageID, int subStageID)
        {
            if (IsLoading || string.IsNullOrEmpty(sceneName) || !DoesSceneExist(sceneName)) yield break;

            IsLoading = true;
            IsTransitioning = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            bool showTransition = enableTransition;
            if (showTransition) ShowTransitionScreen();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            if (asyncLoad == null)
            {
                IsLoading = false;
                IsTransitioning = false;
                yield break;
            }

            asyncLoad.allowSceneActivation = false;
            while (asyncLoad.progress < 0.9f) yield return null;

            if (showTransition) yield return StartCoroutine(EnsureMinimumTransitionTime(minTransitionTime));

            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone) yield return null;

            if (showTransition) HideTransitionScreen();

            IsLoading = false;
            IsTransitioning = false;
            OnSceneLoadCompleted?.Invoke(sceneName);

            yield return StartCoroutine(SetStageAfterSceneLoad(mainStageID, subStageID));
        }

        private IEnumerator EnsureMinimumTransitionTime(float transitionTime)
        {
            float startTime = Time.time;
            while (Time.time - startTime < transitionTime) yield return null;
        }

        private IEnumerator HideTransitionScreenAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideTransitionScreen();
        }

        private IEnumerator FadeOutTransitionScreen()
        {
            if (transitionCanvasGroup == null)
            {
                transitionCanvasGroup = transitionScreen.GetComponent<CanvasGroup>();
                if (transitionCanvasGroup == null) transitionCanvasGroup = transitionScreen.AddComponent<CanvasGroup>();
            }

            if (textAnimationCoroutine != null)
            {
                StopCoroutine(textAnimationCoroutine);
                textAnimationCoroutine = null;
            }

            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                transitionCanvasGroup.alpha = 1f - (elapsed / fadeOutDuration);
                yield return null;
            }

            transitionCanvasGroup.alpha = 0f;
            transitionScreen.SetActive(false);
            OnTransitionCompleted?.Invoke();
        }

        private void CreateTransitionScreen()
        {
            // 기존 전환 화면이 있으면 제거
            if (transitionScreen != null)
            {
                DestroyImmediate(transitionScreen);
            }

            // 프리팹이 있으면 프리팹 사용, 없으면 기본 화면 생성
            if (transitionScreenPrefab != null)
            {
                transitionScreen = Instantiate(transitionScreenPrefab);
            }
            else
            {
                // 기본 전환 화면 생성
                CreateDefaultTransitionScreen();
            }

            if (transitionScreen == null) return;

            // Canvas 설정
            Canvas canvas = transitionScreen.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 1000;
                DontDestroyOnLoad(canvas.gameObject);
            }
            else
            {
                // Canvas가 없으면 추가
                canvas = transitionScreen.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1000;
                transitionScreen.AddComponent<CanvasScaler>();
                transitionScreen.AddComponent<GraphicRaycaster>();
                DontDestroyOnLoad(transitionScreen);
            }

            // CanvasGroup 설정
            transitionCanvasGroup = transitionScreen.GetComponent<CanvasGroup>();
            if (transitionCanvasGroup == null) transitionCanvasGroup = transitionScreen.AddComponent<CanvasGroup>();

            // 텍스트 설정
            transitionText = transitionScreen.GetComponentInChildren<TextMeshProUGUI>();
            if (transitionText != null)
            {
                ApplyTextSettings(transitionText);
                if (transitionTexts.Length > 0) transitionText.text = transitionTexts[0];
            }

            // 배경 이미지 설정 - 모든 Image 컴포넌트를 찾아서 색상 강제 설정
            Image[] allImages = transitionScreen.GetComponentsInChildren<Image>();
            foreach (var image in allImages)
            {
                // LoadingScreen의 배경 이미지인 경우에만 색상 변경
                if (image.gameObject.name == "LoadingScreen" || image.gameObject.name == "Image")
                {
                    image.color = transitionColor;
                    transitionBackground = image;
                    Debug.Log($"전환 화면 배경 색상을 {transitionColor}로 설정: {image.gameObject.name}");
                }
            }

            // 초기 상태 설정
            transitionScreen.SetActive(false);
            transitionCanvasGroup.alpha = 0f;
            
            Debug.Log("전환 화면 생성 완료");
        }

        private void CreateDefaultTransitionScreen()
        {
            // 기본 전환 화면 생성
            transitionScreen = new GameObject("DefaultTransitionScreen");
            
            // 배경 이미지 추가
            Image backgroundImage = transitionScreen.AddComponent<Image>();
            backgroundImage.color = transitionColor;
            
            // 텍스트 추가
            GameObject textObj = new GameObject("TransitionText");
            textObj.transform.SetParent(transitionScreen.transform, false);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = transitionTexts.Length > 0 ? transitionTexts[0] : "로딩 중...";
            
            // 폰트 설정 적용
            ApplyTextSettings(text);
            
            // 텍스트 위치 설정
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // 배경 크기 설정
            RectTransform backgroundRect = transitionScreen.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;
            
            transitionText = text;
            transitionBackground = backgroundImage;
            
            Debug.Log("기본 전환 화면 생성 완료");
        }

        private void ApplyTextSettings(TextMeshProUGUI text)
        {
            if (text == null) return;

            // 폰트 설정
            if (transitionFont != null)
            {
                text.font = transitionFont;
            }

            // 기본 텍스트 설정
            text.fontSize = fontSize;
            text.color = textColor;
            text.alignment = textAlignment;

            // 그림자 설정
            if (enableTextShadow)
            {
                Shadow shadow = text.GetComponent<Shadow>();
                if (shadow == null)
                {
                    shadow = text.gameObject.AddComponent<Shadow>();
                }
                shadow.effectColor = shadowColor;
                shadow.effectDistance = shadowOffset;
            }
            else
            {
                Shadow shadow = text.GetComponent<Shadow>();
                if (shadow != null)
                {
                    DestroyImmediate(shadow);
                }
            }
        }

        private void StartTextAnimation()
        {
            if (textAnimationCoroutine != null) StopCoroutine(textAnimationCoroutine);
            textAnimationCoroutine = StartCoroutine(TextAnimationCoroutine());
        }

        private IEnumerator TextAnimationCoroutine()
        {
            if (transitionText == null || transitionTexts.Length == 0) yield break;

            int currentIndex = 0;
            while (true)
            {
                transitionText.text = transitionTexts[currentIndex];
                currentIndex = (currentIndex + 1) % transitionTexts.Length;
                yield return new WaitForSeconds(textChangeInterval);
            }
        }



        private bool DoesSceneExist(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneNameFromPath == sceneName) return true;
            }
            return false;
        }

        private IEnumerator SetStageAfterSceneLoad(int mainStageID, int subStageID)
        {
            StageManager stageManager = null;
            float waitTime = 0f;
            const float maxWaitTime = 5f;

            while (stageManager == null && waitTime < maxWaitTime)
            {
                stageManager = FindObjectOfType<StageManager>();
                if (stageManager == null)
                {
                    yield return new WaitForSeconds(0.1f);
                    waitTime += 0.1f;
                }
            }

            if (stageManager == null)
            {
                Debug.LogError("StageManager를 찾을 수 없습니다!");
                yield break;
            }

            yield return null;
            stageManager.ForceStage(mainStageID, subStageID);
        }

        private IEnumerator LoadSceneAfterActivation(string sceneName, int mainStageID, int subStageID, int score, bool isWin)
        {
            yield return null;
            HandleSceneSpecificData(sceneName, mainStageID, subStageID, score, isWin);
            StartCoroutine(LoadSceneWithTransition(sceneName));
        }

        private void LoadTransitionPrefab()
        {
            if (transitionScreenPrefab == null)
            {
                transitionScreenPrefab = Resources.Load<GameObject>(transitionPrefabPath);
                if (transitionScreenPrefab == null)
                {
                    Debug.LogWarning($"전환 화면 프리팹을 찾을 수 없습니다: Resources/{transitionPrefabPath}");
                }
                else
                {
                    Debug.Log($"전환 화면 프리팹 로드 완료: {transitionPrefabPath}");
                }
            }
        }

        #endregion
    }
}





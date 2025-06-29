using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YSK;

namespace YSK
{
    public class StageTransition : MonoBehaviour
    {
        [Header("Transition Settings")]
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("References")]
        [SerializeField] private StageManager stageManager;
        [SerializeField] private Camera mainCamera;
        
        private bool isTransitioning = false;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Awake에서는 최소한의 초기화만
        }

        private void Start()
        {
            InitializeComponents();
        }
        
        #endregion
        
        #region Initialization

        /// <summary>
        /// 필요한 컴포넌트들을 초기화합니다.
        /// </summary>
        private void InitializeComponents()
        {
            FindStageManager();
            FindMainCamera();
            InitializeFadePanel();
        }

        /// <summary>
        /// 부모 오브젝트에서 StageManager를 찾습니다.
        /// </summary>
        private void FindStageManager()
        {
            if (stageManager == null)
            {
                stageManager = GetComponentInParent<StageManager>();
                if (stageManager == null)
                {
                    Debug.LogError("StageManager를 부모 오브젝트에서 찾을 수 없습니다!");
                }
            }
        }

        /// <summary>
        /// MainCamera를 찾습니다.
        /// </summary>
        private void FindMainCamera()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("Main Camera를 찾을 수 없습니다!");
                }
            }
        }

        /// <summary>
        /// 페이드 패널을 초기화합니다.
        /// </summary>
        private void InitializeFadePanel()
        {
            if (fadePanel == null)
            {
                CreateFadePanel();
            }
            
            // 초기 상태 설정 - 항상 투명하게 시작
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
            
            Debug.Log("페이드 패널 초기화 완료 - 투명 상태");
        }
        
        private void CreateFadePanel()
        {
            // Canvas 찾기 또는 생성
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("TransitionCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999; // 최상위에 표시
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            // Fade Panel 생성
            GameObject fadeObj = new GameObject("FadePanel");
            fadeObj.transform.SetParent(canvas.transform, false);
            
            Image fadeImage = fadeObj.AddComponent<Image>();
            fadeImage.color = Color.black;
            
            RectTransform rectTransform = fadeObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            fadePanel = fadeObj.AddComponent<CanvasGroup>();
        }
        
        #endregion
        
        /// <summary>
        /// 부드러운 스테이지 전환을 시작합니다.
        /// </summary>
        /// <param name="newStageID">전환할 스테이지 ID</param>
        /// <param name="isGameStart">게임 시작 여부 (true: 게임 시작, false: 스테이지 전환)</param>
        public void StartStageTransition(int newStageID, bool isGameStart = false)
        {
            Debug.Log($"StageTransition.StartStageTransition 호출: 스테이지 {newStageID}, 게임시작: {isGameStart}");
            
            if (!isTransitioning)
            {
                Debug.Log("스테이지 전환 코루틴 시작");
                StartCoroutine(TransitionCoroutine(newStageID, isGameStart));
            }
            else
            {
                Debug.LogWarning("이미 전환 중입니다!");
            }
        }
        
        /// <summary>
        /// 페이드 인/아웃을 이용한 스테이지 전환
        /// </summary>
        private IEnumerator TransitionCoroutine(int newStageID, bool isGameStart)
        {
            Debug.Log($"TransitionCoroutine 시작: 스테이지 {newStageID}");
            isTransitioning = true;
            
            // 게임 시작인지 스테이지 전환인지 판단
            GameStateManager gameStateManager = GameStateManager.Instance;
            
            if (isGameStart)
            {
                Debug.Log("게임 시작: 페이드 효과 없이 즉시 전환");
                stageManager.ChangeStage(newStageID);
            }
            else
            {
                Debug.Log("스테이지 전환: 페이드 효과 사용");
                // 페이드 아웃
                yield return StartCoroutine(FadeOut());
                
                // 스테이지 전환
                stageManager.ChangeStage(newStageID);
                
                // 페이드 인
                yield return StartCoroutine(FadeIn());
            }
            
            isTransitioning = false;
            Debug.Log("TransitionCoroutine 완료");
        }
        
        /// <summary>
        /// 카메라 이동을 이용한 스테이지 전환
        /// </summary>
        public IEnumerator CameraTransitionCoroutine(int newStageID)
        {
            isTransitioning = true;
            
            Vector3 originalPos = mainCamera.transform.position;
            Vector3 upPos = originalPos + Vector3.up * 15f;
            
            // 1. 카메라를 위로 이동하면서 페이드 아웃
            yield return StartCoroutine(MoveCameraWithFade(originalPos, upPos, true));
            
            // 2. 스테이지 전환
            stageManager.ChangeStage(newStageID);
            
            // 3. 카메라를 새 위치로 이동하면서 페이드 인
            Vector3 newPos = originalPos; // 필요에 따라 새 위치 설정
            yield return StartCoroutine(MoveCameraWithFade(upPos, newPos, false));
            
            isTransitioning = false;
        }
        
        /// <summary>
        /// 점진적 맵 전환 (기존 맵이 사라지면서 새 맵이 나타남)
        /// </summary>
        public IEnumerator GradualTransitionCoroutine(int newStageID)
        {
            isTransitioning = true;
            
            // 새 맵을 먼저 생성 (화면 밖에)
            StageData newStage = stageManager.GetStageData(newStageID);
            if (newStage != null)
            {
                yield return StartCoroutine(CreateNewMapsOffscreen(newStage));
            }
            
            // 기존 맵들을 점진적으로 제거
            yield return StartCoroutine(RemoveOldMapsGradually());
            
            isTransitioning = false;
        }
        
        private IEnumerator FadeOut()
        {
            fadePanel.gameObject.SetActive(true);
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                fadePanel.alpha = fadeCurve.Evaluate(t);
                yield return null;
            }
            
            fadePanel.alpha = 1f;
        }
        
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                fadePanel.alpha = 1f - fadeCurve.Evaluate(t);
                yield return null;
            }
            
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
        
        private IEnumerator MoveCameraWithFade(Vector3 fromPos, Vector3 toPos, bool fadeOut)
        {
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                
                // 카메라 이동
                mainCamera.transform.position = Vector3.Lerp(fromPos, toPos, fadeCurve.Evaluate(t));
                
                // 페이드 효과
                if (fadeOut)
                {
                    fadePanel.alpha = fadeCurve.Evaluate(t);
                }
                else
                {
                    fadePanel.alpha = 1f - fadeCurve.Evaluate(t);
                }
                
                yield return null;
            }
            
            mainCamera.transform.position = toPos;
            
            if (fadeOut)
            {
                fadePanel.alpha = 1f;
            }
            else
            {
                fadePanel.alpha = 0f;
            }
        }
        
        private IEnumerator CreateNewMapsOffscreen(StageData newStage)
        {
            // StageManager에서 새 맵 생성 메서드 호출
            // 이 부분은 StageManager에 추가 메서드가 필요할 수 있습니다
            yield return null;
        }
        
        private IEnumerator RemoveOldMapsGradually()
        {
            // StageManager에서 기존 맵 제거 메서드 호출
            // 이 부분은 StageManager에 추가 메서드가 필요할 수 있습니다
            yield return null;
        }
        
        /// <summary>
        /// 현재 전환 중인지 확인
        /// </summary>
        public bool IsTransitioning => isTransitioning;
        
        /// <summary>
        /// 전환 효과 설정
        /// </summary>
        public void SetFadeDuration(float duration)
        {
            fadeDuration = duration;
        }
        
        /// <summary>
        /// 페이드 패널 색상 설정
        /// </summary>
        public void SetFadeColor(Color color)
        {
            if (fadePanel != null)
            {
                Image fadeImage = fadePanel.GetComponent<Image>();
                if (fadeImage != null)
                {
                    fadeImage.color = color;
                }
            }
        }
        
        /// <summary>
        /// 스테이지 버튼 클릭 이벤트 (UI에서 호출)
        /// </summary>
        /// <param name="stageID">선택된 스테이지 ID</param>
        public void OnStageButtonClick(int stageID)
        {
            if (!isTransitioning)
            {
                StartStageTransition(stageID, false); // 스테이지 전환
            }
        }
    }
} 
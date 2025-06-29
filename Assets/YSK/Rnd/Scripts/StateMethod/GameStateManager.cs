using System;
using UnityEngine;
using YSK;

namespace YSK
{
    public class GameStateManager : MonoBehaviour
    {
        [Header("State Management")]
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        [SerializeField] private StageProgressState currentStageState = StageProgressState.NotStarted;
        [SerializeField] private PlayerState currentPlayerState = PlayerState.Alive;
        
        [Header("Stage Management")]
        [SerializeField] private int currentStageID = 1;
        [SerializeField] private int maxStageID = 4;
        
        [Header("References")]
        [SerializeField] private StageManager stageManager;
        [SerializeField] private StageTransition stageTransition;
        
        // 싱글톤 패턴
        public static GameStateManager Instance { get; private set; }
        
        // 이벤트
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<StageProgressState> OnStageStateChanged;
        public static event Action<PlayerState> OnPlayerStateChanged;
        public static event Action<int> OnStageChanged;
        
        // 프로퍼티
        public GameState CurrentGameState => currentGameState;
        public StageProgressState CurrentStageState => currentStageState;
        public PlayerState CurrentPlayerState => currentPlayerState;
        public int CurrentStageID => currentStageID;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // 싱글톤 설정
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeReferences();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // GameSceneManager가 먼저 초기화되도록 약간의 지연
            Invoke(nameof(InitializeGame), 0.1f);
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeReferences()
        {
            if (stageManager == null)
                stageManager = FindObjectOfType<StageManager>();
                
            if (stageTransition == null)
                stageTransition = FindObjectOfType<StageTransition>();
        }
        
        private void InitializeGame()
        {
            // 항상 메인메뉴로 시작
            SetGameState(GameState.MainMenu);
            SetStageState(StageProgressState.NotStarted);
            SetPlayerState(PlayerState.Alive);
            
            Debug.Log("게임이 메인메뉴 상태로 초기화되었습니다.");
        }
        
        #endregion
        
        #region Game State Management
        
        /// <summary>
        /// 게임 상태를 변경합니다.
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (currentGameState == newState) return;
            
            GameState previousState = currentGameState;
            currentGameState = newState;
            
            OnGameStateChanged?.Invoke(newState);
            
            // 상태별 처리
            HandleGameStateChange(previousState, newState);
        }
        
        private void HandleGameStateChange(GameState previousState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    if (previousState == GameState.MainMenu || previousState == GameState.StageSelect)
                    {
                        StartStage(currentStageID);
                    }
                    break;
                    
                case GameState.StageComplete:
                    HandleStageComplete();
                    break;
                    
                case GameState.GameOver:
                    HandleGameOver();
                    break;
                    
                case GameState.Paused:
                    PauseGame();
                    break;
            }
        }
        
        #endregion
        
        #region Stage Management
        
        /// <summary>
        /// 스테이지를 시작합니다.
        /// </summary>
        public void StartStage(int stageID)
        {
            Debug.Log($"GameStateManager.StartStage 호출: 스테이지 {stageID}");
            
            if (stageID < 1 || stageID > maxStageID)
            {
                Debug.LogError($"Invalid stage ID: {stageID}");
                return;
            }
            
            currentStageID = stageID;
            SetStageState(StageProgressState.InProgress);
            SetGameState(GameState.Playing);
            
            // PlayerPrefs에서 메인 스테이지와 서브 스테이지 정보 가져오기
            int mainStageID = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);
            
            // UI 텍스트 업데이트 - 메인-서브 형태로 표시
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.UpdateStageText($"{mainStageID}-{subStageID}");
            }
            else
            {
                Debug.LogWarning("UIFactory를 찾을 수 없습니다!");
            }
            
            OnStageChanged?.Invoke(stageID);
        }
        
        /// <summary>
        /// 다음 스테이지로 진행합니다.
        /// </summary>
        public void NextStage()
        {
            if (currentStageID < maxStageID)
            {
                StartStage(currentStageID + 1);
            }
            else
            {
                // 모든 스테이지 완료
                SetGameState(GameState.StageComplete);
            }
        }
        
        /// <summary>
        /// 이전 스테이지로 돌아갑니다.
        /// </summary>
        public void PreviousStage()
        {
            if (currentStageID > 1)
            {
                StartStage(currentStageID - 1);
            }
        }
        
        /// <summary>
        /// 스테이지 완료를 처리합니다.
        /// </summary>
        private void HandleStageComplete()
        {
            SetStageState(StageProgressState.Completed);
            
            // 잠시 후 다음 스테이지로 자동 진행
            Invoke(nameof(AutoNextStage), 2f);
        }
        
        private void AutoNextStage()
        {
            if (currentGameState == GameState.StageComplete)
            {
                NextStage();
            }
        }
        
        #endregion
        
        #region Player State Management
        
        /// <summary>
        /// 플레이어 상태를 변경합니다.
        /// </summary>
        public void SetPlayerState(PlayerState newState)
        {
            if (currentPlayerState == newState) return;
            
            currentPlayerState = newState;
            OnPlayerStateChanged?.Invoke(newState);
            
            HandlePlayerStateChange(newState);
        }
        
        private void HandlePlayerStateChange(PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Dead:
                    HandlePlayerDeath();
                    break;
                    
                case PlayerState.Invincible:
                    StartInvincibility();
                    break;
            }
        }
        
        private void HandlePlayerDeath()
        {
            SetGameState(GameState.GameOver);
            SetStageState(StageProgressState.Failed);
        }
        
        private void StartInvincibility()
        {
            // 무적 시간 설정
            Invoke(nameof(EndInvincibility), 3f);
        }
        
        private void EndInvincibility()
        {
            if (currentPlayerState == PlayerState.Invincible)
            {
                SetPlayerState(PlayerState.Alive);
            }
        }
        
        #endregion
        
        #region Stage State Management
        
        /// <summary>
        /// 스테이지 진행 상태를 변경합니다.
        /// </summary>
        public void SetStageState(StageProgressState newState)
        {
            if (currentStageState == newState) return;
            
            currentStageState = newState;
            OnStageStateChanged?.Invoke(newState);
        }
        
        #endregion
        
        #region Game Control
        
        /// <summary>
        /// 게임을 일시정지합니다.
        /// </summary>
        public void PauseGame()
        {
            Time.timeScale = 0f;
        }
        
        /// <summary>
        /// 게임을 재개합니다.
        /// </summary>
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            SetGameState(GameState.Playing);
        }
        
        /// <summary>
        /// 게임을 일시정지/재개 토글합니다.
        /// </summary>
        public void TogglePause()
        {
            if (currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
            else if (currentGameState == GameState.Playing)
            {
                PauseGame();
                SetGameState(GameState.Paused);
            }
        }
        
        /// <summary>
        /// 게임 오버를 처리합니다.
        /// </summary>
        private void HandleGameOver()
        {
            // 게임 오버 UI 표시
            // 재시작 옵션 제공
        }
        
        /// <summary>
        /// 게임을 재시작합니다.
        /// </summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            currentStageID = 1;
            SetGameState(GameState.Playing);
            SetStageState(StageProgressState.InProgress);
            SetPlayerState(PlayerState.Alive);
        }
        
        #endregion
        
        #region Public Methods for External Systems
        
        /// <summary>
        /// 플레이어가 목표 지점에 도달했을 때 호출
        /// </summary>
        public void OnPlayerReachedGoal()
        {
            SetStageState(StageProgressState.Completed);
            SetGameState(GameState.StageComplete);
        }
        
        /// <summary>
        /// 플레이어가 사망했을 때 호출
        /// </summary>
        public void OnPlayerDied()
        {
            SetPlayerState(PlayerState.Dead);
        }
        
        /// <summary>
        /// 파워업 아이템을 획득했을 때 호출
        /// </summary>
        public void OnPowerUpCollected()
        {
            SetPlayerState(PlayerState.PowerUp);
        }
        
        /// <summary>
        /// StageManager 참조를 설정합니다.
        /// </summary>
        /// <param name="newStageManager">설정할 StageManager 인스턴스</param>
        public void SetStageManager(StageManager newStageManager)
        {
            stageManager = newStageManager;
            
            if (stageManager != null)
            {
                Debug.Log("GameStateManager에 StageManager 참조 설정 완료");
            }
            else
            {
                Debug.LogWarning("GameStateManager에서 StageManager 참조가 null로 설정되었습니다.");
            }
        }
        
        /// <summary>
        /// 현재 설정된 StageManager 참조를 반환합니다.
        /// </summary>
        /// <returns>StageManager 인스턴스 또는 null</returns>
        public StageManager GetStageManager()
        {
            return stageManager;
        }
        
        /// <summary>
        /// StageTransition 참조를 설정합니다.
        /// </summary>
        /// <param name="newStageTransition">설정할 StageTransition 인스턴스</param>
        public void SetStageTransition(StageTransition newStageTransition)
        {
            stageTransition = newStageTransition;
            
            if (stageTransition != null)
            {
                Debug.Log("GameStateManager에 StageTransition 참조 설정 완료");
            }
            else
            {
                Debug.LogWarning("GameStateManager에서 StageTransition 참조가 null로 설정되었습니다.");
            }
        }
        
        /// <summary>
        /// 현재 설정된 StageTransition 참조를 반환합니다.
        /// </summary>
        /// <returns>StageTransition 인스턴스 또는 null</returns>
        public StageTransition GetStageTransition()
        {
            return stageTransition;
        }
        
        #endregion
    }
}

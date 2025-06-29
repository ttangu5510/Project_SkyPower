using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YSK;

namespace YSK
{
    public class StageManager : MonoBehaviour
    {
        [Header("StageData")]
        [SerializeField] private List<StageData> stageDataList; // 모든 스테이지의 데이터
        private StageData currentStage;
        private List<GameObject> MapPrefabs;
        [SerializeField] int selectedstageID; // Test용 Stage ID의 경우 외부 선택에 의해서 전해지게 되는 값으로 설정해야함.

        [Header("MoveInfo")]
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float mapLength = 20;

        [Header("References")]
        [SerializeField] private GameStateManager gameStateManager;

        private List<GameObject> spawnedMaps = new(); // 프리팹을 이용한 Stage Map 생성

        private List<GameObject> movingMaps = new(); // 현재 이동중인 맵.
        private StageTransition stageControl;

        #region Unity Lifecycle

        private void Awake()
        {
            // Awake에서는 최소한의 초기화만
        }

        private void Start()
        {
            InitializeComponents();

            // GameStateManager 이벤트 구독
            if (gameStateManager != null)
            {
                GameStateManager.OnStageChanged += OnStageChanged;
                GameStateManager.OnGameStateChanged += OnGameStateChanged;
            }
        }

        private void Update()
        {
            UpdateMovingMaps();
            CheckInput();
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (gameStateManager != null)
            {
                GameStateManager.OnStageChanged -= OnStageChanged;
                GameStateManager.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        #endregion

        /// <summary>
        /// GameStateManager에서 스테이지 변경 이벤트를 받았을 때 호출됩니다.
        /// </summary>
        private void OnStageChanged(int newStageID)
        {
            Debug.Log($"GameStateManager에서 스테이지 변경 요청: {newStageID}");
            LoadStage(newStageID);
        }

        /// <summary>
        /// GameStateManager에서 게임 상태 변경 이벤트를 받았을 때 호출됩니다.
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            Debug.Log($"OnGameStateChanged: {newState}");

            switch (newState)
            {
                case GameState.Playing:
                    // 이미 스테이지가 로드되어 있지 않은 경우에만 로드
                    if (spawnedMaps.Count == 0)
                    {
                        Debug.Log("게임 시작: 스테이지 로드");
                        // PlayerPrefs에서 메인 스테이지 정보 가져오기
                        int mainStageID = PlayerPrefs.GetInt("SelectedMainStage", 1);
                        LoadStage(mainStageID);
                    }
                    else
                    {
                        Debug.Log($"게임 시작: 이미 스테이지가 로드됨 (맵 개수: {spawnedMaps.Count})");
                    }
                    break;

                case GameState.MainMenu:
                case GameState.StageSelect:
                    // 메인메뉴나 스테이지 선택 시에는 기존 스테이지 정리
                    Debug.Log("메인메뉴/스테이지 선택: 기존 맵 정리");
                    ClearAllMaps();
                    break;
            }
        }

        /// <summary>
        /// 필요한 컴포넌트들을 초기화합니다.
        /// </summary>
        private void InitializeComponents()
        {
            FindStageTransition();
            FindTransformPoints();
            FindGameStateManager();
        }

        /// <summary>
        /// GameStateManager를 찾습니다.
        /// </summary>
        private void FindGameStateManager()
        {
            if (gameStateManager == null)
            {
                gameStateManager = GameStateManager.Instance;
                if (gameStateManager == null)
                {
                    Debug.LogWarning("GameStateManager를 찾을 수 없습니다! (씬 전환 중이거나 아직 초기화되지 않았을 수 있습니다)");
                }
            }
        }

        /// <summary>
        /// 자식 오브젝트에서 StageTransition을 찾습니다.
        /// </summary>
        private void FindStageTransition()
        {
            stageControl = GetComponentInChildren<StageTransition>();

            if (stageControl == null)
            {
                Debug.LogWarning("StageTransition 컴포넌트를 자식 오브젝트에서 찾을 수 없습니다! (해당 씬에서만 사용되는 컴포넌트입니다)");
            }
        }

        /// <summary>
        /// 자식 오브젝트에서 StartPoint와 EndPoint를 찾습니다.
        /// </summary>
        private void FindTransformPoints()
        {
            if (startPoint == null)
            {
                startPoint = transform.Find("StartPoint");
                if (startPoint == null)
                {
                    Debug.LogWarning("StartPoint를 찾을 수 없습니다! (씬에서 설정해야 합니다)");
                }
            }

            if (endPoint == null)
            {
                endPoint = transform.Find("EndPoint");
                if (endPoint == null)
                {
                    Debug.LogWarning("EndPoint를 찾을 수 없습니다! (씬에서 설정해야 합니다)");
                }
            }
        }

        private void CheckInput()
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) HandleKey(1);
                else if (Input.GetKeyDown(KeyCode.Alpha2)) HandleKey(2);
                else if (Input.GetKeyDown(KeyCode.Alpha3)) HandleKey(3);
                else if (Input.GetKeyDown(KeyCode.Alpha4)) HandleKey(4);
            }
        }

        private void HandleKey(int keyNumber)
        {
            if (stageControl == null)
            {
                Debug.LogError("StageTransition이 초기화되지 않았습니다!");
                return;
            }

            switch (keyNumber)
            {
                case 1:
                    Debug.Log("1번 키: 첫 번째 스테이지 로드");
                    stageControl.StartStageTransition(1, false);
                    break;

                case 2:
                    Debug.Log("2번 키: 두 번째 스테이지 로드");
                    stageControl.StartStageTransition(2, false);
                    break;

                case 3:
                    Debug.Log("3번 키: 세 번째 스테이지 로드");
                    stageControl.StartStageTransition(3, false);
                    break;

                case 4:
                    Debug.Log("4번 키: 네 번째 스테이지 로드");
                    stageControl.StartStageTransition(4, false);
                    break;

                default:
                    Debug.LogWarning("알 수 없는 키 입력");
                    break;
            }
        }

        private void LoadStage(int stageID)
        {
            Debug.Log($"LoadStage 호출: 스테이지 {stageID}");

            // 기존 맵 정리
            ClearAllMaps();

            // PlayerPrefs에서 메인 스테이지와 서브 스테이지 정보 가져오기
            int mainStageID = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"스테이지 정보 - 메인: {mainStageID}, 서브: {subStageID}");

            // 스테이지 데이터 리스트가 비어있는지 확인
            if (stageDataList == null || stageDataList.Count == 0)
            {
                Debug.LogError("스테이지 데이터 리스트가 비어있습니다! Inspector에서 StageData를 할당해주세요.");
                return;
            }

            Debug.Log($"사용 가능한 스테이지: {string.Join(", ", stageDataList.Select(s => s.stageID))}");

            // 메인 스테이지 ID로 해당 스테이지 데이터 찾기
            currentStage = stageDataList.Find(data => data.stageID == mainStageID);

            if (currentStage == null)
            {
                Debug.LogError($"Main Stage ID {mainStageID} not found! 사용 가능한 스테이지: {string.Join(", ", stageDataList.Select(s => s.stageID))}");
                return;
            }

            Debug.Log($"메인 스테이지 {mainStageID} 데이터 로드 완료, 서브 스테이지 {subStageID} 맵 생성 시작");
            SpawnMaps();
        }

        private void SpawnMaps()
        {
            Debug.Log($"SpawnMaps 시작: 기존 맵 {spawnedMaps.Count}개 정리");
            spawnedMaps.Clear();
            movingMaps.Clear();

            // currentStage가 null인지 확인
            if (currentStage == null)
            {
                Debug.LogError("currentStage가 null입니다! LoadStage를 먼저 호출해주세요.");
                return;
            }

            // PlayerPrefs에서 서브 스테이지 정보 가져오기
            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);

            // 사용할 맵 프리팹 리스트 결정
            List<GameObject> mapPrefabsToUse = currentStage.mapPrefabs;

            // 서브 스테이지별 커스텀 맵이 있는지 확인
            if (currentStage.subStages != null && currentStage.subStages.Count > 0)
            {
                SubStageData subStageData = currentStage.subStages.Find(s => s.subStageID == subStageID);
                if (subStageData != null && subStageData.customMapPrefabs != null && subStageData.customMapPrefabs.Count > 0)
                {
                    mapPrefabsToUse = subStageData.customMapPrefabs;
                    Debug.Log($"서브 스테이지 {subStageID} 전용 맵 사용");
                }
                else
                {
                    Debug.Log($"서브 스테이지 {subStageID}는 기본 맵 사용");
                }
            }

            // 맵 프리팹 리스트가 비어있는지 확인
            if (mapPrefabsToUse == null || mapPrefabsToUse.Count == 0)
            {
                Debug.LogWarning($"스테이지 {currentStage.stageID}의 맵 프리팹이 비어있습니다!");
                return;
            }

            Debug.Log($"새 맵 생성 시작: {mapPrefabsToUse.Count}개 프리팹 (서브 스테이지 {subStageID})");
            for (int i = 0; i < mapPrefabsToUse.Count; i++)
            {
                // 프리팹이 null인지 확인
                if (mapPrefabsToUse[i] == null)
                {
                    Debug.LogWarning($"스테이지 {currentStage.stageID}의 맵 프리팹 {i}번이 null입니다!");
                    continue;
                }

                GameObject map = Instantiate(mapPrefabsToUse[i]);

                // startPoint가 null인지 확인
                if (startPoint == null)
                {
                    Debug.LogWarning("startPoint가 null입니다! 맵을 기본 위치에 생성합니다.");
                    map.transform.position = Vector3.back * (mapLength * i);
                }
                else
                {
                    // 맵을 일렬로 배치 (예: Z축 기준)
                    map.transform.position = startPoint.position + Vector3.back * (mapLength * i);
                }

                spawnedMaps.Add(map);
                movingMaps.Add(map);
                Debug.Log($"맵 {i + 1} 생성 완료: {map.name}");
            }

            Debug.Log($"SpawnMaps 완료: 총 {spawnedMaps.Count}개 맵 생성 (서브 스테이지 {subStageID})");
        }

        private void MoveMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            // endPoint가 null인지 확인
            if (endPoint == null)
            {
                Debug.LogWarning("endPoint가 null입니다! 맵 이동을 중단합니다.");
                return;
            }

            // endPoint를 지나면 startPoint로 재배치
            if (map.transform.position.z < endPoint.position.z)
            {
                float maxZ = GetMaxZPosition();
                map.transform.position = new Vector3(map.transform.position.x, map.transform.position.y, maxZ + mapLength);
            }
        }

        private float GetMaxZPosition()
        {
            float maxZ = float.MinValue;
            foreach (var map in movingMaps)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            return maxZ;
        }

        private void UpdateMovingMaps()
        {
            // movingMaps가 null이거나 비어있는지 확인
            if (movingMaps == null || movingMaps.Count == 0)
            {
                return;
            }

            foreach (var map in movingMaps)
            {
                if (map != null)
                {
                    MoveMap(map);
                }
            }
        }

        public void ChangeStage(int newStageID)
        {
            Debug.Log($"ChangeStage 호출: 스테이지를 {newStageID}로 변경");

            // LoadStage에서 이미 ClearAllMaps를 호출하므로 여기서는 생략
            // 기존 스테이지와 새 스테이지의 맵을 교체
            LoadStage(newStageID);

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
                Debug.LogWarning("UIFactory.Instance가 null입니다. UIFactory를 찾아보겠습니다.");
                UIFactory uiFactory = FindObjectOfType<UIFactory>();
                if (uiFactory != null)
                {
                    uiFactory.UpdateStageText($"{mainStageID}-{subStageID}");
                }
                else
                {
                    Debug.LogWarning("UIFactory를 찾을 수 없습니다! UI 텍스트 업데이트를 건너뜁니다.");
                }
            }

            // 3. 플레이어/카메라 위치 초기화
            // player.transform.position = ...;
            // camera.transform.position = ...;

            // 4. UI 및 게임 상태 초기화
            // UpdateUI();
            // SetGameState(GameState.Ready);
        }

        /// <summary>
        /// 특정 스테이지의 ID로 StageData를 반환합니다.
        /// </summary>
        /// <param name="stageID">찾을 스테이지의 ID</param>
        /// <returns>해당 스테이지의 데이터 또는 null</returns>
        public StageData GetStageData(int stageID)
        {
            return stageDataList.Find(data => data.stageID == stageID);
        }

        /// <summary>
        /// 현재 스테이지의 데이터를 반환합니다.
        /// </summary>
        /// <returns>현재 스테이지의 데이터</returns>
        public StageData GetCurrentStageData()
        {
            return currentStage;
        }

        /// <summary>
        /// 현재 생성된 맵 오브젝트 리스트를 반환합니다.
        /// </summary>
        /// <returns>생성된 맵 리스트</returns>
        public List<GameObject> GetSpawnedMaps()
        {
            return new List<GameObject>(spawnedMaps);
        }

        /// <summary>
        /// 현재 이동 중인 맵 오브젝트 리스트를 반환합니다.
        /// </summary>
        /// <returns>이동 중인 맵 리스트</returns>
        public List<GameObject> GetMovingMaps()
        {
            return new List<GameObject>(movingMaps);
        }

        /// <summary>
        /// 특정 맵을 제거합니다.
        /// </summary>
        /// <param name="map">제거할 맵 오브젝트</param>
        public void RemoveMap(GameObject map)
        {
            if (spawnedMaps.Contains(map))
            {
                spawnedMaps.Remove(map);
                movingMaps.Remove(map);
                Destroy(map);
            }
        }

        /// <summary>
        /// 모든 맵을 제거합니다.
        /// </summary>
        public void ClearAllMaps()
        {
            Debug.Log($"ClearAllMaps 시작: {spawnedMaps.Count}개 맵 제거");

            foreach (var map in spawnedMaps)
            {
                if (map != null)
                {
                    Debug.Log($"맵 제거: {map.name}");
                    Destroy(map);
                }
            }
            spawnedMaps.Clear();
            movingMaps.Clear();

            Debug.Log("ClearAllMaps 완료: 모든 맵 제거됨");
        }

        /// <summary>
        /// 맵 이동을 일시정지합니다.
        /// </summary>
        public void PauseMapMovement()
        {
            enabled = false;
        }

        /// <summary>
        /// 맵 이동을 재개합니다.
        /// </summary>
        public void ResumeMapMovement()
        {
            enabled = true;
        }

        /// <summary>
        /// 현재 선택된 메인 스테이지 ID를 반환합니다.
        /// </summary>
        /// <returns>메인 스테이지 ID</returns>
        public int GetCurrentMainStageID()
        {
            return PlayerPrefs.GetInt("SelectedMainStage", 1);
        }

        /// <summary>
        /// 현재 선택된 서브 스테이지 ID를 반환합니다.
        /// </summary>
        /// <returns>서브 스테이지 ID</returns>
        public int GetCurrentSubStageID()
        {
            return PlayerPrefs.GetInt("SelectedSubStage", 1);
        }

        /// <summary>
        /// 현재 서브 스테이지의 데이터를 반환합니다.
        /// </summary>
        /// <returns>서브 스테이지 데이터 또는 null</returns>
        public SubStageData GetCurrentSubStageData()
        {
            if (currentStage == null) return null;

            int subStageID = GetCurrentSubStageID();
            return currentStage.subStages?.Find(s => s.subStageID == subStageID);
        }

        /// <summary>
        /// 현재 스테이지의 난이도를 반환합니다.
        /// </summary>
        /// <returns>난이도 (1-5)</returns>
        public float GetCurrentDifficulty()
        {
            if (currentStage == null) return 1f;

            int subStageID = GetCurrentSubStageID();
            SubStageData subStageData = GetCurrentSubStageData();

            // 서브 스테이지별 커스텀 난이도가 있으면 사용
            if (subStageData != null && subStageData.customDifficulty > 0)
            {
                return subStageData.customDifficulty;
            }

            // 기본 난이도 + 서브 스테이지별 증가량
            return currentStage.baseDifficulty + (subStageID - 1) * currentStage.difficultyIncreasePerSubStage;
        }

        /// <summary>
        /// GameStateManager 참조를 설정합니다.
        /// </summary>
        /// <param name="newGameStateManager">설정할 GameStateManager 인스턴스</param>
        public void SetGameStateManager(GameStateManager newGameStateManager)
        {
            gameStateManager = newGameStateManager;

            if (gameStateManager != null)
            {
                Debug.Log("StageManager에 GameStateManager 참조 설정 완료");

                // 이벤트 구독
                GameStateManager.OnStageChanged += OnStageChanged;
                GameStateManager.OnGameStateChanged += OnGameStateChanged;
            }
            else
            {
                Debug.LogWarning("StageManager에서 GameStateManager 참조가 null로 설정되었습니다.");

                // 이벤트 구독 해제
                GameStateManager.OnStageChanged -= OnStageChanged;
                GameStateManager.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        /// <summary>
        /// 현재 설정된 GameStateManager 참조를 반환합니다.
        /// </summary>
        /// <returns>GameStateManager 인스턴스 또는 null</returns>
        public GameStateManager GetGameStateManager()
        {
            return gameStateManager;
        }

        /// <summary>
        /// StageTransition 참조를 설정합니다.
        /// </summary>
        /// <param name="newStageTransition">설정할 StageTransition 인스턴스</param>
        public void SetStageTransition(StageTransition newStageTransition)
        {
            stageControl = newStageTransition;

            if (stageControl != null)
            {
                Debug.Log("StageManager에 StageTransition 참조 설정 완료");
            }
            else
            {
                Debug.LogWarning("StageManager에서 StageTransition 참조가 null로 설정되었습니다.");
            }
        }

        /// <summary>
        /// 현재 설정된 StageTransition 참조를 반환합니다.
        /// </summary>
        /// <returns>StageTransition 인스턴스 또는 null</returns>
        public StageTransition GetStageTransition()
        {
            return stageControl;
        }
    }
}
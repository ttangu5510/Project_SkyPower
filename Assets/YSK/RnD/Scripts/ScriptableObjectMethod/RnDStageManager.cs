using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YSK;
using System.Collections;
using UnityEngine.UI;
using KYG_skyPower;

namespace YSK
{
    public class RnDStageManager : MonoBehaviour
    {
        [Header("Stage Data")]
        [SerializeField] private List<StageData> stageDataList;
        [SerializeField] private int maxMainStages = 4;
        [SerializeField] private int maxSubStages = 5;
        [SerializeField] private StageDataManager dataManager;

        private StageData currentStage;

        [Header("Map Connection System")]
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float mapLength = 20;
        [SerializeField] private float mapConnectionDistance = 0.1f;
        [SerializeField] private float mapTransitionDuration = 1f;
        [SerializeField] private AnimationCurve mapTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private int mapRepeatCount = 2; // 각 맵을 몇 개씩 생성할지

        // 맵 진행 관리
        private List<GameObject> oldMaps = new(); // 기존 맵들 (제거됨)
        private List<GameObject> newMaps = new(); // 새로 생성된 맵들 (무한 반복)
        private List<GameObject> currentStageMapPrefabs = new(); // 현재 스테이지의 맵 프리팹들
        private int currentMapIndex = 0; // 현재 진행 중인 맵 인덱스
        private bool isMapTransitioning = false;

        [Header("Transition Settings")]
        [SerializeField] private bool useGameSceneManagerTransition = true;
        [SerializeField] private bool enableTransition = true;

        private bool isTransitioning = false;

        // 이벤트
        public static System.Action<int, int> OnStageChanged;
        public static System.Action OnMapTransitionStarted;
        public static System.Action OnMapTransitionCompleted;

        #region Unity Lifecycle

        private void Awake()
        {
            // 최소한의 초기화만
        }

        private void Start()
        {
            Debug.Log("=== StageManager Start 시작 ===");
            InitializeComponents();
            //Manager.Game.onGameClear.AddListener(OnGameComplete); // 게임 클리어 이벤트 리스너 등록
            Debug.Log("=== StageManager Start 완료 ===");
        }

        private void Update()
        {
            if (!isMapTransitioning)
            {
                UpdateMapMovement();
                UpdateMapConnections();
               
            }
            
            // 디버그: F1 키로 맵 상태 확인
            if (Input.GetKeyDown(KeyCode.F1))
            {
                DebugMapStatus();
            }
            
            // 디버그: F2 키로 ClearCurrentStageAndNext 테스트
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("F2 키로 ClearCurrentStageAndNext 테스트 실행");
                ClearCurrentStageAndNext();
            }
            
            // 디버그: F3 키로 다음 맵 진행 테스트
            if (Input.GetKeyDown(KeyCode.F3))
            {
                Debug.Log("F3 키로 다음 맵 진행 테스트 실행");
                ProgressToNextMap();
            }
        }

        private void OnDestroy()
        {
            //Manager.Game.onGameClear.RemoveListener(OnGameComplete); // 게임 클리어 이벤트 리스너 제거
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            FindTransformPoints();
            FindDataManager();
        }

        private void FindTransformPoints()
        {
            if (startPoint == null)
            {
                startPoint = transform.Find("StartPoint");
                if (startPoint == null)
                {
                    Debug.LogWarning("StartPoint를 찾을 수 없습니다!");
                }
            }

            if (endPoint == null)
            {
                endPoint = transform.Find("EndPoint");
                if (endPoint == null)
                {
                    Debug.LogWarning("EndPoint를 찾을 수 없습니다!");
                }
            }
        }

        private void FindDataManager()
        {
            if (dataManager == null)
            {
                dataManager = FindObjectOfType<StageDataManager>();
                if (dataManager == null)
                {
                    Debug.LogWarning("StageDataManager를 찾을 수 없습니다!");
                }
            }
        }

        #endregion

        #region Map Connection System

        private void UpdateMapConnections()
        {
            // 모든 맵들을 하나의 리스트로 합쳐서 정렬
            var allMaps = new List<GameObject>();
            allMaps.AddRange(oldMaps);
            allMaps.AddRange(newMaps);
            
            if (allMaps.Count < 2) return;

            // 맵들을 Z 위치 순으로 정렬
            var sortedMaps = allMaps.OrderBy(m => m.transform.position.z).ToList();

            for (int i = 0; i < sortedMaps.Count - 1; i++)
            {
                GameObject currentMap = sortedMaps[i];
                GameObject nextMap = sortedMaps[i + 1];

                if (currentMap == null || nextMap == null) continue;

                float distance = Mathf.Abs(nextMap.transform.position.z - currentMap.transform.position.z);
                
                // 맵 간격이 너무 크면 조정
                if (distance > mapLength + mapConnectionDistance)
                {
                    Vector3 newPosition = currentMap.transform.position;
                    newPosition.z += mapLength;
                    nextMap.transform.position = newPosition;
                    
                    Debug.Log($"맵 연결 조정: {distance:F2} -> {mapLength:F2}");
                }
            }
        }

        private void UpdateMapMovement()
        {
            // 기존 맵들 이동
            foreach (var map in oldMaps.ToList())
            {
                if (map != null)
                {
                    MoveOldMap(map);
                }
                else
                {
                    oldMaps.Remove(map);
                }
            }

            // 새 맵들 이동
            foreach (var map in newMaps.ToList())
            {
                if (map != null)
                {
                    MoveNewMap(map);
                }
                else
                {
                    newMaps.Remove(map);
                }
            }
        }

        private void MoveOldMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            if (endPoint == null)
            {
                Debug.LogWarning("endPoint가 null입니다!");
                return;
            }

            // 기존 맵은 끝점을 지나면 제거
            if (map.transform.position.z < endPoint.position.z)
            {
                oldMaps.Remove(map);
                Destroy(map);
                Debug.Log($"기존 맵이 EndPoint를 지나 제거됨: {map.name}");
            }
        }

        private void MoveNewMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            if (endPoint == null)
            {
                Debug.LogWarning("endPoint가 null입니다!");
                return;
            }

            // 새 맵은 끝점을 지나면 뒤쪽으로 이동 (무한 반복)
            if (map.transform.position.z < endPoint.position.z)
            {
                float maxZ = GetMaxZPosition();
                if (maxZ == float.MinValue)
                {
                    maxZ = startPoint != null ? startPoint.position.z : 0f;
                }
                else
                {
                    maxZ += mapLength;
                }

                Vector3 newPosition = map.transform.position;
                newPosition.z = maxZ;
                map.transform.position = newPosition;

                Debug.Log($"새 맵이 EndPoint를 지나 뒤쪽으로 이동: {map.name} - 새 위치: {newPosition}");
            }
        }

        #endregion

        #region Map Management

        private void LoadStage(int mainStageID, int subStageID = 1)
        {
            Debug.Log($"=== LoadStage 시작: {mainStageID}-{subStageID} ===");

            // 맵 연결 전환 실행
            StartCoroutine(ConnectMapTransition(mainStageID, subStageID));

            Debug.Log($"=== LoadStage 완료: {mainStageID}-{subStageID} ===");
        }

        private IEnumerator ConnectMapTransition(int mainStageID, int subStageID)
        {
            Debug.Log($"맵 연결 전환 시작: {mainStageID}-{subStageID}");
            isMapTransitioning = true;
            OnMapTransitionStarted?.Invoke();

            // 1. 새 스테이지 데이터 준비
            UpdatePlayerPrefs(mainStageID, subStageID);

            if (!ValidateStageData(mainStageID))
            {
                Debug.LogError($"스테이지 데이터 검증 실패: {mainStageID}");
                isMapTransitioning = false;
                yield break;
            }

            // 2. 새 스테이지 데이터를 먼저 설정
            StageData newStage = stageDataList.Find(data => data.stageID == mainStageID);
            currentStage = newStage;

            // 3. 기존 새 맵들을 oldMaps로 이동 (제거 대기)
            MoveNewMapsToOldMaps();

            // 4. 새 맵 프리팹 가져오기
            List<GameObject> newMapPrefabs = GetMapPrefabsForSubStage(subStageID);

            if (newMapPrefabs == null || newMapPrefabs.Count == 0)
            {
                Debug.LogWarning($"새 스테이지 {mainStageID}의 맵 프리팹이 없습니다!");
                isMapTransitioning = false;
                yield break;
            }

            // 5. 새 맵들을 newMaps 리스트에 생성
            yield return StartCoroutine(SpawnAndConnectNewMaps(newMapPrefabs));

            // 6. 이벤트 발생
            OnStageChanged?.Invoke(mainStageID, subStageID);

            isMapTransitioning = false;
            OnMapTransitionCompleted?.Invoke();
            Debug.Log("맵 연결 전환 완료");
        }

        private IEnumerator SpawnAndConnectNewMaps(List<GameObject> mapPrefabs)
        {
            Debug.Log($"새 맵들 연결 생성: {mapPrefabs.Count}개 프리팹");

            // 현재 스테이지 맵 프리팹 저장
            currentStageMapPrefabs.Clear();
            currentStageMapPrefabs.AddRange(mapPrefabs);
            currentMapIndex = 0;

            // 첫 번째 맵을 mapRepeatCount만큼 생성
            yield return StartCoroutine(SpawnCurrentMapWithRepeat());

            Debug.Log($"첫 번째 맵 생성 완료: {newMaps.Count}개");
        }

        // 현재 맵 인덱스의 맵을 반복 생성하는 메서드
        private IEnumerator SpawnCurrentMapWithRepeat()
        {
            if (currentMapIndex >= currentStageMapPrefabs.Count)
            {
                Debug.LogWarning("더 이상 생성할 맵이 없습니다!");
                yield break;
            }

            GameObject currentMapPrefab = currentStageMapPrefabs[currentMapIndex];
            if (currentMapPrefab == null)
            {
                Debug.LogError($"맵 프리팹 {currentMapIndex}가 null입니다!");
                yield break;
            }

            Debug.Log($"현재 맵 {currentMapIndex} ({currentMapPrefab.name})을 {mapRepeatCount}개 생성");

            // 모든 맵들 중 가장 뒤쪽 Z 위치 찾기
            float maxZ = GetMaxZPosition();
            if (maxZ == float.MinValue)
            {
                maxZ = startPoint != null ? startPoint.position.z : 0f;
            }
            else
            {
                maxZ += mapLength;
            }

            // 현재 맵을 mapRepeatCount만큼 생성
            for (int i = 0; i < mapRepeatCount; i++)
            {
                GameObject map = Instantiate(currentMapPrefab);
                Vector3 spawnPosition = new Vector3(
                    startPoint != null ? startPoint.position.x : 0f,
                    startPoint != null ? startPoint.position.y : 0f,
                    maxZ + (mapLength * i)
                );

                map.transform.position = spawnPosition;
                newMaps.Add(map);

                Debug.Log($"맵 생성: {map.name} (반복 {i + 1}/{mapRepeatCount}) - 위치: {spawnPosition}");

                // 부드러운 등장 효과 (첫 번째만)
                if (i == 0)
                {
                    yield return StartCoroutine(FadeInMap(map));
                }
                else
                {
                    CanvasGroup cg = map.GetComponent<CanvasGroup>();
                    if (cg == null) cg = map.AddComponent<CanvasGroup>();
                    cg.alpha = 1f;
                }
            }

            Debug.Log($"맵 {currentMapIndex} 생성 완료: {mapRepeatCount}개");
        }

        private IEnumerator FadeInMap(GameObject map)
        {
            CanvasGroup cg = map.GetComponent<CanvasGroup>();
            if (cg == null) cg = map.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < mapTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / mapTransitionDuration;
                cg.alpha = mapTransitionCurve.Evaluate(t);
                yield return null;
            }

            cg.alpha = 1f;
        }

        private float GetMaxZPosition()
        {
            float maxZ = float.MinValue;
            
            // 기존 맵들에서 최대 Z 찾기
            foreach (var map in oldMaps)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            
            // 새 맵들에서도 최대 Z 찾기
            foreach (var map in newMaps)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            
            return maxZ;
        }

        private bool ValidateStageData(int mainStageID)
        {
            if (stageDataList == null || stageDataList.Count == 0)
            {
                Debug.LogError("스테이지 데이터 리스트가 비어있습니다!");
                return false;
            }

            if (!stageDataList.Exists(data => data.stageID == mainStageID))
            {
                Debug.LogError($"Main Stage ID {mainStageID} not found!");
                return false;
            }

            return true;
        }

        private void UpdatePlayerPrefs(int mainStageID, int subStageID)
        {
            PlayerPrefs.SetInt("SelectedMainStage", mainStageID);
            PlayerPrefs.SetInt("SelectedSubStage", subStageID);
            PlayerPrefs.Save();
        }

        private List<GameObject> GetMapPrefabsForSubStage(int subStageID)
        {
            // currentStage가 null일 수 있으므로 안전하게 처리
            if (currentStage == null)
            {
                Debug.LogWarning("currentStage가 null입니다. 기본 맵 프리팹을 사용합니다.");
                return stageDataList.Count > 0 ? stageDataList[0].mapPrefabs : new List<GameObject>();
            }

            List<GameObject> mapPrefabsToUse = currentStage.mapPrefabs;

            if (currentStage.subStages != null && currentStage.subStages.Count > 0)
            {
                SubStageData subStageData = currentStage.subStages.Find(s => s.subStageID == subStageID);
                if (subStageData != null && subStageData.customMapPrefabs != null && subStageData.customMapPrefabs.Count > 0)
                {
                    mapPrefabsToUse = subStageData.customMapPrefabs;
                    Debug.Log($"커스텀 서브스테이지 {subStageID} 맵 사용: {mapPrefabsToUse.Count}개");
                }
                else
                {
                    Debug.Log($"서브스테이지 {subStageID} 커스텀 맵 없음, 기본 맵 사용: {mapPrefabsToUse.Count}개");
                }
            }

            return mapPrefabsToUse;
        }

        public void ClearAllMaps()
        {
            Debug.Log($"ClearAllMaps 시작: 기존 맵 {oldMaps.Count}개, 새 맵 {newMaps.Count}개 정리");

            foreach (var map in oldMaps)
            {
                if (map != null)
                {
                    Destroy(map);
                }
            }
            oldMaps.Clear();

            foreach (var map in newMaps)
            {
                if (map != null)
                {
                    Destroy(map);
                }
            }
            newMaps.Clear();

            Debug.Log("ClearAllMaps 완료");
        }

        #endregion

        #region Stage Transition

        public void StartStageTransition(int mainStageID, int subStageID = 1, bool isGameStart = false)
        {
            Debug.Log($"StageManager.StartStageTransition: {mainStageID}-{subStageID}, 게임시작: {isGameStart}");

            if (!enableTransition)
            {
                LoadStage(mainStageID, subStageID);
                return;
            }

            if (!isTransitioning)
            {
                StartCoroutine(TransitionCoroutine(mainStageID, subStageID, isGameStart));
            }
            else
            {
                Debug.LogWarning("이미 전환 중입니다!");
            }
        }

        private IEnumerator TransitionCoroutine(int mainStageID, int subStageID, bool isGameStart)
        {
            Debug.Log($"TransitionCoroutine 시작: {mainStageID}-{subStageID}");
            isTransitioning = true;

            if (isGameStart)
            {
                LoadStage(mainStageID, subStageID);
            }
            else
            {
                if (enableTransition && useGameSceneManagerTransition && Manager.GSM != null)
                {
                    // GameSceneManager의 전환 화면 사용
                    Manager.GSM.ShowTransitionScreen();
                    yield return new WaitForSeconds(0.5f); // 전환 화면 표시 시간

                    LoadStage(mainStageID, subStageID);

                    yield return new WaitForSeconds(0.1f); // 스테이지 로드 대기
                    Manager.GSM.HideTransitionScreen();
                }
                else
                {
                    LoadStage(mainStageID, subStageID);
                }
            }

            isTransitioning = false;
            Debug.Log("TransitionCoroutine 완료");
        }

        #endregion

        #region Stage Progression

        public void ClearCurrentStageAndNext()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"=== 스테이지 클리어 시작: {currentMainStage}-{currentSubStage} ===");

            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);

            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어! 게임 클리어 처리");
                OnGameComplete();
                return;
            }

            // 맵 연결 전환 실행
            StartCoroutine(ConnectMapTransition(nextStage.mainStage, nextStage.subStage));
            
            Debug.Log($"다음 스테이지로 전환: {nextStage.mainStage}-{nextStage.subStage}");
        }

        private (int mainStage, int subStage, bool isGameComplete) CalculateNextStage(int currentMainStage, int currentSubStage)
        {
            int nextSubStage = currentSubStage + 1;

            if (nextSubStage > maxSubStages)
            {
                int nextMainStage = currentMainStage + 1;

                if (nextMainStage > maxMainStages)
                {
                    return (0, 0, true); // 게임 클리어
                }

                return (nextMainStage, 1, false);
            }
            else
            {
                return (currentMainStage, nextSubStage, false);
            }
        }

        private void OnGameComplete()
        {
            Debug.Log("게임 클리어!");

            if (Manager.GSM != null)
            {
                // 결과 화면으로 이동
            }
        }

        #endregion

        #region Public API

        public void OnStageButtonClick(int mainStageID, int subStageID = 1)
        {
            if (!isTransitioning)
            {
                StartStageTransition(mainStageID, subStageID, false);
            }
        }

        public void ForceStage(int mainStageID, int subStageID)
        {
            Debug.Log($"강제 스테이지 이동: {mainStageID}-{subStageID}");
            LoadStage(mainStageID, subStageID);
        }

        public void OnStageCompleted()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"=== 스테이지 완료 처리 시작: {currentMainStage}-{currentSubStage} ===");

            // 점수 가져오기
            int score = Manager.Score?.Score ?? 0;
            Debug.Log($"현재 점수: {score}");

            // StageDataManager를 통한 완전한 처리
            if (dataManager != null)
            {
                dataManager.CompleteStageWithSave(currentMainStage, currentSubStage, score, Time.time);
                Debug.Log("StageDataManager를 통한 완료 처리 완료");
            }
            else
            {
                Debug.LogError("StageDataManager가 null입니다!");
            }

            // 게임 클리어 처리
            if (Manager.Game != null)
            {
                Manager.Game.SetGameClear();
                Debug.Log("게임 클리어 처리 완료");
            }

            Debug.Log($"=== 스테이지 완료 처리 완료: {currentMainStage}-{currentSubStage} ===");
        }

        public void ResetStageProgress()
        {
            ClearAllMaps();
             PlayerPrefs.SetInt("SelectedMainStage", 1);
             PlayerPrefs.SetInt("SelectedSubStage", 1);
            PlayerPrefs.Save();
            int mainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int subStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            Debug.Log("스테이지 진행 상태 초기화: 1-1");
            StartCoroutine(ConnectMapTransition(mainStage,subStage));
            Debug.Log("스테이지 1-1 맵 생성.");
        }

        #endregion

        #region Utility Methods

        public StageData GetStageData(int stageID)
        {
            return stageDataList.Find(data => data.stageID == stageID);
        }

        public int GetCurrentSubStageID()
        {
            return PlayerPrefs.GetInt("SelectedSubStage", 1);
        }

        public SubStageData GetCurrentSubStageData()
        {
            if (currentStage == null) return null;

            int subStageID = GetCurrentSubStageID();
            return currentStage.subStages?.Find(s => s.subStageID == subStageID);
        }

        public bool IsTransitioning => isTransitioning;
        public bool IsMapTransitioning => isMapTransitioning;

        public void SetTransitionEnabled(bool enabled)
        {
            enableTransition = enabled;
        }

        public void SetUseGameSceneManagerTransition(bool use)
        {
            useGameSceneManagerTransition = use;
        }

        public void SetMapTransitionDuration(float duration)
        {
            mapTransitionDuration = Mathf.Max(0.1f, duration);
        }

        public int GetActiveMapCount()
        {
            return oldMaps.Count + newMaps.Count; // 두 리스트 합친 개수
        }

        private void UnlockNextStage(int currentMainStage, int currentSubStage)
        {
            if (dataManager == null) return;

            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);

            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어!");
                return;
            }

            dataManager.UnlockStage(nextStage.mainStage);
            dataManager.UnlockSubStage(nextStage.mainStage, nextStage.subStage);

            Debug.Log($"다음 스테이지 해금: {nextStage.mainStage}-{nextStage.subStage}");
        }

        private bool CanLoadStage(int mainStageID, int subStageID)
        {
            if (dataManager == null) return true;

            return dataManager.IsStageUnlocked(mainStageID) &&
                   dataManager.IsSubStageUnlocked(mainStageID, subStageID);
        }

        #endregion

        // 디버그용 메서드 추가
        public void DebugMapStatus()
        {
            Debug.Log("=== 맵 상태 디버그 ===");
            Debug.Log($"기존 맵 수: {oldMaps.Count}");
            Debug.Log($"새 맵 수: {newMaps.Count}");
            Debug.Log($"맵 전환 중: {isMapTransitioning}");
            Debug.Log($"현재 스테이지: {currentStage?.stageID ?? -1}");
            Debug.Log($"현재 서브스테이지: {GetCurrentSubStageID()}");
            Debug.Log($"현재 맵 인덱스: {currentMapIndex}/{currentStageMapPrefabs.Count}");
            Debug.Log($"맵 반복 수: {mapRepeatCount}");
            
            Debug.Log("--- 기존 맵들 (제거 예정) ---");
            for (int i = 0; i < oldMaps.Count; i++)
            {
                if (oldMaps[i] != null)
                {
                    Debug.Log($"기존 맵 {i}: {oldMaps[i].name} - 위치: {oldMaps[i].transform.position}");
                }
            }
            
            Debug.Log("--- 새 맵들 (무한 반복) ---");
            for (int i = 0; i < newMaps.Count; i++)
            {
                if (newMaps[i] != null)
                {
                    Debug.Log($"새 맵 {i}: {newMaps[i].name} - 위치: {newMaps[i].transform.position}");
                }
            }
            
            Debug.Log("=== 맵 상태 디버그 완료 ===");
        }

        // 기존 새 맵들을 oldMaps로 이동하는 메서드 추가
        private void MoveNewMapsToOldMaps()
        {
            Debug.Log($"기존 새 맵들을 oldMaps로 이동: {newMaps.Count}개");
            
            foreach (var map in newMaps)
            {
                if (map != null)
                {
                    oldMaps.Add(map);
                    Debug.Log($"맵을 oldMaps로 이동: {map.name}");
                }
            }
            
            newMaps.Clear();
            Debug.Log($"oldMaps 개수: {oldMaps.Count}, newMaps 개수: {newMaps.Count}");
        }

        // 다음 맵으로 진행하는 메서드 (외부에서 호출)
        public void ProgressToNextMap()
        {
            Debug.Log($"다음 맵으로 진행 요청: 현재 {currentMapIndex}, 전체 {currentStageMapPrefabs.Count}");

            if (currentMapIndex >= currentStageMapPrefabs.Count - 1)
            {
                Debug.Log("마지막 맵입니다. 스테이지 전환을 진행합니다.");
                // 스테이지 전환
                ClearCurrentStageAndNext();
                return;
            }

            // 기존 맵들을 oldMaps로 이동 (제거 대기)
            MoveNewMapsToOldMaps();

            // 다음 맵 인덱스로 이동
            currentMapIndex++;

            // 다음 맵 생성
            StartCoroutine(SpawnCurrentMapWithRepeat());
        }
    }
}

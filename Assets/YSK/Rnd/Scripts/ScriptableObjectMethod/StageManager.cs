using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YSK;
using System.Collections;
using UnityEngine.UI;
using KYG_skyPower;
using System.Net.NetworkInformation;

namespace YSK
{
    public class StageManager : MonoBehaviour
    {
        [Header("Stage Data")]
        [SerializeField] private List<StageData> stageDataList;
        [SerializeField] private int maxMainStages = 4;
        [SerializeField] private int maxSubStages = 5;
        [SerializeField] private StageDataManager dataManager;
        
        private StageData currentStage;

        [Header("Map System")]
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float mapLength = 20;
        
        private List<GameObject> spawnedMaps = new();
        private List<GameObject> movingMaps = new();

        [Header("Transition Settings")]
        [SerializeField] private bool useGameSceneManagerTransition = true;
        [SerializeField] private bool enableTransition = true;
        
        private bool isTransitioning = false;

        #region Unity Lifecycle

        private void Awake()
        {
            // 최소한의 초기화만
        }

        private void Start()
        {
            Debug.Log("=== StageManager Start 시작 ===");
            InitializeComponents();
            //Manager.Game.onGameClear.AddListener(OnStageCompleted);
            Debug.Log("=== StageManager Start 완료 ===");
        }

        private void Update()
        {
            UpdateMovingMaps();
        }

        private void OnDestroy()
        {
            //Manager.Game.onGameClear.RemoveListener(OnStageCompleted);
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

        #region Map Management

        private void LoadStage(int mainStageID, int subStageID = 1)
        {
            Debug.Log($"=== LoadStage 시작: {mainStageID}-{subStageID} ===");

            ClearAllMaps();
            UpdatePlayerPrefs(mainStageID, subStageID);

            if (!ValidateStageData(mainStageID))
                return;

            currentStage = stageDataList.Find(data => data.stageID == mainStageID);
            SpawnMaps();
            
            Debug.Log($"=== LoadStage 완료: {mainStageID}-{subStageID} ===");
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

        private void SpawnMaps()
        {
            spawnedMaps.Clear();
            movingMaps.Clear();

            if (currentStage == null)
            {
                Debug.LogError("currentStage가 null입니다!");
                return;
            }

            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);
            List<GameObject> mapPrefabsToUse = GetMapPrefabsForSubStage(subStageID);

            if (mapPrefabsToUse == null || mapPrefabsToUse.Count == 0)
            {
                Debug.LogWarning($"스테이지 {currentStage.stageID}의 맵 프리팹이 비어있습니다!");
                return;
            }

            Debug.Log($"새 맵 생성 시작: {mapPrefabsToUse.Count}개 프리팹 (서브 스테이지 {subStageID})");
            
            for (int i = 0; i < mapPrefabsToUse.Count; i++)
            {
                if (mapPrefabsToUse[i] == null)
                {
                    Debug.LogWarning($"맵 프리팹 {i}번이 null입니다!");
                    continue;
                }

                GameObject map = Instantiate(mapPrefabsToUse[i]);
                Vector3 spawnPosition = startPoint != null 
                    ? startPoint.position + Vector3.forward * (mapLength * i)
                    : Vector3.forward * (mapLength * i);
                
                map.transform.position = spawnPosition;
                spawnedMaps.Add(map);
                movingMaps.Add(map);
            }

            Debug.Log($"SpawnMaps 완료: 총 {spawnedMaps.Count}개 맵 생성");
        }

        private List<GameObject> GetMapPrefabsForSubStage(int subStageID)
        {
            List<GameObject> mapPrefabsToUse = currentStage.mapPrefabs;

            if (currentStage.subStages != null && currentStage.subStages.Count > 0)
            {
                SubStageData subStageData = currentStage.subStages.Find(s => s.subStageID == subStageID);
                if (subStageData != null && subStageData.customMapPrefabs != null && subStageData.customMapPrefabs.Count > 0)
                {
                    mapPrefabsToUse = subStageData.customMapPrefabs;
                    Debug.Log($"서브 스테이지 {subStageID} 커스텀 맵 사용");
                }
            }

            return mapPrefabsToUse;
        }

        private void UpdateMovingMaps()
        {
            if (movingMaps == null || movingMaps.Count == 0)
                return;

            foreach (var map in movingMaps)
            {
                if (map != null)
                {
                    MoveMap(map);
                }
            }
        }

        private void MoveMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            if (endPoint == null)
            {
                Debug.LogWarning("endPoint가 null입니다!");
                return;
            }

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

        public void ClearAllMaps()
        {
            Debug.Log($"ClearAllMaps 시작: {spawnedMaps.Count}개 맵 정리");

            foreach (var map in spawnedMaps)
            {
                if (map != null)
                {
                    Destroy(map);
                }
            }
            spawnedMaps.Clear();
            movingMaps.Clear();

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

        public void StartStageTransitionOnlyFadeIn(int mainStageID, int subStageID = 1, bool isGameStart = false)
        {
            Debug.Log($"StageManager.StartStageTransitionOnlyFadeIn: {mainStageID}-{subStageID}, 게임시작: {isGameStart}");

            if (!enableTransition)
            {
                LoadStage(mainStageID, subStageID);
                return;
            }

            if (!isTransitioning)
            {
                StartCoroutine(TransitionWithFadeInCoroutine(mainStageID, subStageID, isGameStart));
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

        private IEnumerator TransitionWithFadeInCoroutine(int mainStageID, int subStageID, bool isGameStart)
        {
            Debug.Log($"TransitionWithFadeInCoroutine 시작: {mainStageID}-{subStageID}");
            isTransitioning = true;

            if (isGameStart)
            {
                LoadStage(mainStageID, subStageID);
            }
            else
            {
                if (enableTransition && useGameSceneManagerTransition && Manager.GSM != null)
                {
                    // GameSceneManager의 전환 화면을 검은색으로 설정
                    Manager.GSM.SetTransitionText("스테이지 전환 중...");
                    Manager.GSM.ShowTransitionScreen();
                    
                    LoadStage(mainStageID, subStageID);
                    yield return new WaitForSeconds(0.1f);
                    
                    Manager.GSM.HideTransitionScreen();
                }
                else
                {
                    LoadStage(mainStageID, subStageID);
                }
            }

            isTransitioning = false;
            Debug.Log("TransitionWithFadeInCoroutine 완료");
        }

        #endregion

        #region Stage Progression

        public void ClearCurrentStageAndNext()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            
            Debug.Log($"스테이지 클리어: {currentMainStage}-{currentSubStage}");
            
            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);
            
            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어! 게임 클리어 처리");
                OnGameComplete();
                return;
            }
            
            LoadStage(nextStage.mainStage, nextStage.subStage);
            Debug.Log($"다음 스테이지로 전환: {nextStage.mainStage}-{nextStage.subStage}");
        }

        public void ClearCurrentStageAndNextWithTransition()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            
            Debug.Log($"스테이지 클리어 (전환 화면 사용): {currentMainStage}-{currentSubStage}");
            
            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);
            
            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어! 게임 클리어 처리");
                OnGameComplete();
                return;
            }
            
            StartStageTransition(nextStage.mainStage, nextStage.subStage, false);
            Debug.Log($"다음 스테이지로 전환 (전환 화면): {nextStage.mainStage}-{nextStage.subStage}");
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
            PlayerPrefs.SetInt("SelectedMainStage", 1);
            PlayerPrefs.SetInt("SelectedSubStage", 1);
            PlayerPrefs.Save();
            LoadStage(1, 1);
            Debug.Log("스테이지 진행 상태 초기화: 1-1");
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

        public void SetTransitionEnabled(bool enabled)
        {
            enableTransition = enabled;
        }

        public void SetUseGameSceneManagerTransition(bool use)
        {
            useGameSceneManagerTransition = use;
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
    }
}

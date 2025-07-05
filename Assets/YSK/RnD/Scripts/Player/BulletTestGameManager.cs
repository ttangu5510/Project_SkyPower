using UnityEngine;
using System.Collections.Generic;

namespace YSK
{
    public class BulletTestGameManager : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool autoSpawnTargets = true;
        [SerializeField] private int maxTargets = 5;
        [SerializeField] private float targetSpawnInterval = 3f;
        [SerializeField] private Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);
        
        [Header("Player Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3 playerSpawnPosition = Vector3.zero;
        
        [Header("Target Settings")]
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private float targetHealth = 100f;
        
        [Header("UI Settings")]
        [SerializeField] private bool showUI = true;
        [SerializeField] private KeyCode resetKey = KeyCode.R;
        [SerializeField] private KeyCode spawnTargetKey = KeyCode.T;
        [SerializeField] private KeyCode clearAllKey = KeyCode.C;
        
        // Private variables
        private PlayerController playerController;
        private List<TargetController> activeTargets = new List<TargetController>();
        private float lastTargetSpawnTime = 0f;
        private int targetsDestroyed = 0;
        private int totalShots = 0;
        
        void Start()
        {
            InitializeGame();
        }
        
        void Update()
        {
            HandleInput();
            HandleAutoSpawn();
            UpdateUI();
        }
        
        #region Initialization
        
        private void InitializeGame()
        {
            SpawnPlayer();
            SetupTargets();
            
            Debug.Log("총알 효과 테스트 게임이 시작되었습니다!");
            Debug.Log("조작법:");
            Debug.Log("- WASD: 이동");
            Debug.Log("- 마우스: 회전");
            Debug.Log("- 마우스 좌클릭 또는 스페이스바: 총알 발사");
            Debug.Log("- R: 게임 리셋");
            Debug.Log("- T: 타겟 스폰");
            Debug.Log("- C: 모든 총알 제거");
        }
        
        private void SpawnPlayer()
        {
            if (playerPrefab != null)
            {
                GameObject player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
                playerController = player.GetComponent<PlayerController>();
            }
            else
            {
                // 기본 플레이어 생성
                GameObject player = CreateDefaultPlayer();
                playerController = player.GetComponent<PlayerController>();
            }
            
            if (playerController == null)
            {
                Debug.LogError("PlayerController를 찾을 수 없습니다!");
            }
        }
        
        private GameObject CreateDefaultPlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "DefaultPlayer";
            player.transform.position = playerSpawnPosition;
            
            // PlayerController 추가
            PlayerController controller = player.AddComponent<PlayerController>();
            
            // 기본 머티리얼 설정
            Renderer renderer = player.GetComponent<Renderer>();
            Material material = new Material(Shader.Find("Standard"));
            material.color = Color.blue;
            renderer.material = material;
            
            return player;
        }
        
        private void SetupTargets()
        {
            if (autoSpawnTargets)
            {
                // 초기 타겟들 스폰
                for (int i = 0; i < maxTargets / 2; i++)
                {
                    SpawnTarget();
                }
            }
        }
        
        #endregion
        
        #region Target Management
        
        private void SpawnTarget()
        {
            if (activeTargets.Count >= maxTargets) return;
            
            Vector3 spawnPosition = GetRandomSpawnPosition();
            
            GameObject target;
            if (targetPrefab != null)
            {
                target = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                target = CreateDefaultTarget(spawnPosition);
            }
            
            TargetController targetController = target.GetComponent<TargetController>();
            if (targetController != null)
            {
                targetController.SetMaxHealth(targetHealth);
                targetController.SetHealth(targetHealth);
                activeTargets.Add(targetController);
            }
            
            Debug.Log($"타겟이 스폰되었습니다: {spawnPosition}");
        }
        
        private GameObject CreateDefaultTarget(Vector3 position)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "DefaultTarget";
            target.transform.position = position;
            target.transform.localScale = new Vector3(2f, 2f, 0.5f);
            
            // TargetController 추가
            TargetController controller = target.AddComponent<TargetController>();
            
            // 기본 머티리얼 설정
            Renderer renderer = target.GetComponent<Renderer>();
            Material material = new Material(Shader.Find("Standard"));
            material.color = Color.white;
            renderer.material = material;
            
            return target;
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float z = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);
            
            return new Vector3(x, 1f, z);
        }
        
        private void HandleAutoSpawn()
        {
            if (!autoSpawnTargets) return;
            
            if (Time.time - lastTargetSpawnTime >= targetSpawnInterval)
            {
                SpawnTarget();
                lastTargetSpawnTime = Time.time;
            }
        }
        
        private void ClearAllTargets()
        {
            foreach (var target in activeTargets)
            {
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
            }
            activeTargets.Clear();
            
            Debug.Log("모든 타겟이 제거되었습니다.");
        }
        
        private void ResetAllTargets()
        {
            foreach (var target in activeTargets)
            {
                if (target != null)
                {
                    target.ResetTarget();
                }
            }
            
            Debug.Log("모든 타겟이 리셋되었습니다.");
        }
        
        #endregion
        
        #region Input Handling
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(resetKey))
            {
                ResetGame();
            }
            
            if (Input.GetKeyDown(spawnTargetKey))
            {
                SpawnTarget();
            }
            
            if (Input.GetKeyDown(clearAllKey))
            {
                ClearAllTargets();
            }
        }
        
        private void ResetGame()
        {
            // 플레이어 리셋
            if (playerController != null)
            {
                playerController.ClearAllBullets();
                playerController.transform.position = playerSpawnPosition;
            }
            
            // 타겟 리셋
            ResetAllTargets();
            
            // 통계 리셋
            targetsDestroyed = 0;
            totalShots = 0;
            
            Debug.Log("게임이 리셋되었습니다.");
        }
        
        #endregion
        
        #region Statistics
        
        public void OnTargetDestroyed()
        {
            targetsDestroyed++;
        }
        
        public void OnShotFired()
        {
            totalShots++;
        }
        
        private float GetAccuracy()
        {
            if (totalShots == 0) return 0f;
            return (float)targetsDestroyed / totalShots * 100f;
        }
        
        #endregion
        
        #region UI
        
        private void UpdateUI()
        {
            if (!showUI) return;
            
            // 화면에 UI 정보 표시
            string uiInfo = $"게임 통계:\n" +
                          $"활성 타겟: {activeTargets.Count}/{maxTargets}\n" +
                          $"파괴된 타겟: {targetsDestroyed}\n" +
                          $"총 발사 수: {totalShots}\n" +
                          $"명중률: {GetAccuracy():F1}%\n" +
                          $"R: 리셋, T: 타겟 스폰, C: 타겟 제거";
            
            // OnGUI에서 표시
        }
        
        void OnGUI()
        {
            if (!showUI) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 290, 200));
            GUILayout.Label("=== 총알 효과 테스트 ===");
            GUILayout.Label($"활성 타겟: {activeTargets.Count}/{maxTargets}");
            GUILayout.Label($"파괴된 타겟: {targetsDestroyed}");
            GUILayout.Label($"총 발사 수: {totalShots}");
            GUILayout.Label($"명중률: {GetAccuracy():F1}%");
            GUILayout.Space(10);
            GUILayout.Label("조작법:");
            GUILayout.Label("WASD: 이동");
            GUILayout.Label("마우스: 회전");
            GUILayout.Label("마우스 좌클릭/스페이스: 발사");
            GUILayout.Label("R: 리셋, T: 타겟 스폰, C: 타겟 제거");
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Public Methods
        
        public void SetAutoSpawn(bool enabled)
        {
            autoSpawnTargets = enabled;
        }
        
        public void SetMaxTargets(int maxTargets)
        {
            this.maxTargets = maxTargets;
        }
        
        public void SetTargetSpawnInterval(float interval)
        {
            targetSpawnInterval = interval;
        }
        
        public void SetTargetHealth(float health)
        {
            targetHealth = health;
            foreach (var target in activeTargets)
            {
                if (target != null)
                {
                    target.SetMaxHealth(health);
                }
            }
        }
        
        public PlayerController GetPlayerController()
        {
            return playerController;
        }
        
        public List<TargetController> GetActiveTargets()
        {
            return activeTargets;
        }
        
        #endregion
        
        #region Events
        
        void OnDrawGizmosSelected()
        {
            // 스폰 영역 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, spawnAreaSize);
            
            // 플레이어 스폰 위치 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(playerSpawnPosition, 1f);
        }
        
        #endregion
    }
} 
using UnityEngine;
using System.Collections.Generic;

namespace YSK
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float jumpForce = 5f;
        
        [Header("Shooting Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private int maxBullets = 10;
        
        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private AudioSource shootSound;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Private variables
        private Rigidbody rb;
        private bool canShoot = true;
        private float nextFireTime = 0f;
        private List<GameObject> activeBullets = new List<GameObject>();
        
        // Input variables
        private float horizontalInput;
        private float verticalInput;
        private bool jumpInput;
        private bool shootInput;
        
        void Start()
        {
            InitializeComponents();
            SetupFirePoint();
        }
        
        void Update()
        {
            GetInput();
            HandleShooting();
            UpdateDebugInfo();
        }
        
        void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
            HandleJump();
        }
        
        #region Initialization
        
        private void InitializeComponents()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            // Rigidbody 설정
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // FirePoint가 없으면 자동 생성
            if (firePoint == null)
            {
                CreateFirePoint();
            }
        }
        
        private void SetupFirePoint()
        {
            if (firePoint == null)
            {
                CreateFirePoint();
            }
            
            // 총알 프리팹이 없으면 기본 총알 생성
            if (bulletPrefab == null)
            {
                CreateDefaultBulletPrefab();
            }
        }
        
        private void CreateFirePoint()
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 1.5f, 1f);
            firePoint = firePointObj.transform;
            
            Debug.Log("FirePoint가 자동으로 생성되었습니다.");
        }
        
        private void CreateDefaultBulletPrefab()
        {
            // 기본 총알 프리팹 생성
            GameObject bullet = new GameObject("DefaultBullet");
            
            // MeshRenderer 추가
            MeshRenderer renderer = bullet.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = bullet.AddComponent<MeshFilter>();
            
            // 구체 메시 생성
            meshFilter.mesh = CreateSphereMesh();
            
            // 기본 머티리얼 생성
            Material material = new Material(Shader.Find("Standard"));
            material.color = Color.yellow;
            renderer.material = material;
            
            // Collider 추가
            SphereCollider collider = bullet.AddComponent<SphereCollider>();
            collider.radius = 0.1f;
            
            // Rigidbody 추가
            Rigidbody bulletRb = bullet.AddComponent<Rigidbody>();
            bulletRb.useGravity = false;
            
            // BulletEffectController 추가
            BulletEffectController bulletEffect = bullet.AddComponent<BulletEffectController>();
            bulletEffect.speed = bulletSpeed;
            
            // 총알 크기 조정
            bullet.transform.localScale = Vector3.one * 0.2f;
            
            bulletPrefab = bullet;
            bullet.SetActive(false); // 프리팹으로 사용하기 위해 비활성화
            
            Debug.Log("기본 총알 프리팹이 생성되었습니다.");
        }
        
        private Mesh CreateSphereMesh()
        {
            // 간단한 구체 메시 생성
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Mesh sphereMesh = tempSphere.GetComponent<MeshFilter>().mesh;
            DestroyImmediate(tempSphere);
            return sphereMesh;
        }
        
        #endregion
        
        #region Input Handling
        
        private void GetInput()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            jumpInput = Input.GetKeyDown(KeyCode.Space);
            shootInput = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
        }
        
        #endregion
        
        #region Movement
        
        private void HandleMovement()
        {
            Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
            movement = movement.normalized * moveSpeed * Time.fixedDeltaTime;
            
            rb.MovePosition(rb.position + movement);
        }
        
        private void HandleRotation()
        {
            // 마우스 X축 회전 (좌우 회전)
            float mouseX = Input.GetAxis("Mouse X");
            Vector3 rotation = Vector3.up * mouseX * rotationSpeed * Time.deltaTime;
            transform.Rotate(rotation);
        }
        
        private void HandleJump()
        {
            if (jumpInput && IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        
        private bool IsGrounded()
        {
            // 간단한 지면 체크
            return Physics.Raycast(transform.position, Vector3.down, 1.1f);
        }
        
        #endregion
        
        #region Shooting
        
        private void HandleShooting()
        {
            if (shootInput && canShoot && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        
        private void Shoot()
        {
            if (bulletPrefab == null || firePoint == null) return;
            
            // 총알 개수 제한 확인
            CleanupDestroyedBullets();
            if (activeBullets.Count >= maxBullets) return;
            
            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(true);
            
            // 총알에 속도 적용
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = firePoint.forward * bulletSpeed;
            }
            
            // 총알 효과 적용
            BulletEffectController bulletEffect = bullet.GetComponent<BulletEffectController>();
            if (bulletEffect != null)
            {
                bulletEffect.speed = bulletSpeed;
            }
            
            // 총알을 활성 리스트에 추가
            activeBullets.Add(bullet);
            
            // 시각 효과
            PlayMuzzleFlash();
            PlayShootSound();
            
            Debug.Log($"총알 발사! 위치: {firePoint.position}, 방향: {firePoint.forward}");
        }
        
        private void CleanupDestroyedBullets()
        {
            activeBullets.RemoveAll(bullet => bullet == null);
        }
        
        private void PlayMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
        }
        
        private void PlayShootSound()
        {
            if (shootSound != null)
            {
                shootSound.Play();
            }
        }
        
        #endregion
        
        #region Debug
        
        private void UpdateDebugInfo()
        {
            if (!showDebugInfo) return;
            
            // 화면에 디버그 정보 표시
            string debugInfo = $"플레이어 상태:\n" +
                             $"위치: {transform.position}\n" +
                             $"속도: {rb.velocity.magnitude:F1}\n" +
                             $"활성 총알: {activeBullets.Count}/{maxBullets}\n" +
                             $"총알 발사 가능: {canShoot}\n" +
                             $"다음 발사 시간: {nextFireTime - Time.time:F2}";
            
            // OnGUI에서 표시하거나 Debug.Log로 출력
            if (Time.frameCount % 60 == 0) // 1초마다 한 번씩만 출력
            {
                Debug.Log(debugInfo);
            }
        }
        
        void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"플레이어 상태:");
            GUILayout.Label($"위치: {transform.position}");
            GUILayout.Label($"속도: {rb.velocity.magnitude:F1}");
            GUILayout.Label($"활성 총알: {activeBullets.Count}/{maxBullets}");
            GUILayout.Label($"총알 발사 가능: {canShoot}");
            GUILayout.Label($"다음 발사 시간: {nextFireTime - Time.time:F2:F2}");
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Public Methods
        
        public void SetCanShoot(bool canShoot)
        {
            this.canShoot = canShoot;
        }
        
        public void SetFireRate(float fireRate)
        {
            this.fireRate = Mathf.Max(0.1f, fireRate);
        }
        
        public void SetBulletSpeed(float speed)
        {
            this.bulletSpeed = speed;
        }
        
        public void SetMaxBullets(int maxBullets)
        {
            this.maxBullets = Mathf.Max(1, maxBullets);
        }
        
        public void ClearAllBullets()
        {
            foreach (var bullet in activeBullets)
            {
                if (bullet != null)
                {
                    Destroy(bullet);
                }
            }
            activeBullets.Clear();
        }
        
        #endregion
        
        #region Events
        
        void OnDrawGizmosSelected()
        {
            if (firePoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(firePoint.position, 0.1f);
                Gizmos.DrawRay(firePoint.position, firePoint.forward * 2f);
            }
        }
        
        #endregion
    }
} 
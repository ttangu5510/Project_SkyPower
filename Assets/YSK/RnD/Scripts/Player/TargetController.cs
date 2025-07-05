using UnityEngine;
using System.Collections;

namespace YSK
{
    public class TargetController : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool isDestructible = true;
        [SerializeField] private float respawnTime = 3f;
        
        [Header("Visual Effects")]
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material hitMaterial;
        [SerializeField] private Material destroyedMaterial;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private ParticleSystem destroyEffect;
        
        [Header("Audio")]
        [SerializeField] private AudioSource hitSound;
        [SerializeField] private AudioSource destroySound;
        
        [Header("Animation")]
        [SerializeField] private bool useHitAnimation = true;
        [SerializeField] private float hitAnimationDuration = 0.2f;
        [SerializeField] private float hitAnimationScale = 1.2f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Private variables
        private Renderer targetRenderer;
        private Vector3 originalScale;
        private bool isDestroyed = false;
        private Coroutine hitAnimationCoroutine;
        private Coroutine respawnCoroutine;
        
        void Start()
        {
            InitializeComponents();
            SetupMaterials();
        }
        
        #region Initialization
        
        private void InitializeComponents()
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                targetRenderer = gameObject.AddComponent<MeshRenderer>();
            }
            
            originalScale = transform.localScale;
            
            // 기본 머티리얼이 없으면 생성
            if (normalMaterial == null)
            {
                CreateDefaultMaterials();
            }
        }
        
        private void SetupMaterials()
        {
            if (normalMaterial != null)
            {
                targetRenderer.material = normalMaterial;
            }
        }
        
        private void CreateDefaultMaterials()
        {
            // 기본 머티리얼들 생성
            normalMaterial = new Material(Shader.Find("Standard"));
            normalMaterial.color = Color.white;
            
            hitMaterial = new Material(Shader.Find("Standard"));
            hitMaterial.color = Color.red;
            
            destroyedMaterial = new Material(Shader.Find("Standard"));
            destroyedMaterial.color = Color.gray;
            
            targetRenderer.material = normalMaterial;
        }
        
        #endregion
        
        #region Damage System
        
        public void TakeDamage(float damage)
        {
            if (isDestroyed) return;
            
            health -= damage;
            
            // 히트 효과
            PlayHitEffect();
            PlayHitSound();
            
            if (useHitAnimation)
            {
                StartHitAnimation();
            }
            
            // 체력에 따른 머티리얼 변경
            UpdateMaterial();
            
            Debug.Log($"타겟이 {damage} 데미지를 받았습니다. 남은 체력: {health}");
            
            // 체력이 0 이하가 되면 파괴
            if (health <= 0)
            {
                DestroyTarget();
            }
        }
        
        private void UpdateMaterial()
        {
            if (targetRenderer == null) return;
            
            float healthPercent = health / maxHealth;
            
            if (healthPercent <= 0.3f)
            {
                targetRenderer.material = destroyedMaterial;
            }
            else if (healthPercent <= 0.7f)
            {
                targetRenderer.material = hitMaterial;
            }
            else
            {
                targetRenderer.material = normalMaterial;
            }
        }
        
        private void DestroyTarget()
        {
            if (isDestroyed) return;
            
            isDestroyed = true;
            
            // 파괴 효과
            PlayDestroyEffect();
            PlayDestroySound();
            
            // 머티리얼을 파괴된 상태로 변경
            if (targetRenderer != null)
            {
                targetRenderer.material = destroyedMaterial;
            }
            
            Debug.Log("타겟이 파괴되었습니다!");
            
            // 파괴 가능한 타겟이면 리스폰
            if (isDestructible)
            {
                StartRespawn();
            }
        }
        
        private void StartRespawn()
        {
            if (respawnCoroutine != null)
            {
                StopCoroutine(respawnCoroutine);
            }
            respawnCoroutine = StartCoroutine(RespawnCoroutine());
        }
        
        private IEnumerator RespawnCoroutine()
        {
            yield return new WaitForSeconds(respawnTime);
            
            // 타겟 리스폰
            health = maxHealth;
            isDestroyed = false;
            
            // 머티리얼 복원
            if (targetRenderer != null)
            {
                targetRenderer.material = normalMaterial;
            }
            
            // 스케일 복원
            transform.localScale = originalScale;
            
            Debug.Log("타겟이 리스폰되었습니다!");
        }
        
        #endregion
        
        #region Visual Effects
        
        private void PlayHitEffect()
        {
            if (hitEffect != null)
            {
                hitEffect.Play();
            }
        }
        
        private void PlayDestroyEffect()
        {
            if (destroyEffect != null)
            {
                destroyEffect.Play();
            }
        }
        
        private void StartHitAnimation()
        {
            if (hitAnimationCoroutine != null)
            {
                StopCoroutine(hitAnimationCoroutine);
            }
            hitAnimationCoroutine = StartCoroutine(HitAnimationCoroutine());
        }
        
        private IEnumerator HitAnimationCoroutine()
        {
            Vector3 hitScale = originalScale * hitAnimationScale;
            
            // 확대
            float elapsed = 0f;
            while (elapsed < hitAnimationDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (hitAnimationDuration * 0.5f);
                transform.localScale = Vector3.Lerp(originalScale, hitScale, t);
                yield return null;
            }
            
            // 축소
            elapsed = 0f;
            while (elapsed < hitAnimationDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (hitAnimationDuration * 0.5f);
                transform.localScale = Vector3.Lerp(hitScale, originalScale, t);
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
        
        #endregion
        
        #region Audio
        
        private void PlayHitSound()
        {
            if (hitSound != null)
            {
                hitSound.Play();
            }
        }
        
        private void PlayDestroySound()
        {
            if (destroySound != null)
            {
                destroySound.Play();
            }
        }
        
        #endregion
        
        #region Collision Detection
        
        void OnCollisionEnter(Collision collision)
        {
            // 총알과 충돌했는지 확인
            if (collision.gameObject.CompareTag("Bullet") || 
                collision.gameObject.GetComponent<BulletEffectController>() != null)
            {
                // 충돌 강도에 따른 데미지 계산
                float damage = CalculateDamage(collision);
                TakeDamage(damage);
                
                Debug.Log($"총알과 충돌! 데미지: {damage}");
            }
        }
        
        private float CalculateDamage(Collision collision)
        {
            // 충돌 속도에 따른 데미지 계산
            float impactForce = collision.relativeVelocity.magnitude;
            float damage = impactForce * 10f; // 충돌 속도 * 10
            
            // 최소/최대 데미지 제한
            damage = Mathf.Clamp(damage, 10f, 50f);
            
            return damage;
        }
        
        #endregion
        
        #region Public Methods
        
        public void SetHealth(float health)
        {
            this.health = Mathf.Clamp(health, 0f, maxHealth);
            UpdateMaterial();
        }
        
        public void SetMaxHealth(float maxHealth)
        {
            this.maxHealth = maxHealth;
            health = Mathf.Min(health, maxHealth);
            UpdateMaterial();
        }
        
        public void ResetTarget()
        {
            health = maxHealth;
            isDestroyed = false;
            
            if (targetRenderer != null)
            {
                targetRenderer.material = normalMaterial;
            }
            
            transform.localScale = originalScale;
            
            if (respawnCoroutine != null)
            {
                StopCoroutine(respawnCoroutine);
                respawnCoroutine = null;
            }
        }
        
        public float GetHealthPercent()
        {
            return health / maxHealth;
        }
        
        public bool IsAlive()
        {
            return !isDestroyed && health > 0;
        }
        
        #endregion
        
        #region Debug
        
        void OnGUI()
        {
            if (!showDebugInfo) return;
            
            // 타겟 위에 체력바 표시
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
            
            if (screenPos.z > 0)
            {
                float barWidth = 100f;
                float barHeight = 10f;
                float healthPercent = GetHealthPercent();
                
                // 배경
                GUI.color = Color.black;
                GUI.Box(new Rect(screenPos.x - barWidth/2, Screen.height - screenPos.y - barHeight/2, barWidth, barHeight), "");
                
                // 체력바
                GUI.color = Color.Lerp(Color.red, Color.green, healthPercent);
                GUI.Box(new Rect(screenPos.x - barWidth/2, Screen.height - screenPos.y - barHeight/2, barWidth * healthPercent, barHeight), "");
                
                // 텍스트
                GUI.color = Color.white;
                GUI.Label(new Rect(screenPos.x - barWidth/2, Screen.height - screenPos.y - barHeight/2 - 15, barWidth, 15), $"HP: {health:F0}/{maxHealth:F0}");
            }
        }
        
        void OnDrawGizmosSelected()
        {
            // 타겟 범위 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
            
            // 체력에 따른 색상 변경
            if (Application.isPlaying)
            {
                float healthPercent = GetHealthPercent();
                Gizmos.color = Color.Lerp(Color.red, Color.green, healthPercent);
                Gizmos.DrawWireSphere(transform.position, 1f);
            }
        }
        
        #endregion
    }
} 
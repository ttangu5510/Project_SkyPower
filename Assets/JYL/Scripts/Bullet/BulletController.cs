using System;
using UnityEngine;

namespace JYL
{
    public class BulletController : MonoBehaviour
    {
        [Range(0.05f, 1f)][SerializeField] float ticTime = 0.5f;
        [SerializeField] GameObject flash; // TODO: 총알 프리팹 하나하나 다 직접 달아줘야 함
        [SerializeField] GameObject hit;
        public Rigidbody rig;
        private Collider col;
        private ParticleSystem ps;
        public int attackPower;
        public bool canDeactive = true;
        private float timer;
        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            if(ps == null)
            {
                Debug.LogError("파티클 시스템이 없음");
            }
            rig = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            rig.constraints = RigidbodyConstraints.FreezeRotation;
            rig.useGravity = false;
            col.isTrigger = true;
            timer = ticTime;
        }
        private void OnEnable()
        {
            timer = ticTime;
            OnFire();
        }
        private void Update()
        {
            if (!canDeactive)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else if (timer < 0)
                {
                    timer = ticTime;
                }
            }
        }
        //private void OnTriggerStay(Collider other)
        //{
        //if(gameObject.layer == 7) // 플레이어
        //{
        //    //조건에 따라서 SetActive false
        //    // 적한테 테이크 데미지
        //    Enemy enemy = other.GetComponent<Enemy>();
        //    if (enemy == null)
        //    {
        //        gameObject.SetActive(false);
        //        return;
        //    }
        //    if (!canDeactive && timer <= 0)
        //    {
        //        enemy.TakeDamage(attackPower);
        //    }
        //    else if (canDeactive)
        //    {
        //        enemy.TakeDamage(attackPower);
        //        gameObject.SetActive(false);
        //    }
        //}
        //else if(gameObject.layer == 9)
        //{
        //    // 적이 총알 쏜거 처리
        //    PlayerController enemy =other.GetComponent<PlayerController>();
        //    if (enemy == null)
        //    {
        //        gameObject.SetActive(false);
        //        return;
        //    }
        //    if (!canDeactive && timer <= 0)
        //    {
        //        //enemy.TakeDamage(attackPower); // TODO:플레이어 구현필요
        //    }
        //    else if (canDeactive)
        //    {
        //        //enemy.TakeDamage(attackPower); // TODO:플레이어 구현필요
        //        gameObject.SetActive(false);
        //    }
        //}

        private void OnTriggerStay(Collider other)
        {
            if (gameObject.layer == 7) // 플레이어의 총알일 경우
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy == null)
                {
                    SpawnHitEffect(other.transform);
                    gameObject.SetActive(false);
                    return;
                }
                if (!canDeactive && timer <= 0)
                {
                    enemy.TakeDamage(attackPower);
                    SpawnHitEffect(other.transform);
                }
                else if (canDeactive)
                {
                    enemy.TakeDamage(attackPower);
                    SpawnHitEffect(other.transform);
                    gameObject.SetActive(false);
                }
            }
            Debug.Log("트리거 인식");
            if (gameObject.layer == 9) // 에너미의 총알일 경우
            {
                Debug.Log("총알이 적의 총알일 경우.");
                PlayerController player = other.GetComponent<PlayerController>();
                if (player == null)
                {
                    SpawnHitEffect(other.transform);
                    gameObject.SetActive(false);
                    return;
                }
                if (!canDeactive && timer <= 0)
                {
                    //player.TakeDamage(attackPower); // TODO:플레이어 구현필요
                    SpawnHitEffect(other.transform);
                }
                else if (canDeactive)
                {
                    //player.TakeDamage(attackPower); // TODO:플레이어 구현필요
                    SpawnHitEffect(other.transform);
                    gameObject.SetActive(false);
                }
            }
        }
        private void OnDisable(){ }
        private void OnFire()
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
            if (flash != null)
            {
                GameObject flashInstance = Instantiate(flash, transform.position, Quaternion.identity,transform);
                flashInstance.transform.forward = gameObject.transform.forward;
                if(flashInstance == null)
                {
                    Debug.Log("플래시 게임오브젝트 생성 실패 NUll");
                }
                ParticleSystem flashPs = flashInstance.GetComponent<ParticleSystem>();
                if (flashPs != null)
                {
                    Destroy(flashInstance, flashPs.main.duration);
                }
                else if(flashPs == null)
                {
                    Debug.Log("플래시 파티클 시스템 컴포넌트가 NUll");
                    ParticleSystem flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(flashInstance, flashPsParts.main.duration);
                    if(flashPsParts == null)
                    {
                        Debug.Log("플래시의 자식도 파티클 시스템 컴포넌트가 Null");
                    }
                    
                }
                
            }
            else if(flash == null)
            {
                Debug.Log($"플래시 프리팹 참조가 NUll");
            }
        }

        private void SpawnHitEffect(Transform parent)
        {
            if (hit != null)
            {
                // 충돌 위치와 방향 계산
                Vector3 hitPos = transform.position;
                Quaternion hitRot = Quaternion.identity;
                if (parent != null)
                {
                    hitRot = Quaternion.LookRotation(parent.position - transform.position);
                }
                else if(parent == null)
                {
                    Debug.Log("충돌체가 null임");
                }
                GameObject hitInstance = Instantiate(hit, hitPos, hitRot, parent);
                if(hitInstance == null)
                {
                    Debug.Log("히트 게임 오브젝트 생성 실패");
                }
                ParticleSystem hitPs = hitInstance.GetComponent<ParticleSystem>();
                if (hitPs != null)
                {
                    hitPs.Play();
                    Destroy(hitInstance, hitPs.main.duration);
                }
                else if(hitPs == null)
                {
                    Debug.Log("히트 파티클 시스템이 Null임");
                    if (hitInstance.transform.childCount > 0)
                    {
                        var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                        if (hitPsParts == null)
                        {
                            Debug.Log("히트 : 자식에게서도 파티클 시스템을 찾을 수 없음");
                        }
                        hitPsParts.Play();
                        Destroy(hitInstance, hitPsParts.main.duration);
                    }
                }

            }
            else
            {
                Debug.Log("히트 게임오브젝트 참조가 null");
            }
        }
    }
}

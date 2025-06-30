using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;
using Unity.VisualScripting.Antlr3.Runtime;

[CreateAssetMenu(fileName = "FanShapeShot", menuName = "ScriptableObject/BulletPattern/FanShapeShot")]
public class FanShapeShot : BulletPatternData
{
    [Header("Fan Shape Shot Settings")]
    public int shotCount = 5; // 한 번에 발사할 총알의 개수
    public float fireDelay = 1f;
    public float fireDelayBetweenShots = 0f;
    public float fanShapeangle = 90;
    public override IEnumerator Shoot(Transform[] firePoints, GameObject bulletPrefab, float bulletSpeed)
    {
        // TODO : ReturnToPool()호출 타이밍 생각해야함. => 플레이어와 충돌 or 시간이 지날 때 ReturnToPool()해야하나?
        while (true)
        {
            for (int i = 0; i < shotCount; i++)
            {
                BulletPrefabController bullet = objectPool.ObjectOut() as BulletPrefabController;
                float angle = i * (fanShapeangle / (shotCount - 1)) - (fanShapeangle / 2);
                firePoints[0].rotation = Quaternion.Euler(0, angle, 0); // Y축을 기준으로 회전
                firePoints[0].forward = firePoints[0].rotation * Vector3.forward; // 회전된 방향으로 총구를 향하게 함
                // Debug.Log($"FanShapeShot angle : {angle}");
                // Debug.Log($"FanShapeShot firePoint.forward : {firePoints[0].forward}");
                if (bullet != null)
                {
                    bullet.transform.position = firePoints[0].position;

                    foreach (BulletInfo info in bullet.bullet)
                    {
                        if (info.rig != null)
                        {
                            info.trans.gameObject.SetActive(true);
                            info.trans.position = firePoints[0].position;
                            info.rig.velocity = Vector3.zero;
                            info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
                yield return new WaitForSeconds(fireDelayBetweenShots); // 여기서 간격을 두어 다른 모양으로 변경 가능.
            }
            yield return new WaitForSeconds(fireDelay);
        }
    }
}

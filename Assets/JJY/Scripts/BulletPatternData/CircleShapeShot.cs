using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "CircleShapeShot", menuName = "ScriptableObject/BulletPattern/CircleShapeShot")]

public class CircleShapeShot : BulletPatternData
{
    [Header("Circle Shape Shot Settings")]
    public int shotCount = 8; // 한 번에 발사할 총알의 개수 : 총구 개수의 배수, 총구 개수보다 많아야할듯.
    public float fireDelay = 1f;
    public float fireDelayBetweenShots = 0.1f;
    public override IEnumerator Shoot(Transform[] firePoints, GameObject bulletPrefab, float bulletSpeed)
    {
        // TODO : ReturnToPool()호출 타이밍 생각해야함. => 플레이어와 충돌 or 시간이 지날 때 ReturnToPool()해야하나?
        while (true)
        {
            for (int i = 0; i < shotCount; i++)
            {
                BulletPrefabController bullet = objectPool.ObjectOut() as BulletPrefabController;

                if (bullet != null)
                {
                    bullet.transform.position = firePoints[i].position;

                    foreach (BulletInfo info in bullet.bullet)
                    {
                        if (info.rig != null)
                        {
                            info.trans.gameObject.SetActive(true);
                            info.trans.position = firePoints[0].position;
                            info.rig.velocity = Vector3.zero;
                            info.rig.AddForce(firePoints[i].forward * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
                yield return new WaitForSeconds(fireDelayBetweenShots);
            }
            yield return new WaitForSeconds(fireDelay);
        }
    }
}

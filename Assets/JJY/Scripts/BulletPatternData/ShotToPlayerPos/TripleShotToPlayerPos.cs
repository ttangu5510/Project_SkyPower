using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System;
using JYL;

[CreateAssetMenu(fileName = "TripleShotToPlayerPos", menuName = "ScriptableObject/BulletPattern/TripleShotToPlayerPos")]
public class TripleShotToPlayerPos : BulletPatternData
{
    public int shotCount = 3;
    public float delayBetweenshots = 0.1f;
    public float fireDelay = 2f;
    Vector3 playerPos;
    public override IEnumerator Shoot(Transform[] firePoints, GameObject bulletPrefab, float bulletSpeed)
    {
        // TODO : ReturnToPool()호출 타이밍 생각해야함. => 플레이어와 충돌 or 시간이 지날 때 ReturnToPool()해야하나?
        while (true)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            firePoints[0].LookAt(playerPos);

            for (int i = 0; i < shotCount; i++)
            {
                BulletPrefabController bullet = objectPool.ObjectOut() as BulletPrefabController;

                if (bullet != null)
                {
                    bullet.transform.position = firePoints[0].position;
                    bullet.ReturnToPool(5f);

                    foreach (BulletInfo info in bullet.bullet)
                    {
                        if (info.rig != null)
                        {
                            info.trans.gameObject.SetActive(true);
                            info.trans.localPosition = info.originPos;
                            // info.trans.position = firePoints[0].position;
                            info.rig.velocity = Vector3.zero;
                            info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
                yield return new WaitForSeconds(delayBetweenshots);
            }
            yield return new WaitForSeconds(fireDelay);
        }
    }
}


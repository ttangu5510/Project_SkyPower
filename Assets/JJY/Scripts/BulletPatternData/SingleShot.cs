using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

public class SingleShot : BulletPatternData
{
    private float fireDelay = 1f;
    public override IEnumerator Shoot(Transform firePoint, GameObject bulletPrefab, float bulletSpeed)
    {
        BulletPrefabController bullets = objectPool.ObjectOut() as BulletPrefabController;
        while (true)
        {
            // TODO : 오브젝트풀로 돌아가는 타이밍 다시 생각
            // bullet.ReturnToPool(bulletReturnTimer);
            foreach (BulletInfo info in bullets.bullet)
            {
                if (info.rig != null)
                {
                    info.trans.gameObject.SetActive(true);

                    // TODO : 총구쪽으로 모든 총알이 모이니, 여러 총알일때는 수정해야함.
                    // bullet.transform.position = firePoint.position;
                    info.trans.position = firePoint.position;
                    info.rig.velocity = Vector3.zero;
                    info.rig.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
                }
                yield return new WaitForSeconds(fireDelay);
            }
            yield return null;
        }
    }
}


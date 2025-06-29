using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SingleShotData", menuName = "BulletPattern/SingleShotData")]
public class SingleShot : BulletPatternData
{
    public override void Shoot(Transform firePoint, GameObject bulletPrefab, float bulletSpeed)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
            Debug.Log("Single Shot : 빵야");
        }
        else Debug.Log("Single Shot Error");
    }
}


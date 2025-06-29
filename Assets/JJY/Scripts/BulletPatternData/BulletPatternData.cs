using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPatternData : ScriptableObject
{
    public abstract void Shoot(Transform firePoint, GameObject bulletPrefab, float bulletSpeed);
}

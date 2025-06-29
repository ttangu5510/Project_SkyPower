using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Set Scriptable Object")]
        [SerializeField] PlayerModel playerModel;

        [Header("Set References")]
        [SerializeField] Transform muzzlePoint;

        [Header("Set Value")]
        [SerializeField] float bulletReturnTimer = 2f;
        [SerializeField] List<ObjectPool> bulletPools;

        private int poolIndex = 0;
        private ObjectPool curBulletPool => bulletPools[poolIndex];

        private void Awake()
        {
        }
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (poolIndex)
                {
                    case 0:
                        Fire1();
                        break;
                    case 1:
                        Fire1();
                        break;

                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                poolIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                poolIndex = 1;
            }
        }

        private void PlayerMove()
        {

        }

        private void Fire1()
        {
            BulletPrefabController bullet = curBulletPool.ObjectOut() as BulletPrefabController;
            bullet.transform.position = muzzlePoint.position;
            bullet.ReturnToPool(bulletReturnTimer);
            foreach(BulletInfo info in bullet.bullet)
            {
                if(info.rig == null)
                {
                    continue;
                }
                info.trans.gameObject.SetActive(true);
                info.trans.position = info.originPos;
                info.rig.velocity = Vector3.zero;
                info.rig.AddForce(playerModel.fireSpeed * muzzlePoint.forward, ForceMode.Impulse);
            }
        }
        private void Fire2()
        {

        }
    }
}
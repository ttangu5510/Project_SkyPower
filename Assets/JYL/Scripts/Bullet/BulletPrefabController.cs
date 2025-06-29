using UnityEngine;

namespace JYL
{
    public struct BulletInfo
    {
        public Transform trans;
        public Rigidbody rig;
        public Vector3 originPos;
    }
    public class BulletPrefabController : PooledObject
    {
        public BulletInfo[] bullet;
        private Transform[] transforms;
        private int index = 0;
        private void Awake()
        {
            transforms = GetComponentsInChildren<Transform>();
            bullet = new BulletInfo[transforms.Length];
            for(int i =0; i< bullet.Length;i++)
            {
                bullet[i].trans = transforms[i];
                bullet[i].rig = transforms[i].GetComponent<Rigidbody>();
                bullet[i].originPos = transforms[i].position;
            }
        }
        void Start()
        {

        }

        void Update()
        {

        }
    }
}


using UnityEngine;

namespace JYL
{
    // 총알들의 정보를 담는 구조체
    public struct BulletInfo
    {
        public Transform trans;
        public Rigidbody rig;
        public Vector3 originPos;
        public BulletController bulletController;
        public bool canDeactive;
    }

    public class BulletPrefabController : PooledObject
    {
        public ObjectPool objectPool; // 여러 종류의 Enemy에서 같은 BulletPattern을 사용할 때, 서로 다른 ObjetPool을 사용할 때 구분하기 위해 필요함.
        public BulletInfo[] bulletInfo;
        private Transform[] transforms;
        
        private void Awake()
        {
            transforms = GetComponentsInChildren<Transform>();
            bulletInfo = new BulletInfo[transforms.Length];
            InitBulletPrefab();

        }
        public void InitBulletPrefab()
        {
            for (int i = 0; i < bulletInfo.Length; i++)
            {
                bulletInfo[i].rig = transforms[i].GetComponent<Rigidbody>();
                bulletInfo[i].bulletController = GetComponent<BulletController>();
                bulletInfo[i].trans = transforms[i];
                bulletInfo[i].originPos = transforms[i].localPosition;
                bulletInfo[i].canDeactive = true; // 기본값은 true로 설정
            }
        }
    }
}


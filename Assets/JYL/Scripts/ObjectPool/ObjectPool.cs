using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyData; // 

namespace JYL
{
    public class ObjectPool : MonoBehaviour
    {
        public EnemyType enemyType;
        [Header("Set References")]
        [SerializeField] public PooledObject poolObject;

        [Header("Set Value")]
        [Range(1, 100)][SerializeField] int poolSize = 5;

        private Stack<PooledObject> pool;
        private Coroutine returnRoutine;

        private void Awake()
        {
            CreatePool();
        }
        public void CreatePool()
        {
            pool = new Stack<PooledObject>();
            for (int i = 0; i < poolSize; i++)
            {
                PooledObject go = Instantiate(poolObject, transform);
                go.returnPool = this;
                go.gameObject.SetActive(false);
                pool.Push(go);
            }
        }
        public PooledObject ObjectOut()
        {
            PooledObject go;

            if (pool.Count > 0)
            {
                go = pool.Pop();
            }
            else
            {
                go = Instantiate(poolObject, transform);
            }

            go.returnPool = this;
            go.gameObject.SetActive(true);
            return go;
        }
        public void ReturnToPool(PooledObject obj, float returnTime = 0f)
        {
            returnRoutine = StartCoroutine(ReturnRoutine(obj, returnTime));
        }
        IEnumerator ReturnRoutine(PooledObject obj, float returnTime)
        {
            yield return new WaitForSeconds(returnTime);
            obj.gameObject.SetActive(false);
            pool.Push(obj);
            returnRoutine = null;
        }
        public void ClearPool()
        {
            while (pool.Count > 0)
            {
                PooledObject obj = pool.Pop();
                Destroy(obj.gameObject);
            }
            pool.Clear();
        }
    }
}



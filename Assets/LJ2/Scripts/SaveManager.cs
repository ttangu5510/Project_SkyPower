using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

namespace LJ2
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager instance;
        public static SaveManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // 세이브할 정보 목록 : SaveData 를 상속 받아야 함
        public PlayerSave player;


        // 정보 별 저장, 로드, 삭제 함수 따로 구현
        public void PlayerSave(int index)
        {
            DataSaveController.Save(player.saveDataSample, index);
        }

        public void PlayerLoad(int index)
        {
            DataSaveController.Load(ref player.saveDataSample, index);
        }

        public void PlayerDelete(int index)
        {
            DataSaveController.Delete(player.saveDataSample, index);
        }
    }
}

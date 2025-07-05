using LJ2;
using System;
using UnityEngine;

namespace JYL
{
    public class CharacterSaveLoader : MonoBehaviour
    {
        public CharactorController[] charactorController;
        public CharactorController mainController;
        public CharactorController sub1Controller;
        public CharactorController sub2Controller;
        // public EquipSaveLoader equipLoader;
        private string charPrefabPath = "CharacterPrefabs";

        // TODO : 없어도 되는지 테스트
        //void OnEnable()
        //{
        //    GetCharPrefab();
        //}

        void Update() { }
        public void GetCharPrefab()
        {
            //캐릭터 프리팹 전부 가져오기
            charactorController = Resources.LoadAll<CharactorController>(charPrefabPath);
            foreach (CharactorController cont in charactorController)
            {
                cont.SetParameter(); // TODO : 이큅로더 완성되면 여기에 넣음 equipLoader
                switch(cont.partySet)
                {
                    case PartySet.Main:
                        mainController = cont;
                        break;
                    case PartySet.Sub1:
                        sub1Controller = cont;
                        break;
                    case PartySet.Sub2:
                        sub2Controller = cont;
                        break;
                }
            }
            // 전부 셋파라매터 함.
            Array.Sort(charactorController, (a, b) => a.partySet.CompareTo(b.partySet)); // 추가적인 정렬도 가능함.

        }
    }
}

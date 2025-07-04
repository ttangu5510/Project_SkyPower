using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KYG_skyPower;
using JYL;
using Unity.VisualScripting;

namespace LJ2
{
    [System.Serializable]
    public class CharactorController : MonoBehaviour
    {
        public CharacterData characterData;

        public Parrying parrying;
        public Ultimate ultimate;

        public int id => characterData.id;
        public Grade grade;
        public string charName;
        public Elemental elemental;

        public int level;
        public int step;
        public int exp;

        public int Hp;
        public int attackDamage;
        public float attackSpeed;
        public float moveSpeed;
        public int defense;
        public PartySet partySet;

        public int ultLevel;
        public float ultDamage;
        public int ultCool;

        public PooledObject bulletPrefab; // TODO : 경로지정
        public PooledObject ultBulletPrefab;
        public GameObject ultPrefab; // 리소스

        public Parry parry;
        public int parryCool;

        public Sprite icon;
        public Sprite image;

        public int upgradeUnit;

        private void Awake()
        {
            parrying = GetComponent<Parrying>();
            ultimate = GetComponent<Ultimate>();
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    id = characterData.id;
            //    SetParameter();
            //}

            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    LevelUp(5000);  // 5000은 예시로, 실제 게임에서는 플레이어가 가진 유닛 수에 따라 다르게 설정해야 함
            //    SetParameter();
            //}

            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    StepUp();
            //    SetParameter();
            //}
        }
        public void SetParameter()
        {
            // Data의 값을 그대로 가져옴
            // bulletPrefab = characterData.bulletPrefab;
            // ultPrefab = characterData.ultVisual;
            // image = charictorData.image;

            grade = characterData.grade;
            charName = characterData.characterName;
            elemental = characterData.elemental;
            attackSpeed = characterData.attackSpeed;
            moveSpeed = characterData.moveSpeed;
            defense = characterData.defense;
            image = characterData.image;
            icon = characterData.icon;
            image = characterData.image;


            // Save의 값을 그대로 가져옴  

            CharacterSave characterSave = Manager.Game.saveFiles[Manager.Game.currentSaveIndex].characterInventory.characters.Find(c => c.id == id);

            if (characterSave.id == 0)
            {
                Debug.LogWarning($"id {id}에 해당하는 CharacterSave를 찾을 수 없습니다.");
                return;
            }

            //Debug.Log($"Character ID: {characterSave.id}, Step: {characterSave.step}, Level : {characterSave.level}");
            level = characterSave.level;
            step = characterSave.step;
            bulletPrefab = Resources.Load<PooledObject>($"Prefabs/bullet/{id}_{step}");
            partySet = characterSave.partySet;

            // Save의 값에 따라 Data의 값을 변경
            Hp = characterData.hp + (characterData.hpPlus * (level - 1));
            attackDamage = (int)(characterData.attackDamage + (characterData.damagePlus * (level - 1)));
            ultLevel = step + 1;
            ultCool = characterData.ultCoolDefault - (characterData.ultCoolReduce * step);
            upgradeUnit = characterData.upgradeUnitDefault + (characterData.upgradeUnitPlus * level);

            switch (id)
            {
                case 10001:
                    ultDamage = characterData.attackDamage * ((150 + 25 * Mathf.Pow(step, 2)) / 100);
                    break;
                case 10002:
                    ultDamage = characterData.attackDamage * ((120 + 20 * step) / 100);
                    break;
                case 10003:
                    ultDamage = characterData.attackDamage * ((150 + 50 * step) / 100);
                    break;
                case 10004:
                    ultDamage = characterData.attackDamage * ((130 + 30 * step) / 100);
                    break;
                case 10005:
                    ultDamage = characterData.attackDamage * ((150 + (12.5f * Mathf.Pow(step, 2)) + (37.5f * step)) / 100);
                    break;
                case 10006:
                    ultDamage = characterData.attackDamage * ((150 + (12.5f * Mathf.Pow(step, 2)) + (37.5f * step)) / 100);
                    break;
                default:
                    ultDamage = characterData.attackDamage * ((150 + 50 * step) / 100);
                    break;
            }
        }

        // 업그레이드 가능할 때만 실행
        public void LevelUp(int unit)
        {
            if (level < characterData.maxLevel)
            {
                
                if (unit > upgradeUnit)
                {
                    unit -= upgradeUnit;
                    level++;

                    int index = Manager.Game.CurrentSave.characterInventory.characters.FindIndex(c => c.id == id);
                    CharacterSave characterSave = Manager.Game.CurrentSave.characterInventory.characters[index];
                    characterSave.level = level;
                    Manager.Game.CurrentSave.characterInventory.characters[index] = characterSave;
                }
                else
                {
                    Debug.Log("업그레이드 유닛이 부족합니다.");
                }

            }
            else
            {
                Debug.Log("최대 레벨에 도달했습니다.");
            }
        }

        public void GetUpgradeUnit(int unit)
        {
            exp += unit;
            if (exp > upgradeUnit)
            {
                Debug.Log("업그레이드 가능합니다.");
            }
            else
            {
                Debug.Log("업그레이드 가능 유닛이 부족합니다.");
            }
        }
        public void StepUp() 
        {
            if (step < 4)
            {
                step++;

                int index = Manager.Game.CurrentSave.characterInventory.characters.FindIndex(c => c.id == id);
                CharacterSave characterSave = Manager.Game.CurrentSave.characterInventory.characters[index];
                characterSave.step = step;
                Manager.Game.CurrentSave.characterInventory.characters[index] = characterSave;
            }
            else
            {
                Debug.Log("최대 단계에 도달했습니다.");
            }
        }

        public void UseParry()
        {
            // Parry 기능을 사용할 때마다 쿨타임을 체크하고 실행
            switch (parry)
            {
                case Parry.방어막:
                    parrying.Parry();
                    defense += parrying.Shield();
                    break;
                case Parry.반사B:
                    parrying.Parry();
                    // 반사 기능 미구현
                    break;
                case Parry.무적:
                    parrying.Parry();
                    parrying.Invicible();
                    break;
            }
        }

        public void UseUlt()
        {
            switch(id)
            {                
                case 10001:
                    ultimate.Laser(ultDamage);
                    break;
                case 10002:
                    // 유도탄 미구현
                    break;
                case 10003:
                    // 탄막 변경 데미지 증가
                    // ultimate.BulletUpgrade();
                    break;
                case 10004:
                    // 궁극기 탄막 1회 - 다단히트
                    // ultimate.BigBullet(ultDamage);
                    break;
                case 10005:
                    // 궁극기 탄막 1회 - 다단히트
                    // ultimate.BigBullet(ultDamage);
                    break;
                case 10006:
                    defense += ultimate.Shield(ultDamage);
                    break;
                default:
                    ultimate.AllAttack(ultDamage);
                    break;
            }
        }
    }
}

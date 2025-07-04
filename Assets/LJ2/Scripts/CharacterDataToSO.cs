using IO;
using LJ2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterDataToSO : MonoBehaviour
{
    [SerializeField] private CsvTable table;
    private CharacterData characterData;


    private void Start()
    {
        CsvReader.Read(table);
        MakeCharictor();
    }

    private void MakeCharictor()
    {
        
        for (int i = 2; i < table.Table.GetLength(0); i++)
        {
            characterData = ScriptableObject.CreateInstance<CharacterData>();

            characterData.id = int.Parse(table.GetData(i, 0));

            Enum.TryParse<Grade>(table.GetData(i, 1), out characterData.grade);
            characterData.characterName = table.GetData(i, 2);
            //Debug.Log(characterData.name);
            Enum.TryParse<Elemental>(table.GetData(i, 5), out characterData.elemental);
            
            characterData.maxLevel = int.Parse(table.GetData(i, 7));
            characterData.hp = int.Parse(table.GetData(i, 8));
            characterData.hpPlus = int.Parse(table.GetData(i, 9));

            characterData.attackDamage = float.Parse(table.GetData(i, 11));
            characterData.damagePlus = float.Parse(table.GetData(i, 12));
            characterData.attackSpeed = float.Parse(table.GetData(i, 13));
            characterData.moveSpeed = float.Parse(table.GetData(i, 14));
            characterData.defense = int.Parse(table.GetData(i, 15));

            characterData.ultCoolDefault = int.Parse(table.GetData(i, 18));
            characterData.ultCoolReduce = int.Parse(table.GetData(i, 19));
            //characterData.ultLore = table.GetData(i, 21);
            //characterData.ultVisual = (GameObject)AssetDatabase.LoadAssetAtPath($"정해진 경로/{table.GetData(i, 22)}.Prefab", typeof(GameObject));

            Enum.TryParse<Parry>(table.GetData(i, 23), out characterData.parry);
            Debug.Log(characterData.parry);
            /* TryParse Debug 코드
            string raw = table.GetData(i, 23);
            string clean = raw.Trim();

            if (Enum.TryParse<Parry>(clean, out characterData.parry))
            {
                Debug.Log($"정상 파싱: '{clean}' → {characterData.parry}");
            }
            else
            {
                Debug.LogWarning($"파싱 실패: '{clean}' → 기본값 사용됨");
            }
            */
            int.TryParse(table.GetData(i, 24), out characterData.parryCool);
            

            //charictorData.image = (Sprite)AssetDatabase.LoadAssetAtPath($"이미지 파일들 경로/{table.GetData(i, 26)}.Prefab", typeof(Sprite));

            characterData.upgradeUnitDefault = int.Parse(table.GetData(i, 27));
            characterData.upgradeUnitPlus = int.Parse(table.GetData(i, 28));

            string assetPath = $"Assets/LJ2/Scripts/Charictor/{characterData.characterName}.asset";
            AssetDatabase.CreateAsset(characterData, assetPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

using JYL;
using LJ2;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterDataHolderPrefabCreator
{
#if UNITY_EDITOR


    [MenuItem("Tools/CharacterDataHolder 프리팹 자동생성")]
    public static void CreatePrefabsForAllCharacterData()
    {
        // 모든 CharacterData ScriptableObject 에셋 경로 찾기  
        string[] guids = AssetDatabase.FindAssets("t:CharacterData", new[] { "Assets/LJ2/Scripts/Charictor" });
        var saveDir = "Assets/Resources/CharacterPrefabs";
        if (!AssetDatabase.IsValidFolder(saveDir))
            AssetDatabase.CreateFolder("Assets/LJ2/Prefabs", "CharacterDataHolders");

        var erasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Bullet/UltBullet/Erase.Prefab");
        var laserPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Bullet/UltBullet/Laser.Prefab");
        var shieldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Bullet/UltBullet/Shield.Prefab");
        var firePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Bullet/UltBullet/Fire.Prefab");

        foreach (var guid in guids)
        {
            var data = AssetDatabase.LoadAssetAtPath<CharacterData>(AssetDatabase.GUIDToAssetPath(guid));
            if (data == null) continue;

            // 빈 GameObject 생성 및 CharactorController 컴포넌트 추가
            GameObject go = new GameObject($"{data.characterName}");
            var holder = go.AddComponent<LJ2.CharactorController>();  
            holder.characterData = data;

            // 필요한 경우 추가 컴포넌트 설정
            var parry = go.AddComponent<Parrying>();  // Parrying 컴포넌트 추가
            holder.parrying = parry;  // CharactorController에 Parrying 컴포넌트 연결
            var ultimate = go.AddComponent<Ultimate>();  // Ultimate 컴포넌트 추가
            holder.ultimate = ultimate;  // CharactorController에 Ultimate 컴포넌트 연결
            holder.bulletPrefab = data.bulletPrefab.GetComponent<BulletPrefabController>(); // Bullet 프리팹 연결

            ultimate.ultAll = erasePrefab; // Erase 프리팹 연결
            ultimate.ultLaser = laserPrefab; // Laser 프리팹 연결
            ultimate.ultFire = firePrefab; // Fire 프리팹 연결
            ultimate.shield = shieldPrefab; // Shield 프리팹 연결

            var characterModel = (GameObject)PrefabUtility.InstantiatePrefab(data.characterModel);
            var eraseObject = (GameObject)PrefabUtility.InstantiatePrefab(erasePrefab);
            var laserObject = (GameObject)PrefabUtility.InstantiatePrefab(laserPrefab);
            var shieldObject = (GameObject)PrefabUtility.InstantiatePrefab(shieldPrefab);
            var fireObject = (GameObject)PrefabUtility.InstantiatePrefab(firePrefab);

            characterModel.transform.SetParent(go.transform); // CharacterModel 프리팹을 생성된 GameObject의 자식으로 설정
            eraseObject.transform.SetParent(go.transform); // Erase 프리팹을 생성된 GameObject의 자식으로 설정
            laserObject.transform.SetParent(go.transform); // Laser 프리팹을 생성된 GameObject의 자식으로 설정
            shieldObject.transform.SetParent(go.transform); // Shield 프리팹을 생성된 GameObject의 자식으로 설정
            fireObject.transform.SetParent(go.transform); // Fire 프리팹을 생성된 GameObject의 자식으로 설정

            string prefabPath = $"{saveDir}/{data.characterName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);
        }

        Debug.Log("각 CharacterData별로 CharacterDataHolder 프리팹 자동생성 완료");
    }
    #endif


}

using KYG_skyPower;
using LJ2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KYG_skyPower
{
#if UNITY_EDITOR
public class EquipmentCSVtoSO
{
    [MenuItem("Tools/CSV/Convert Equipment Table to SO")]
    public static void Convert()
    {
        string csvPath = "Assets/Resources/CSV/Equipment_Table.csv";
        string soFolderPath = "Assets/Resources/Inventory/EquipSO/"; // 장비 SO 저장 폴더
        string tableSOPath = "Assets/Resources/Inventory/EquipmentTableSO.asset"; // 테이블 SO 경로

        if (!Directory.Exists(soFolderPath))
            Directory.CreateDirectory(soFolderPath);

        // 기존 장비 SO 삭제 (클린하게 만들고 싶을 때)
        var oldFiles = Directory.GetFiles(soFolderPath, "*.asset");
        foreach (var file in oldFiles) AssetDatabase.DeleteAsset(file);

        var lines = File.ReadAllLines(csvPath);
        if (lines.Length <= 1)
        {
            Debug.LogError("CSV 파일이 비어있습니다.");
            return;
        }

        var tableSO = ScriptableObject.CreateInstance<EquipmentTableSO>();
        tableSO.equipmentList = new List<EquipmentDataSO>();

        for (int i = 2; i < lines.Length; i++) // 0번 라인은 헤더
        {
            var tokens = lines[i].Split(',');
            if (string.IsNullOrWhiteSpace(lines[i]) || tokens.Length < 19) continue;

            var dataSO = ScriptableObject.CreateInstance<EquipmentDataSO>();

            int.TryParse(tokens[0], out dataSO.id);
            Enum.TryParse<EquipGrade>(tokens[1],out dataSO.grade);
            dataSO.equipName = tokens[2];
            Enum.TryParse<EquipType>(tokens[3], out dataSO.type);
            Enum.TryParse<SetType>(tokens[4], out dataSO.setType);
            dataSO.icon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Resources/Sprites/Equipment/{dataSO.id}.png", typeof(Sprite));
            dataSO.level = 1;
            int.TryParse(tokens[7], out dataSO.maxLevel);
            int.TryParse(tokens[8], out dataSO.upgradeGold);
            int.TryParse(tokens[9], out dataSO.upgradeGoldPlus);
            int.TryParse(tokens[11], out dataSO.equipValue);
            int.TryParse(tokens[12], out dataSO.equipValuePlus);
            dataSO.Effect_Desc = tokens.Length > 19 ? tokens[19] : "";

            // 개별 SO 저장
            string assetName = $"{dataSO.id}.asset";
            string assetPath = Path.Combine(soFolderPath, assetName);
            AssetDatabase.CreateAsset(dataSO, assetPath);
            tableSO.equipmentList.Add(dataSO);
        }

        // 테이블 SO 저장
        AssetDatabase.CreateAsset(tableSO, tableSOPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("장비 CSV → SO 변환 완료!");
    }
}
#endif
}
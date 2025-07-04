using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

namespace KYG_skyPower
{

#if UNITY_EDITOR
    public class DialogCSVImporter // CSV 읽어서 DialogDataSO 변환
    {
        [MenuItem("Tools/Import Dialog CSV")]
        public static void ImportDialogCSV()
        {
            string path = EditorUtility.OpenFilePanel("Import Dialog CSV", "", "csv");
            if (string.IsNullOrEmpty(path)) return;

            var lines = File.ReadAllLines(path);
            var data = ScriptableObject.CreateInstance<DialogDataSO>();
            data.lines.Clear();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var tokens = SplitCsvLine(lines[i]);

                var line = new DialogLine();
                line.id = int.Parse(tokens[0]);
                line.speaker = tokens[1];
                line.portraitKey = tokens[2];
                line.text = tokens[3].Trim('"');
                line.isBranch = tokens[4] == "select";
                line.nextIds = new List<int>();
                if (!string.IsNullOrEmpty(tokens[5]))
                {
                    foreach (var nid in tokens[5].Split('|'))
                        if (int.TryParse(nid, out int val)) line.nextIds.Add(val);
                }

                line.voiceKey = tokens[6];
                data.lines.Add(line);

            }

            string assetPath = "Assets/Resources/DialogDataSO.asset";
            AssetDatabase.CreateAsset(data, assetPath);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Dialog CSV Import", "Import Complete!\n" + assetPath, "OK");

        }

        static string[] SplitCsvLine(string line)
        {
            return line.Split(',');
        }
    }
#endif
}
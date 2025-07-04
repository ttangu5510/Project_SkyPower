using IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IO 
{
    public class CsvReader
    {
        public static string BasePath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + Path.DirectorySeparatorChar;
#else
                return Application.persistentDataPath + Path.DirectorySeparatorChar;
#endif
            }
        }

        public static void Read(Csv csv)
        {
            if (!TryLoadLines(csv, out string[] lines))
                return;

            bool isReadSuccessful;
            switch (csv)
            {
                case CsvTable table:
                    isReadSuccessful = ReadToTable(table, lines);
                    break;
                default:
                    isReadSuccessful = false;
                    break;
            }
            PrintResult(csv, isReadSuccessful);
        }

        public static bool ReadToTable(CsvTable csv, string[] lines)
        {
            string[] firstLineFields = lines[0].Split(csv.SplitSymbol);
            int rows = lines.Length;
            int columns = firstLineFields.Length;

            csv.Table = new string[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                string[] fields = lines[r].Split(csv.SplitSymbol);
                if (fields.Length < columns)
                {
                    return false;
                }

                for (int c = 0; c < columns; c++)
                {
                    csv.Table[r, c] = fields[c];
                }
            }

            return true;
        }

        private static bool IsValidPath(Csv csv)
        {
            if (!File.Exists(csv.FilePath))
            {
#if UNITY_EDITOR
                Debug.LogError($"Error: CSV file not found at path: {csv.FilePath}");
#endif
                return false;
            }
            return true;
        }

        private static bool IsValidEmpty(Csv csv, out string[] lines)
        {
            lines = File.ReadAllLines(csv.FilePath);

            if (lines.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogError($"Error: CSV file at path {csv.FilePath} is empty.");
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// 환경에 따라 (Editor: File, Build: Resources) CSV 데이터를 읽어옴
        /// </summary>
        private static bool TryLoadLines(Csv csv, out string[] lines)
        {
            lines = null;

#if UNITY_EDITOR
            // 에디터에서는 실제 파일 경로에서 읽기
            if (!File.Exists(csv.FilePath))
            {
                Debug.LogError($"[CsvReader] File not found at: {csv.FilePath}");
                return false;
            }

            lines = File.ReadAllLines(csv.FilePath);

            if (lines.Length == 0)
            {
                Debug.LogError($"[CsvReader] CSV file is empty: {csv.FilePath}");
                return false;
            }
#else
            TextAsset textAsset;
            // 빌드 환경에서는 Resources.Load 사용
            textAsset = Resources.Load<TextAsset>(csv.ResourcePath);
            if (textAsset == null)
            {
                Debug.LogError($"[CsvReader] Resource not found: Resources/{csv.ResourcePath}");
                return false;
            }

            lines = textAsset.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);

            if (lines.Length == 0)
            {
                Debug.LogError($"[CsvReader] CSV resource is empty: {csv.ResourcePath}");
                return false;
            }
#endif

            return true;
        }


        private static void PrintResult(Csv csv, bool result)
        {
#if UNITY_EDITOR
            if (result)
            {
                Debug.Log($"Successfully loaded CSV file from path: {csv.FilePath}");
            }
            else
            {
                Debug.LogError($"Error: CSV file at path {csv.FilePath} has inconsistent column lengths.");
            }
#endif
        }
    }
}

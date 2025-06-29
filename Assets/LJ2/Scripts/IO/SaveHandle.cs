using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IO
{
    /// <summary>
    /// # Summary
    /// Abstract base class for handling data saving and loading operations in Unity.
    /// Provides a common interface and utility methods for derived save handler classes.
    /// 유니티에서 데이터 저장 및 로드 작업을 처리하기 위한 추상 기본 클래스.
    /// 파생된 저장 핸들러 클래스들을 위한 공통 인터페이스와 유틸리티 메서드를 제공한다.
    ///
    /// ## Usage:
    /// - Inherit from this class to create custom save handlers for different save types (e.g., JSON, Binary).
    /// - Implement the `Save` and `Load` methods to handle specific save logic for different formats.
    /// ## 사용 방법:
    /// - 이 클래스를 상속하여 다른 저장 유형(JSON, Binary 등)을 위한 사용자 지정 저장 핸들러를 생성한다.
    /// - `Save` 및 `Load` 메서드를 구현하여 다양한 형식에 대한 특정 저장 로직을 처리한다.
    ///
    /// ## Features:
    /// - Provides base path determination for saving files based on the environment (Editor or Build).
    /// - Includes utility methods to check file accessibility and validity.
    /// - Offers an interface for derived classes to implement their own save/load logic.
    /// ## 기능:
    /// - 환경(Editor 또는 Build)에 따라 파일을 저장할 기본 경로를 결정한다.
    /// - 파일 접근성과 유효성을 확인하기 위한 유틸리티 메서드를 포함한다.
    /// - 파생 클래스가 자체 저장/로드 로직을 구현할 수 있는 인터페이스를 제공한다.
    /// </summary>
    public class SaveHandle
    {
        /// <summary>
        /// Gets the base path for saving files depending on the build environment.
        /// Returns the 'SaveData' directory path within the project's assets directory in the editor,
        /// and the persistent data path in a build.
        /// 빌드 환경에 따라 파일 저장 경로를 가져온다.
        /// 에디터에서는 프로젝트의 'SaveData' 디렉터리 경로를 반환하며, 빌드된 환경에서는 지속 데이터 경로를 반환한다.
        /// </summary>
        public static string BasePath
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.dataPath, "SaveData");
#else
                return Path.Combine(Application.persistentDataPath, "SaveData");
#endif
            }
        }


        /// <summary>
        /// Checks if the given JSON string is empty or null.
        /// If empty, logs an error message and returns true.
        /// 주어진 JSON 문자열이 비어 있거나 null인지 확인한다.
        /// 비어 있으면 에러 메시지를 기록하고 true를 반환한다.
        /// </summary>
        /// <param name="jsonString">The JSON string to check. 확인할 JSON 문자열.</param>
        /// <param name="type">The save type being used. 사용 중인 저장 타입.</param>
        /// <returns>Returns true if the string is empty or null; otherwise false. 문자열이 비어있거나 null이면 true, 그렇지 않으면 false를 반환.</returns>
        private bool IsFileEmpty(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                PrintErrorMessage($"Failed to convert object : File Empty");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the file at the given path is accessible.
        /// If accessible, logs a success message; otherwise, logs an error message.
        /// 주어진 경로에 있는 파일에 접근할 수 있는지 확인한다.
        /// 접근할 수 있으면 성공 메시지를 기록하고, 그렇지 않으면 에러 메시지를 기록한다.
        /// </summary>
        /// <param name="filePath">The path of the file to check. 확인할 파일의 경로.</param>
        /// <returns>Returns true if the file is accessible; otherwise false. 파일에 접근할 수 있으면 true, 그렇지 않으면 false를 반환.</returns>
        private bool IsFileAccessible(string filePath)
        {
            if (File.Exists(filePath))
            {
                PrintSuccessMessage($"Successfully accessed the file at path: {filePath}.");
                return true;
            }
            else
            {
                PrintErrorMessage($"Failed to access the file at path: {filePath}.");
                return false;
            }
        }

        /// <summary>
        /// Logs a success message. Only logs in the Unity Editor.
        /// 성공 메시지를 기록한다. 유니티 에디터에서만 기록된다.
        /// 추후 로그 수집 기능을 추가할 때 이 부분을 수정하여 다른 로그 수집 시스템과 통합할 수 있다.
        /// </summary>
        /// <param name="message">The success message to log. 기록할 성공 메시지.</param>
        private void PrintSuccessMessage(string message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
            // Add Error Log - To be replaced with a centralized logging system in the future
        }

        /// <summary>
        /// Logs an error message. Only logs in the Unity Editor.
        /// 에러 메시지를 기록한다. 유니티 에디터에서만 기록된다.
        /// 추후 로그 수집 기능을 추가할 때 이 부분을 수정하여 다른 로그 수집 시스템과 통합할 수 있다.
        /// </summary>
        /// <param name="message">The error message to log. 기록할 에러 메시지.</param>
        private void PrintErrorMessage(string message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
            // Add Error Log - To be replaced with a centralized logging system in the future
        }

        /// <summary>
        /// Generates the full file path using the base path and the given file name.
        /// 주어진 파일 이름을 사용하여 기본 경로와 결합한 전체 파일 경로를 생성한다.
        /// </summary>
        /// <param name="fileName">The name of the file. 파일 이름.</param>
        /// <returns>Returns the full file path. 전체 파일 경로를 반환한다.</returns>
        private static string GetFilePath(string fileName, int index)
        {
            return Path.Combine(BasePath, $"Slot_{index}", $"{fileName}.json");
        }

        /// <summary>
        /// Abstract method for saving data. Must be implemented by derived classes.
        /// 데이터를 저장하는 추상 메서드. 파생 클래스에서 구현해야 한다.
        /// </summary>
        /// <typeparam name="T">The type of data to save. 저장할 데이터의 타입.</typeparam>
        /// <param name="target">The data object to save. 저장할 데이터 객체.</param>
        public void Save<T>(T target, int index) where T : SaveData
        {
            Directory.CreateDirectory(BasePath);
            Directory.CreateDirectory($"{BasePath}/Slot_{index}");

            string filePath = GetFilePath(target.FileName, index);
            string jsonString = JsonUtility.ToJson(target);

            if (IsFileEmpty(jsonString)) return;

            File.WriteAllText(filePath, jsonString);

            IsFileAccessible(filePath);
        }

        /// <summary>
        /// Abstract method for loading data. Must be implemented by derived classes.
        /// 데이터를 불러오는 추상 메서드. 파생 클래스에서 구현해야 한다.
        /// </summary>
        /// <typeparam name="T">The type of data to load. 불러올 데이터의 타입.</typeparam>
        /// <param name="target">The data object to load into. 데이터를 불러올 객체.</param>
        public void Load<T>(ref T target, int index) where T : SaveData, new()
        {
            string filePath = GetFilePath(target.GetType().ToString(), index);
            string jsonString = File.ReadAllText(filePath);

            if (!IsFileAccessible(filePath)) return;

            if (IsFileEmpty(jsonString)) return;

            target = JsonUtility.FromJson<T>(jsonString);
        }

        public void Delete<T>(T target, int index) where T : SaveData
        {
            string filePath = GetFilePath(target.GetType().ToString(), index);
            string jsonString = File.ReadAllText(filePath);

            if (!IsFileAccessible(filePath)) return;

            if (IsFileEmpty(jsonString)) return;

            File.Delete(filePath);
        }
        
    } 
}

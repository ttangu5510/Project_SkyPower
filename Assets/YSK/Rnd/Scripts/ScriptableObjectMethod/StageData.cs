using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YSK;

namespace YSK
{
    [CreateAssetMenu(menuName = "Stage/StageData")]
    public class StageData : ScriptableObject
    {
        [Header("Basic Info")]
        public int stageID;
        public string stageName;
        public string sceneName;
        public float duration;
        
        [Header("Map Settings")]
        public List<GameObject> mapPrefabs;
        
        [Header("Sub Stage Settings")]
        [Tooltip("서브 스테이지별 설정 (현재는 모든 서브 스테이지가 같은 맵 사용)")]
        public List<SubStageData> subStages = new List<SubStageData>();
        
        [Header("Difficulty Settings")]
        [Tooltip("기본 난이도 (1-5)")]
        [Range(1, 5)]
        public int baseDifficulty = 1;
        
        [Tooltip("서브 스테이지별 난이도 증가량")]
        [Range(0, 2)]
        public float difficultyIncreasePerSubStage = 0.2f;
    }
    
    [System.Serializable]
    public class SubStageData
    {
        [Header("Sub Stage Info")]
        public int subStageID;
        public string subStageName;
        
        [Header("Map Override")]
        [Tooltip("이 서브 스테이지 전용 맵 프리팹 (null이면 기본 맵 사용)")]
        public List<GameObject> customMapPrefabs;
        
        [Header("Difficulty Override")]
        [Tooltip("이 서브 스테이지 전용 난이도 (0이면 기본 난이도 사용)")]
        [Range(0, 5)]
        public int customDifficulty = 0;
        
        [Header("Special Settings")]
        [Tooltip("이 서브 스테이지에서만 활성화되는 특별한 설정")]
        public bool hasSpecialMechanics = false;
        public string specialMechanicsDescription = "";
    }
}

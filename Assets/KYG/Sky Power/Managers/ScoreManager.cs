using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KYG_skyPower
{
    // 점수 전담 매니저
    public class ScoreManager : Singleton<ScoreManager>
    {
        
        public UnityEvent<int> onScoreChanged; // UI 담당자의 UI 연결을 위해 이벤트로 확장성
        public int Score { get; private set; }

        public override void Init()
        {
            // 필요성 확인
        }

        /*private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }*/

        public void ResetScore() { Score = 0; onScoreChanged?.Invoke(Score); }
        public void AddScore(int value)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
            Score += value;
            onScoreChanged?.Invoke(Score);
        }

        public void RecordBestScore()
        {
            int bestScore = Manager.SDM.runtimeData[Manager.Game.selectWorldIndex].subStages[Manager.Game.selectStageIndex].bestScore;
            if (Manager.Score.Score > bestScore)
            {
                // TODO 신기록 달성
                Manager.SDM.runtimeData[Manager.Game.selectWorldIndex].subStages[Manager.Game.selectStageIndex].bestScore = bestScore;
            }
        }
    }
}

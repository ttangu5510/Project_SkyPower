using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using LJ2;
using IO;
using JYL;

namespace KYG_skyPower
{

    /*
    
    싱글톤 기반 게임 매니저

    필요 기능
    게임 스코어
    게임 오버 여부
    게임 일시정지,게임 재개 기능

    추가 기능
    이벤트 기반 코드로 확장성 고려
    세이브 테이터를 배열로 게임 매니저가 가지고 와야 한다
    게임 시작시 세이브 데이터 가지고 와서 가지고 있는다

    */

    
    public class GameManager : Singleton<GameManager>
    {
        public UnityEvent onGameOver, onPause, onResume, onGameClear;

        public GameData[] saveFiles = new GameData[3]; // 세이브 파일 3개

        public int currentSaveIndex { get; private set; } = 0;

        public GameData CurrentSave => saveFiles[currentSaveIndex]; // 세이브 파일 인덱스로 배열

        public bool isGameOver { get; private set; } // 게임 오버
        public bool isPaused { get; private set; } // 게임 일시 정지

        public bool isGameCleared { get; private set; } // 게임 클리어

        public int selectWorldIndex=0;
        public int selectStageIndex=0;

        //[SerializeField] private int defaultPlayerHP = 5;
        //public int playerHp { get; private set; } // 플레이어에 붙을 수도 있지만 나중에 추가 될지 몰라 주석 처리

        public override void Init() // 게임 시작시 세이브 데이터 로드
        {
            ResetSaveRef();
        }
        public void ResetSaveRef()
        {
            for (int i = 0; i < saveFiles.Length; i++)
            {
                saveFiles[i] = new GameData();
                SaveManager.Instance.GameLoad(ref saveFiles[i], i + 1); // 인덱스 1부터
            }
        }

        public void SelectSaveFile(int index)
        {
            if (index >= 0 && index < saveFiles.Length)
                currentSaveIndex = index;
        }

        /*private void Awake() // 싱글톤 패턴
        {
            if (Instance != null && Instance != this) // 만약 다른 Instance 있으면 본 Instance
            {
                Destroy(gameObject); // 삭제
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // 게임 오브젝트 파괴되지 않게 제한

        }*/



        public void SetGameOver()
        {
            if (isGameOver) return;
            isGameOver = true; // 게임 오버가 true면
            Time.timeScale = 0f; // 시간 정지 기능
            onGameOver?.Invoke();
            Debug.Log("게임 오버");
        }

        public void SetGameClear()
        {
            if (isGameCleared || isGameOver) return;
            isGameCleared = true;
            Time.timeScale = 1f;
            onGameClear?.Invoke();
            UIManager.Instance.ShowPopUp<StageClearPopUp>();
            
            Debug.Log("게임 클리어");
        }

        public void PausedGame()
        {
            if (isPaused || isGameOver) return;
            isPaused = true;
            Time.timeScale = 0f; // 전체 게임 정지
            onPause?.Invoke();
            Debug.Log("일시 정지");
        }

        public void ResumeGame()
        {
            if (!isPaused || isGameOver) return;
            isPaused = false;
            Time.timeScale = 1f; // 게임 시간 정상화
            onResume?.Invoke();
            Debug.Log("게임 재개");
        }

        public void ResetStageIndex()
        {
            selectWorldIndex = 0;
            selectStageIndex = 0;
        }
        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(isPaused) ResumeGame();
                else PausedGame();
            }
        }*/ // ESC 키입력으로 일시정지 기능 예시로 작성
    }
}


using YSK;

namespace YSK
{
    /// <summary>
    /// 게임의 기본 상태를 정의합니다.
    /// </summary>
    public enum GameState
    {
        MainMenu,        // 메인 메뉴
        StageSelect,     // 스테이지 선택
        Playing,         // 게임 플레이 중
        Paused,          // 일시정지
        StageComplete,   // 스테이지 완료
        GameOver,        // 게임 오버
        Loading          // 로딩 중
    }

    /// <summary>
    /// 스테이지 진행 상태를 정의합니다.
    /// </summary>
    public enum StageProgressState
    {
        NotStarted,      // 시작 전
        InProgress,      // 진행 중
        Completed,       // 완료
        Failed,          // 실패
        Skipped          // 건너뜀
    }

    /// <summary>
    /// 플레이어 상태를 정의합니다.
    /// </summary>
    public enum PlayerState
    {
        Alive,           // 생존
        Dead,            // 사망
        Invincible,      // 무적
        PowerUp          // 파워업
    }
} 
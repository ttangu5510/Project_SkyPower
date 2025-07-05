# 총알 효과 테스트 시스템

## 📋 개요
`BulletEffectController`와 연동하여 총알 효과를 테스트할 수 있는 간단한 플레이어 시스템입니다.

## 🎮 구성 요소

### 1. PlayerController
- **기능**: 플레이어 이동, 회전, 총알 발사
- **조작**: WASD 이동, 마우스 회전, 마우스 좌클릭/스페이스바 발사
- **특징**: 자동으로 총알 프리팹과 FirePoint 생성

### 2. TargetController
- **기능**: 타겟 체력 관리, 데미지 처리, 파괴/리스폰
- **특징**: 체력에 따른 시각적 피드백, 히트 애니메이션

### 3. BulletTestGameManager
- **기능**: 게임 전체 관리, 타겟 자동 스폰, 통계 관리
- **특징**: 간단한 UI, 디버그 정보 표시

## 🚀 빠른 시작

### 1. 기본 설정
```csharp
// 1. 빈 GameObject 생성
GameObject gameManager = new GameObject("BulletTestGameManager");

// 2. BulletTestGameManager 컴포넌트 추가
BulletTestGameManager manager = gameManager.AddComponent<BulletTestGameManager>();

// 3. 게임 시작
// 자동으로 플레이어와 타겟이 생성됩니다!
```

### 2. 수동 설정
```csharp
// 플레이어 프리팹 설정
manager.playerPrefab = yourPlayerPrefab;

// 타겟 프리팹 설정
manager.targetPrefab = yourTargetPrefab;

// 게임 설정
manager.SetMaxTargets(10);
manager.SetTargetHealth(150f);
manager.SetAutoSpawn(true);
```

## 🎯 조작법

### 플레이어 조작
- **WASD**: 이동
- **마우스**: 좌우 회전
- **마우스 좌클릭** 또는 **스페이스바**: 총알 발사
- **스페이스바**: 점프 (지면에 있을 때)

### 게임 조작
- **R**: 게임 리셋
- **T**: 타겟 수동 스폰
- **C**: 모든 타겟 제거

## ⚙️ 설정 옵션

### PlayerController 설정
```csharp
[Header("Movement Settings")]
moveSpeed = 5f;           // 이동 속도
rotationSpeed = 100f;     // 회전 속도
jumpForce = 5f;          // 점프 힘

[Header("Shooting Settings")]
bulletSpeed = 20f;       // 총알 속도
fireRate = 0.2f;         // 발사 속도
maxBullets = 10;         // 최대 총알 수
```

### TargetController 설정
```csharp
[Header("Target Settings")]
health = 100f;           // 초기 체력
maxHealth = 100f;        // 최대 체력
isDestructible = true;   // 파괴 가능 여부
respawnTime = 3f;        // 리스폰 시간
```

### BulletTestGameManager 설정
```csharp
[Header("Test Settings")]
autoSpawnTargets = true;     // 자동 타겟 스폰
maxTargets = 5;              // 최대 타겟 수
targetSpawnInterval = 3f;    // 타겟 스폰 간격
spawnAreaSize = (20, 0, 20); // 스폰 영역 크기
```

## 🎨 시각 효과

### 총알 효과
- **Flash 효과**: 총알 발사 시 플래시 효과
- **Hit 효과**: 타겟 충돌 시 히트 효과
- **Destroy 효과**: 타겟 파괴 시 파괴 효과

### 타겟 피드백
- **체력바**: 타겟 위에 체력바 표시
- **색상 변화**: 체력에 따른 머티리얼 색상 변경
- **히트 애니메이션**: 피격 시 스케일 애니메이션

## 🔧 커스터마이징

### 커스텀 총알 프리팹
```csharp
// PlayerController에서 설정
playerController.bulletPrefab = yourCustomBulletPrefab;

// 또는 런타임에 생성
GameObject customBullet = CreateCustomBullet();
playerController.bulletPrefab = customBullet;
```

### 커스텀 타겟 프리팹
```csharp
// BulletTestGameManager에서 설정
gameManager.targetPrefab = yourCustomTargetPrefab;
```

### 이벤트 연결
```csharp
// 타겟 파괴 이벤트
TargetController.OnTargetDestroyed += () => {
    Debug.Log("타겟이 파괴되었습니다!");
    gameManager.OnTargetDestroyed();
};

// 총알 발사 이벤트
PlayerController.OnShotFired += () => {
    gameManager.OnShotFired();
};
```

## 🐛 문제 해결

### 총알이 발사되지 않는 경우
1. **FirePoint 확인**: PlayerController의 FirePoint가 올바르게 설정되었는지 확인
2. **총알 프리팹 확인**: bulletPrefab이 null이 아닌지 확인
3. **입력 확인**: 마우스 좌클릭이나 스페이스바가 제대로 인식되는지 확인

### 타겟이 데미지를 받지 않는 경우
1. **Collider 확인**: 타겟에 Collider가 있는지 확인
2. **Tag 확인**: 총알에 "Bullet" 태그가 있는지 확인
3. **BulletEffectController 확인**: 총알에 BulletEffectController가 있는지 확인

### 성능 최적화
1. **총알 개수 제한**: maxBullets 설정으로 총알 개수 제한
2. **타겟 개수 제한**: maxTargets 설정으로 타겟 개수 제한
3. **자동 정리**: 파괴된 총알과 타겟 자동 정리

## 📊 통계 정보

게임 매니저에서 다음 통계를 확인할 수 있습니다:
- **활성 타겟 수**: 현재 활성화된 타겟 개수
- **파괴된 타겟 수**: 총 파괴된 타겟 개수
- **총 발사 수**: 총 발사한 총알 개수
- **명중률**: 파괴된 타겟 / 총 발사 수 * 100

## 🎮 확장 가능성

### 추가 기능 구현
1. **다양한 무기**: 다른 종류의 총알과 무기 추가
2. **파워업**: 속도 증가, 데미지 증가 등의 파워업
3. **점수 시스템**: 타겟 파괴 시 점수 획득
4. **레벨 시스템**: 난이도가 점진적으로 증가하는 레벨 시스템

### 네트워크 연동
1. **멀티플레이어**: 여러 플레이어가 동시에 플레이
2. **점수 랭킹**: 온라인 점수 랭킹 시스템
3. **실시간 대전**: 실시간 PvP 시스템

## 📝 예제 코드

### 기본 사용 예제
```csharp
using UnityEngine;
using YSK;

public class BulletTestExample : MonoBehaviour
{
    private BulletTestGameManager gameManager;
    
    void Start()
    {
        // 게임 매니저 생성
        GameObject managerObj = new GameObject("GameManager");
        gameManager = managerObj.AddComponent<BulletTestGameManager>();
        
        // 설정
        gameManager.SetMaxTargets(8);
        gameManager.SetTargetHealth(120f);
        gameManager.SetAutoSpawn(true);
    }
    
    void Update()
    {
        // 추가 기능 구현
        if (Input.GetKeyDown(KeyCode.P))
        {
            // 일시정지
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }
    }
}
```

이 시스템을 사용하여 `BulletEffectController`의 다양한 효과를 쉽게 테스트할 수 있습니다! 
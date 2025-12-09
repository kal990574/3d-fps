# 3D FPS Game - Claude Code 프로젝트 가이드

## 프로젝트 개요
- **엔진**: Unity 6000.0.60f1 (LTS)
- **장르**: 3D First-Person Shooter
- **언어**: C#
- **렌더링**: URP (Universal Render Pipeline)

---

## 0. Claude Code Assistant 지침

* **리뷰 언어:** 모든 코드 리뷰 요약 및 코멘트를 **한국어(Korean)**로 작성합니다. 명확하고 자연스러운 한글 설명을 사용합니다.
* **디자인 원칙 검토:** 모든 PR에 대해 **SOLID 원칙**과 **디미터의 법칙(Law of Demeter)** 위반 여부를 중점적으로 확인합니다.
* **함수:** 함수는 한 가지 일만 합니다.

### 0.1. 핵심 디자인 원칙 (SOLID & LoD)

#### SOLID 원칙

| 약어 | 원칙 | 핵심 요약 |
| :---: | :--- | :--- |
| **S** | **단일 책임 원칙 (SRP)** | 클래스는 **단 하나의 변경 이유**만 가져야 합니다. |
| **O** | **개방-폐쇄 원칙 (OCP)** | **확장에는 열려 있고, 수정에는 닫혀 있어야** 합니다. |
| **L** | **리스코프 치환 원칙 (LSP)** | 상위 타입을 하위 타입으로 **치환해도 문제없이 작동**해야 합니다. |
| **I** | **인터페이스 분리 원칙 (ISP)** | **단일 목적의 작은 인터페이스**를 선호합니다. |
| **D** | **의존성 역전 원칙 (DIP)** | **고수준/저수준 모듈 모두 추상화에 의존**해야 합니다. |

#### 디미터의 법칙 (LoD)

* **핵심:** "오직 가장 가까운 친구와만 이야기하라"
* **위반 방지:** `a.getB().getC().doSomething()` 같은 **"기차 참사(Train Wreck)"** 패턴 금지

### 0.2. 보이스카우트 원칙

**"코드를 발견했을 때보다 더 깨끗하게 만들어 놓고 떠나라"**

- 작은 개선이라도 누적하면 큰 품질 향상
- 중복 제거, 네이밍 개선, 불필요한 복잡성 제거
- 미사용 변수, 죽은 코드(dead code) 정리
- 테스트 가능성, 확장 가능성 향상

---

## 1. 프로젝트 구조

```
Assets/
├── 01.Scenes/          # 씬 파일
├── 02.Scripts/         # C# 스크립트
│   ├── Camera/         # 카메라 시스템
│   ├── Player/         # 플레이어 시스템
│   └── UI/             # UI 시스템
├── 03.Prefabs/         # 프리팹
├── 04.Images/          # 이미지 에셋
├── 05.Models/          # 3D 모델
├── 06.Sounds/          # 사운드 에셋
├── 07.Animations/      # 애니메이션
├── 08.Fonts/           # 폰트
├── 09.Materials/       # 머티리얼
└── Plugins/            # 외부 플러그인 (DOTween 등)
```

---

## 2. 구현된 시스템

### 플레이어 시스템 (`02.Scripts/Player/`)
| 스크립트 | 설명 |
|----------|------|
| `PlayerMove.cs` | WASD 이동, 달리기(Shift), 점프/더블점프(Space) |
| `PlayerRotate.cs` | 카메라 방향에 따른 플레이어 회전 |
| `PlayerStats.cs` | 체력/스태미나 관리 |

**주요 수치:**
- 이동 속도: 7 units/sec
- 달리기 속도: 12 units/sec
- 점프력: 10 units
- 스태미나 소모: 달리기 20/sec, 더블점프 25
- 스태미나 회복: 10/sec

### 카메라 시스템 (`02.Scripts/Camera/`)
| 스크립트 | 설명 |
|----------|------|
| `CameraRotate.cs` | 마우스 시점 조작 (감도 2, 수직 -90°~90°) |
| `CameraFollow.cs` | 플레이어 추적 |
| `CameraModeSwitch.cs` | FPS/TPS 전환 (T키), DOTween 트랜지션 |

### UI 시스템 (`02.Scripts/UI/`)
| 스크립트 | 설명 |
|----------|------|
| `StaminaUI.cs` | 체력/스태미나 슬라이더 UI |

---

## 3. 명명 규칙 (Naming Conventions)

### PascalCase 사용 대상
- 클래스, 구조체, 레코드, 대리자
- 인터페이스: **`I`** 접두사 (`IMovable`, `IDamageable`)
- 공용 멤버: 속성, 메서드, 이벤트, 공용 필드
- 상수

### camelCase 사용 대상
- 메서드 매개변수, 지역 변수

### 필드 명명 및 접두사

| 대상 | 접두사 | 예시 |
| :--- | :--- | :--- |
| Private 인스턴스 필드 | `_` | `_workerQueue` |
| 정적 필드 | `s_` | `s_defaultLogger` |
| 스레드 정적 필드 | `t_` | `t_timeSpan` |

### 일반 원칙
- **간결성보다 명확성** 우선
- 연속 밑줄(`__`) 사용 금지
- 단일 문자 이름은 루프 카운터 외 금지

---

## 4. C# 언어 사용 규칙

- **데이터 형식:** 런타임 형식(`System.Int32`) 대신 **언어 키워드**(`int`, `string`) 사용
- **`var` 사용:** 형식을 **명확히 유추할 수 있는 경우에만** 사용
- **문자열 처리:**
  - 짧은 연결: **문자열 보간**(`$"{}"`)
  - 루프 내 대용량: **`StringBuilder`**
- **대리자:** `Func<>` 또는 `Action<>` 사용
- **예외 처리:** 처리할 수 있는 **특정 예외만 catch**, 일반 `Exception` 포괄 금지

---

## 5. 레이아웃 및 주석 규칙

- **들여쓰기:** **4개의 공백** (탭 금지)
- **중괄호:** **Allman 스타일** (여는/닫는 중괄호 별도 줄)
- **코드 밀도:** 한 줄에 하나의 문장/선언
- **주석:** `//` 사용, **별도 줄**에 배치, **대문자로 시작**, **마침표**로 종료
- **XML 주석:** 작성하지 않음 (메서드명/매개변수명이 충분히 설명적이어야 함)

---

## 6. Unity 특화 규칙

### 컴포넌트 캐싱
```csharp
// Good - Start()에서 캐싱
private CharacterController _characterController;

private void Start()
{
    _characterController = GetComponent<CharacterController>();
}

// Bad - Update()에서 매 프레임 호출
private void Update()
{
    GetComponent<CharacterController>().Move(...); // 금지
}
```

### Camera.main 캐싱
```csharp
// Good
private Camera _mainCamera;

private void Start()
{
    _mainCamera = Camera.main;
}

// Bad
private void Update()
{
    Camera.main.transform.position; // 금지
}
```

### 스크립트 구조
```csharp
public class ExampleScript : MonoBehaviour
{
    [Header("Settings")]
    public float exampleValue = 10f;

    private ComponentType _cachedComponent;

    private void Start()
    {
        _cachedComponent = GetComponent<ComponentType>();
    }

    private void Update()
    {
        // 프레임 로직
    }
}
```

### 주의사항
- `CharacterController` 기반 이동 (Rigidbody X)
- Legacy Input System 사용 (`Input.GetAxis`)
- `.meta` 파일 수정 금지
- MonoBehaviour 클래스는 파일명과 클래스명 일치 필수
- `SerializeField`, `Header` 어트리뷰트 활용
- 에디터 전용 코드는 `#if UNITY_EDITOR` 사용

---

## 7. 외부 플러그인

### DOTween
- 애니메이션/트위닝 라이브러리
- 카메라 모드 전환 시 부드러운 트랜지션
- `DOMove()`, `SetEase(Ease.OutCubic)` 등

---

## 8. Git 브랜치 전략

- `main`: 안정 버전
- `feature/*`: 기능 개발
- `test/*`: 테스트/실험

---

## 9. 개발 예정 기능

- [ ] 무기/전투 시스템
- [ ] 적 AI
- [ ] 데미지 시스템
- [ ] 사운드 시스템
- [ ] 레벨 디자인
- [ ] 이펙트/파티클

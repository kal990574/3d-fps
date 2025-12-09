# Unity 베스트 프랙티스 체크

다음 파일/코드를 Unity 베스트 프랙티스 기준으로 검토해주세요:
$ARGUMENTS

## 검토 항목

### 성능 최적화
- [ ] 컴포넌트 캐싱 (`GetComponent` → `Start()`에서 캐싱)
- [ ] `Camera.main` 캐싱
- [ ] `Find` 계열 함수 사용 최소화
- [ ] 오브젝트 풀링 적용 여부
- [ ] `Update()` 최적화

### MonoBehaviour 생명주기
- [ ] 올바른 생명주기 메서드 사용
  - `Awake()`: 자기 자신 초기화
  - `Start()`: 다른 컴포넌트 참조
  - `OnEnable()`/`OnDisable()`: 이벤트 구독/해제
- [ ] 실행 순서(Script Execution Order) 고려

### 직렬화 및 Inspector
- [ ] `[SerializeField]` 적절한 사용
- [ ] `[Header]`, `[Tooltip]` 활용
- [ ] `[Range]` 로 값 제한
- [ ] public 필드 최소화

### 물리 및 충돌
- [ ] 물리 연산은 `FixedUpdate()`에서 처리
- [ ] Layer 및 Tag 적절한 사용
- [ ] Raycast 최적화

### 코루틴
- [ ] `StopCoroutine` 적절한 사용
- [ ] `WaitForSeconds` 캐싱
- [ ] 무한 루프 방지

### 메모리 관리
- [ ] 이벤트 구독 해제 (`OnDestroy`, `OnDisable`)
- [ ] 불필요한 참조 정리
- [ ] 씬 전환 시 리소스 정리

## 출력 형식
- **통과** / **개선 필요** / **심각** 등급으로 분류
- 개선이 필요한 항목은 구체적인 코드 예시 제공
- 한국어로 작성

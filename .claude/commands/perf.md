# 성능 분석

다음 파일/코드의 성능을 분석해주세요:
$ARGUMENTS

## 분석 항목

### Unity 성능 이슈
- [ ] `Update()`에서 `GetComponent()` 호출
- [ ] `Update()`에서 `Camera.main` 사용
- [ ] `Update()`에서 `Find()`, `FindObjectOfType()` 호출
- [ ] 매 프레임 문자열 연결 (GC 유발)
- [ ] 매 프레임 new 키워드로 객체 생성
- [ ] 불필요한 `Debug.Log()` 호출

### 물리/충돌 성능
- [ ] 과도한 Raycast 호출
- [ ] FixedUpdate vs Update 적절한 사용

### 메모리 관리
- [ ] 오브젝트 풀링 필요 여부
- [ ] 불필요한 참조 유지

## 출력 형식
- 발견된 성능 이슈를 **심각도(높음/중간/낮음)**와 함께 나열
- 각 이슈에 대한 **개선 코드 예시** 제공
- 한국어로 작성

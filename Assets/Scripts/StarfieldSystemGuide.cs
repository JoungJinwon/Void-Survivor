/*
=== Starfield System 사용 가이드 ===

1. 셰이더 설정:
   - StarfieldShader.shader가 포함된 Material을 생성하세요
   - 이 Material을 Plane에 적용하세요

2. Starfield Plane 프리팹 생성:
   - 빈 GameObject 생성
   - Plane 메시 추가 (또는 MeshFilter + MeshRenderer 컴포넌트)
   - StarfieldPlane.cs 스크립트 추가
   - 위에서 만든 Starfield Material 적용

3. StarfieldPlaneManager 설정:
   - 빈 GameObject 생성 (예: "StarfieldManager")
   - StarfieldPlaneManager.cs 스크립트 추가
   - Plane Prefab에 위에서 만든 프리팹 할당
   - Global Star Density로 모든 Plane의 통일된 밀도 설정
   - Plane Size, Grid Radius 설정

=== 중요한 변경사항 ===

★ Plane 경계 격자 문제 해결:
- 이제 모든 Plane이 동일한 Star Density를 사용합니다
- StarfieldPlaneManager의 "Global Star Density" 값이 모든 Plane에 적용됩니다
- SetRandomVariation()에서는 density가 아닌 다른 속성만 변경됩니다

=== 기능 설명 ===

★ 개선된 셰이더 기능:
- 다층 깊이감: 여러 레이어로 원근감 구현
- 큰 별: 랜덤 확률로 큰 별 생성
- 다양한 색상: 3가지 별 색상 랜덤 적용
- 반짝임 효과: 시간에 따른 밝기 변화

★ 동적 Plane 관리:
- 플레이어 위치 기반 자동 Plane 생성/제거
- 메모리 효율적인 그리드 시스템
- 통일된 별 밀도로 연속적인 우주 배경

=== 매개변수 설명 ===

StarfieldShader:
- Star Density: 별의 밀도 (모든 Plane에서 통일됨)
- Star Size: 기본 별 크기
- Depth Layers: 깊이 레이어 수 (원근감)
- Big Star Chance: 큰 별이 나타날 확률
- Big Star Size Multiplier: 큰 별의 크기 배율
- Star Colors: 별의 색상 3가지
- Twinkle Speed: 반짝임 속도

StarfieldPlaneManager:
- Global Star Density: 모든 Plane에 적용되는 통일된 별 밀도
- Plane Size: 각 Plane의 크기 (Unity 단위)
- Grid Radius: 플레이어 주변 유지할 Plane 수

=== 최적화 팁 ===

1. Global Star Density로 모든 Plane의 일관성을 유지하세요
2. Plane Size를 너무 작게 하지 마세요 (빈번한 생성/제거)
3. Grid Radius는 2-3이 적당합니다
4. Depth Layers는 3-4가 성능과 품질의 균형점입니다
5. 모바일에서는 Global Star Density를 낮춰주세요

=== 런타임 설정 변경 ===

UpdateAllPlanesSettings() 메서드를 호출하면 기존 모든 Plane에
새로운 Global Star Density 설정을 적용할 수 있습니다.

*/

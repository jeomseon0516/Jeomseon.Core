# 변경 기록

## [0.1.3] - 2026-09-03

- Unity 최소 버전을 `6000.5.7f1` → `6000.6.0f1`로 상향했습니다. Managed plug-in을 재빌드했습니다. 공개 API 변경은 없습니다.

## [0.1.2] - 2026-08-11

- 모든 공개 API에 XML 문서를 추가하고 문서 누락을 CI 오류로 처리합니다.
- UPM managed plug-in에 IntelliSense용 XML 문서를 함께 배포합니다.
- 워크스페이스 명명 규칙에 맞춰 `RuntimeTypeDiscovery`·`MemberReflection`의 `private static
  readonly` 필드를 `_camelCase`로 정리했습니다. 공개 API 변경은 없습니다.

## [0.1.0] - 2026-07-31

- Unity 비의존 Collections와 Reflection API를 별도 저장소 및 패키지로 분리했습니다.
- NuGet과 UPM managed plug-in을 동일한 소스에서 생성하는 빌드 구조를 추가했습니다.


## [0.1.1] - 2026-08-05

- Unity 6000.5.7f1을 최소 지원 버전으로 상향했습니다.

# Jeomseon Core 로드맵

## 배포 전

- [x] 공개 API XML 문서를 완성하고 `CS1591` 누락을 CI 오류로 처리합니다.
- [x] NuGet 패키지 ID `Jeomseon.Core`를 확인하고 `0.1.0`을 배포했습니다.
- [x] NuGet.org Trusted Publishing을 GitHub Actions에 구성했습니다.
- [x] OpenUPM에 `com.jeomseon.core`를 등록하고 UPM 의존성 해석을 검증했습니다.
- [x] Unity 6000.5.7f1 IL2CPP Player에서 Reflection 필드 접근과 stripping을 검증했습니다.

## 지속 검증

- [x] CI 빌드에서 대상 프레임워크, Unity 전처리 심볼 및 빌드된 DLL의 Unity AssemblyRef를 검사합니다.

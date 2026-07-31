# Jeomseon Core 로드맵

## 배포 전

- 공개 API XML 문서를 완성하고 `CS1591` 억제를 제거합니다.
- 배포 직전에 NuGet 패키지 ID `Jeomseon.Core`가 계속 사용 가능한지 다시 확인합니다.
- NuGet.org Trusted Publishing 또는 제한된 `NUGET_API_KEY`를 GitHub Actions에 구성합니다.
- OpenUPM에 `com.jeomseon.core` 저장소를 등록하고 UPM 의존성 해석을 검증합니다.
- Unity IL2CPP Player 빌드에서 Reflection API의 stripping 대응 범위를 검증합니다.

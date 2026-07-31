# Jeomseon Core

Unity에 의존하지 않는 `netstandard2.1` 라이브러리입니다.

- `Jeomseon.Collections`: Deque, PriorityQueue 및 컬렉션 확장
- `Jeomseon.Reflection`: 멤버 캐시, 런타임 타입 탐색 및 타입 생성

## 빌드

```bash
./build.sh
```

이 명령은 단위 테스트를 실행하고 다음 산출물을 생성합니다.

- `Runtime/Plugins/Jeomseon.Core.dll`: UPM managed plug-in
- `artifacts~/nuget/Jeomseon.Core.*.nupkg`: NuGet 패키지
- `artifacts~/nuget/Jeomseon.Core.*.snupkg`: NuGet 심볼 패키지

## Unity

OpenUPM에서 `com.jeomseon.core`를 설치하거나 Git URL로 직접 설치할 수 있습니다.
다른 UPM 패키지는 scoped registry 배포 환경에서 `package.json`의 dependency로 선언할 수 있습니다.

## .NET

NuGet 배포 후 다음 명령으로 설치합니다.

```bash
dotnet add package Jeomseon.Core
```

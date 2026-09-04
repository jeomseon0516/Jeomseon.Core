# Jeomseon Core

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

## Unity (OpenUPM)

프로젝트의 `Packages/manifest.json`에 OpenUPM scoped registry를 한 번 등록합니다.

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.jeomseon"
      ]
    }
  ],
  "dependencies": {
    "com.jeomseon.core": "0.1.3"
  }
}
```

## Unity (Git URL)

OpenUPM 배포는 `upm/` 태그를 씁니다. Unity Package Manager의 `Install package from git URL`에
다음 주소를 사용합니다.

```text
https://github.com/jeomseon0516/Jeomseon.Core.git#upm/0.1.3
```

## .NET

NuGet 배포 후 다음 명령으로 설치합니다.

```bash
dotnet add package Jeomseon.Core
```

# Jeomseon Core

한국어 | [English](README.en.md)

Unity 비의존 Deque·PriorityQueue·collection 확장과 reflection API를 제공합니다.

## 빠르게 시작하기

Source~ 소스를 .NET 프로젝트에서 빌드/테스트하거나 UPM managed plug-in을 설치합니다. 컬렉션은 해당 타입의 push/pop/enqueue/dequeue 계약으로 사용하고 reflection은 멤버 탐색·생성 결과를 확인합니다.

## 계약과 주의사항

PriorityQueue의 기본 순서는 `MinimumFirst`(작은 priority 우선)이며, `PriorityQueueOrder.MaximumFirst`로 큰 priority 우선을 선택할 수 있습니다. UnityEngine 타입을 의존하지 않습니다. UPM은 Runtime/Plugins DLL을 소비하므로 Source~ 수정만으로 Unity DLL이 자동 갱신되지는 않습니다.

## 요구 사항

- Unity `6000.6.0f1`
- 패키지: `com.jeomseon.core` `0.1.3`

## 설치

프로젝트 `Packages/manifest.json`의 기존 dependencies와 registry를 유지하면서 다음 항목을 병합합니다.

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

또는 Package Manager의 **Install package from git URL**을 사용합니다:

```text
https://github.com/jeomseon0516/Jeomseon.Core.git#upm/0.1.3
```

## API 사용과 상세 설명

### 빌드

```bash
./build.sh
```

이 명령은 단위 테스트를 실행하고 다음 산출물을 생성합니다.

- `Runtime/Plugins/Jeomseon.Core.dll`: UPM managed plug-in
- `artifacts~/nuget/Jeomseon.Core.*.nupkg`: NuGet 패키지
- `artifacts~/nuget/Jeomseon.Core.*.snupkg`: NuGet 심볼 패키지

### .NET

NuGet 배포 후 다음 명령으로 설치합니다.

```bash
dotnet add package Jeomseon.Core
```

## 샘플 실행

등록된 Package Manager 샘플이 없습니다. 위의 빌드·마이그레이션 절차를 따릅니다.

## 테스트

[테스트 코드](Tests~)

`./build.sh`로 .NET 테스트와 plug-in/NuGet 산출물 생성을 실행합니다.

## 추가 문서

[전체 코드 지도와 패키지 구성](Documentation~/Overview.md)
- [Changelog](CHANGELOG.md)

## 라이선스

[License](LICENSE.md)

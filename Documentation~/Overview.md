# Jeomseon Core

[English](Overview.en.md) · [설치](../README.md)

## 목적

Unity 비의존 Deque·PriorityQueue·collection 확장과 reflection API를 제공합니다.

## 사용 흐름

Source~ 소스를 .NET 프로젝트에서 빌드/테스트하거나 UPM managed plug-in을 설치합니다. 컬렉션은 해당 타입의 push/pop/enqueue/dequeue 계약으로 사용하고 reflection은 멤버 탐색·생성 결과를 확인합니다.

## 계약과 제한

PriorityQueue의 기본 순서는 `MinimumFirst`(작은 priority 우선)이며, `PriorityQueueOrder.MaximumFirst`로 큰 priority 우선을 선택할 수 있습니다. UnityEngine 타입을 의존하지 않습니다. UPM은 Runtime/Plugins DLL을 소비하므로 Source~ 수정만으로 Unity DLL이 자동 갱신되지는 않습니다.

## 패키지 요구 사항

- Package ID: `com.jeomseon.core`
- Version: `0.1.3`
- Unity: `6000.6` / `0f1`

## 샘플과 검증

package.json에 Package Manager 샘플이 등록되어 있지 않습니다.

[테스트 코드](../Tests~)

## 코드 지도

- [Deque](../Source~/Jeomseon.Core/Collections/Deque.cs)
- [EnumerableExtensions](../Source~/Jeomseon.Core/Collections/EnumerableExtensions.cs)
- [ListExtensions](../Source~/Jeomseon.Core/Collections/ListExtensions.cs)
- [PriorityQueueOrder, PriorityQueue](../Source~/Jeomseon.Core/Collections/PriorityQueue.cs)
- [QueueExtensions](../Source~/Jeomseon.Core/Collections/QueueExtensions.cs)
- [StackExtensions](../Source~/Jeomseon.Core/Collections/StackExtensions.cs)
- [MemberReflection](../Source~/Jeomseon.Core/Reflection/MemberReflection.cs)
- [RuntimeTypeDiscovery](../Source~/Jeomseon.Core/Reflection/RuntimeTypeDiscovery.cs)
- [TypeActivationFailure, TypeActivationResult](../Source~/Jeomseon.Core/Reflection/TypeActivationResult.cs)
- [TypeActivator](../Source~/Jeomseon.Core/Reflection/TypeActivator.cs)

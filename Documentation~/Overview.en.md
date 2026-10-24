# Jeomseon Core

[한국어](Overview.md) · [Installation](../README.en.md)

## Purpose

Provides Unity-independent Deque, PriorityQueue, collection helpers, and reflection APIs.

## Usage flow

Build/test Source~ as .NET code or install the UPM managed plug-in. Use each collection's push/pop/enqueue/dequeue contract and inspect reflection lookup/activation results.

## Contracts and limitations

PriorityQueue processes lower priorities first. There is no UnityEngine dependency. UPM consumes Runtime/Plugins DLLs; editing Source~ alone does not update that DLL.

## Package requirements

- Package ID: `com.jeomseon.core`
- Version: `0.1.3`
- Unity: `6000.6` / `0f1`

## Samples and verification

No Package Manager sample is declared in package.json.

[Test sources](../Tests~)

## Source map

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

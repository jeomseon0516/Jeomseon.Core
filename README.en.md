# Jeomseon Core

[한국어](README.md) | English

Provides Unity-independent Deque, PriorityQueue, collection helpers, and reflection APIs.

## Quick start

Build/test Source~ as .NET code or install the UPM managed plug-in. Use each collection's push/pop/enqueue/dequeue contract and inspect reflection lookup/activation results.

## Contracts and limitations

PriorityQueue processes lower priorities first. There is no UnityEngine dependency. UPM consumes Runtime/Plugins DLLs; editing Source~ alone does not update that DLL.

## Requirements

- Unity `6000.6.0f1`
- Package: `com.jeomseon.core` `0.1.3`

## Installation

Merge this into your project's `Packages/manifest.json`; preserve existing dependencies and registries.

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

Or use **Install package from git URL** in Package Manager:

```text
https://github.com/jeomseon0516/Jeomseon.Core.git#upm/0.1.3
```

## Samples

No Package Manager sample is registered. Follow the migration/build instructions above.

## Tests

[Test sources](Tests~)

Run `./build.sh` to execute the .NET tests and produce the plug-in/NuGet artifacts.

## Further reading

[Complete source map and package metadata](Documentation~/Overview.en.md)
- [Changelog](CHANGELOG.md)

## License

[License](LICENSE.md)

# NativeAOT Host + Plugin Sample

This repository demonstrates a NativeAOT plugin pattern in .NET 10 where:

- `Host` is published as a native executable.
- `Plugin` is published as a native shared library.
- The host loads the plugin at runtime and invokes a C ABI export (`execute_plugin`) via `NativeLibrary` + function pointer marshalling.

## Projects

- `Contracts`
  - Shared compile-time contract (`IPlugin`).
  - Useful for code organization and optional managed fallback paths.

- `Plugin`
  - Contains plugin logic.
  - Exposes native export:
    - `[UnmanagedCallersOnly(EntryPoint = "execute_plugin", CallConvs = [typeof(CallConvCdecl)])]`
  - Built as a shared native library (`<NativeLib>Shared</NativeLib>`).

- `Host`
  - Native console app.
  - Loads `Plugin.dll`, resolves `execute_plugin`, calls it through a `cdecl` delegate.

## Architecture Flow

1. Host starts (`Host.exe`).
2. Host resolves `Plugin.dll` from its base directory.
3. Host calls `NativeLibrary.Load` and `NativeLibrary.GetExport("execute_plugin")`.
4. Host marshals the function pointer to a managed delegate with `CallingConvention.Cdecl`.
5. Host invokes the plugin export.

## Build and Publish

`Directory.Build.props` applies these defaults solution-wide:

- `<PublishAot>true</PublishAot>`
- `<PublishDir>.../artifacts/</PublishDir>`

Publish for `win-x64`:

```bash
dotnet publish Host/Host.csproj -c Release -r win-x64
dotnet publish Plugin/Plugin.csproj -c Release -r win-x64
```

Expected outputs (under `artifacts/`):

- `Host.exe`
- `Plugin.dll`

Place `Plugin.dll` next to `Host.exe` before running.

## Notes on Size

NativeAOT binaries include runtime support code. Even small plugins can be hundreds of KB to around 1 MB on `win-x64`. That is expected for this deployment model.

## Pros and Cons

### Pros

- Fast startup and predictable runtime behavior.
- No dependency on a separately installed .NET runtime.
- Clear ABI boundary (`execute_plugin`) for cross-language or low-level interop.
- Good fit for constrained or locked-down deployment environments.

### Cons

- Larger binary size than a pure managed class library.
- Reduced runtime flexibility vs managed reflection-based plugin discovery.
- ABI compatibility is now your responsibility (calling convention, symbol naming, argument layout).
- NativeAOT limitations can affect dynamic code patterns and reflection-heavy designs.

## NativeAOT-Compatible Invocation Alternatives

Beyond a single flat C export like `execute_plugin`, you can use:

- Delegate export table
  - Plugin exports one function such as `get_plugin_api()` that returns a versioned struct of function pointers.
  - Better ABI evolution and capability discovery than many independent exports.

- COM/WinRT boundary (Windows-focused)
  - Plugin exposes COM-style interfaces and the host calls through interface vtables.
  - Standardized binary contracts, but more platform-specific complexity.

- IPC boundary (out-of-process plugin)
  - Host communicates with plugin over Named Pipes, gRPC, sockets, or HTTP.
  - Strong process isolation and runtime independence, with serialization and lifecycle overhead.

- Embedded runtime model (Lua/JS/Wasm)
  - Host embeds a VM and executes plugin scripts/modules rather than loading native plugin DLLs directly.
  - Smaller plugin payloads and high flexibility, but less direct host API access and more embedding work.

- Message-based extension model
  - Host and plugin communicate through explicit commands/events rather than direct function invocation.
  - Looser coupling and easier long-term extensibility, but added architecture and debugging complexity.

## Current Limitations in This Sample

- Plugin discovery is static (`Plugin.dll` hardcoded in host).
- The native export currently uses no arguments and no return value.
- No explicit host/plugin ABI version negotiation yet.

## Next Improvements

1. Add a versioned ABI struct and explicit init/teardown exports.
2. Support plugin directory scanning with signature or hash validation.
3. Define error codes and logging callbacks across the ABI boundary.

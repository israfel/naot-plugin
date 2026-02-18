# NativeAOT Host + Plugin Sample

This repository demonstrates a NativeAOT plugin pattern in .NET 10 where:

- `Host` is published as a native executable.
- `Plugin` is published as a native shared library.
- The host loads the plugin at runtime and invokes a C ABI export (`execute_plugin`) via `NativeLibrary` + function pointer marshalling.
- Request data is passed across the ABI boundary as UTF-8 JSON and mapped to `IPluginRequest` (`PluginRequest`).

## Projects

- `Contracts`
  - Shared compile-time contracts (`IPlugin`, `IPluginRequest`, `PluginRequest`).
  - Shared `System.Text.Json` source-generation context (`ContractsJsonContext`) for AOT-safe serialization.
  - Useful for code organization and optional managed fallback paths.

- `Plugin`
  - Contains plugin logic.
  - Exposes native export:
    - `[UnmanagedCallersOnly(EntryPoint = "execute_plugin", CallConvs = [typeof(CallConvCdecl)])]`
  - Export signature accepts a request payload pointer (`nint requestJsonUtf8Ptr`).
  - Built as a shared native library (`<NativeLib>Shared</NativeLib>`).

- `Host`
  - Native console app.
  - Loads `Plugin.dll`, resolves `execute_plugin`, serializes `PluginRequest` with source-generated JSON metadata, and calls the export through a `cdecl` delegate.

## Architecture Flow

1. Host starts (`Host.exe`).
2. Host resolves `Plugin.dll` from its base directory.
3. Host calls `NativeLibrary.Load` and `NativeLibrary.GetExport("execute_plugin")`.
4. Host marshals the function pointer to a managed delegate with `CallingConvention.Cdecl`.
5. Host serializes `PluginRequest` to UTF-8 JSON and passes its pointer to `execute_plugin`.
6. Plugin converts the pointer back to JSON, deserializes with `ContractsJsonContext`, and executes `IPlugin.Execute(IPluginRequest)`.

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
- The native export currently has a request argument but still no return value/error code contract.
- Request payload schema and ABI versioning are not negotiated explicitly.

## Next Improvements

1. Add a versioned ABI struct and explicit init/teardown exports.
2. Support plugin directory scanning with signature or hash validation.
3. Define error codes and logging callbacks across the ABI boundary.

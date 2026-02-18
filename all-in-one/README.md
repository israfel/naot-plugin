# NAOT Plugin Solution

## Solution Structure

This is a .NET 10 solution demonstrating a plugin architecture with Native AOT compilation:

- **Contracts** - Library defining the `IPlugin` interface that all plugins implement
- **Plugin** - Plugin implementation that implements `IPlugin` with sample functionality
- **Host** - Console application that loads and executes plugins with Native AOT enabled

## Publishing with Native AOT

To publish the Host application with Native AOT compilation:

```bash
dotnet publish Host -c Release -r win-x64
```

This produces a self-contained, AOT-compiled executable at:
```
Host/bin/Release/net10.0/win-x64/publish/Host.exe
```

Native AOT is enabled via `<PublishAot>true</PublishAot>` in the Host.csproj, which compiles the .NET application directly to native machine code for faster startup and reduced memory footprint.

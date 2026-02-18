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
./artifacts/Host.exe
```

Native AOT is enabled via `<PublishAot>true</PublishAot>` in the Host.csproj, which compiles the .NET application directly to native machine code for faster startup and reduced memory footprint.

## Native AOT & Direct Dependencies

The Host has a direct project reference to Plugin (`<ProjectReference Include="..\Plugin\Plugin.csproj" />`). In Native AOT compilation, this means:

- **Static Binding**: Plugin code is compiled directly into the Host executable, not loaded as a separate DLL at runtime
- **Known at Compile Time**: The AOT compiler analyzes all Plugin code during the build and includes it in the native binary
- **No Dynamic Loading**: You cannot swap Plugin implementations at runtime or load different versions dynamically


# Plugin Solution

## Solution Structure

This is a .NET 10 solution demonstrating a plugin architecture with managed code:

- **Contracts** - Library defining the `IPlugin` interface that all plugins implement
- **Plugin** - Plugin implementation that implements `IPlugin` with sample functionality
- **Host** - Console application that loads and executes plugins as managed code

## Publishing

To publish the Host application:

```bash
dotnet publish Host -c Release -r win-x64
```

This produces a self-contained executable at:
```
./artifacts/Host.exe
```

The application runs as managed .NET code with the runtime handling execution and memory management.

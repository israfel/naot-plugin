using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Contracts;

namespace Plugin;

public class Plugin : IPlugin
{
    public string Name => "MyPlugin";

    public void Execute()
    {
        Console.WriteLine("Hello from MyPlugin!");
    }
}

public static class PluginExports
{
    [UnmanagedCallersOnly(EntryPoint = "execute_plugin", CallConvs = [typeof(CallConvCdecl)])]
    public static void ExecutePlugin()
    {
        new Plugin().Execute();
    }
}

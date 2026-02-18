// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

Console.WriteLine("Hello from Host!");

ExecuteNativePlugin("Plugin.dll");

void ExecuteNativePlugin(string assemblyPath)
{
    string pluginPath = GetPluginPath(assemblyPath);
    nint handle = NativeLibrary.Load(pluginPath);
    nint symbol = NativeLibrary.GetExport(handle, "execute_plugin");

    var pluginFunc = Marshal.GetDelegateForFunctionPointer<ExecutePluginDelegate>(symbol);
    pluginFunc();
}

static string GetPluginPath(string pluginName)
{
    string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginName);
    if (!File.Exists(pluginPath))
    {
        throw new FileNotFoundException($"Plugin not found: {pluginPath}");
    }

    return pluginPath;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
delegate void ExecutePluginDelegate();

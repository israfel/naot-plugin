using System.Runtime.InteropServices;
using System.Text.Json;
using Contracts;

Console.WriteLine("Hello from Host!");

ExecuteNativePlugin("Plugin.dll", new PluginRequest
{
    RequestData = "Payload from Host"
});

void ExecuteNativePlugin(string assemblyPath, IPluginRequest request)
{
    string pluginPath = GetPluginPath(assemblyPath);
    nint handle = NativeLibrary.Load(pluginPath);
    try
    {
        nint symbol = NativeLibrary.GetExport(handle, "execute_plugin");
        var pluginFunc = Marshal.GetDelegateForFunctionPointer<ExecutePluginDelegate>(symbol);

        string requestJson = JsonSerializer.Serialize(
            new PluginRequest { RequestData = request.RequestData },
            ContractsJsonContext.Default.PluginRequest);

        nint requestPtr = Marshal.StringToCoTaskMemUTF8(requestJson);
        try
        {
            pluginFunc(requestPtr);
        }
        finally
        {
            Marshal.FreeCoTaskMem(requestPtr);
        }
    }
    finally
    {
        NativeLibrary.Free(handle);
    }
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
delegate void ExecutePluginDelegate(nint requestJsonUtf8Ptr);

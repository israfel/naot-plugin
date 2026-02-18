using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Contracts;

namespace Plugin;

public class Plugin : IPlugin
{
    public string Name => "MyPlugin";

    public void Execute(IPluginRequest request)
    {
        Console.WriteLine($"Hello from MyPlugin! Request data: {request.RequestData}");
    }
}

public static class PluginExports
{
    [UnmanagedCallersOnly(EntryPoint = "execute_plugin", CallConvs = [typeof(CallConvCdecl)])]
    public static void ExecutePlugin(nint requestJsonUtf8Ptr)
    {
        if (requestJsonUtf8Ptr == nint.Zero)
        {
            throw new ArgumentNullException(nameof(requestJsonUtf8Ptr));
        }

        string requestJson = Marshal.PtrToStringUTF8(requestJsonUtf8Ptr)
            ?? throw new InvalidOperationException("Request payload pointer is invalid.");

        PluginRequest request = JsonSerializer.Deserialize(
            requestJson,
            ContractsJsonContext.Default.PluginRequest)
            ?? throw new InvalidOperationException("Request payload cannot be deserialized.");

        new Plugin().Execute(request);
    }
}

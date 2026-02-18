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

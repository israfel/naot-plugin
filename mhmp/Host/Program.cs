// See https://aka.ms/new-console-template for more information
using Contracts;

Console.WriteLine("Hello from Host!");

IPlugin plugin = LoadPlugin("Plugin.dll");

IPlugin LoadPlugin(string pluginName)
{
    string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginName);
    if (!File.Exists(pluginPath))
    {
        throw new FileNotFoundException($"Plugin not found: {pluginPath}");
    }
    var assembly = System.Reflection.Assembly.LoadFrom(pluginPath);
    var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface);
    if (pluginType == null)
    {
        throw new Exception("No plugin found in assembly.");
    }
    return (IPlugin?)Activator.CreateInstance(pluginType) ?? throw new Exception("Failed to create plugin instance.");
}

Console.WriteLine($"Loaded plugin: {plugin.Name}");
plugin.Execute();

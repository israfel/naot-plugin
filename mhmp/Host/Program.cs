// See https://aka.ms/new-console-template for more information
using Contracts;

Console.WriteLine("Hello from Host!");

IPlugin plugin = new Plugin.Plugin();
Console.WriteLine($"Loaded plugin: {plugin.Name}");
plugin.Execute();

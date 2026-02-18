namespace Contracts;

public interface IPlugin
{
    string Name { get; }
    void Execute(IPluginRequest request);
}

public interface IPluginRequest
{
    string RequestData { get; }
}

public sealed class PluginRequest : IPluginRequest
{
    public required string RequestData { get; init; }
}

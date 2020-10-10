using System;

namespace Datafy.Core
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        ILogger Logger { get; set; }
    }
}
using System;

namespace Datafy.Core
{
    public interface IDriver : IPlugin
    {
        ISerializer Serializer { get; set; }

        bool Initialize(DriverConfig config);
        bool LoadAll(Transaction transaction);
    }
}
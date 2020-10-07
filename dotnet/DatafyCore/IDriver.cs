using System;

namespace DatafyCore
{
    public interface IDriver : IPlugin
    {
        ISerializer Serializer { get; set; }

        bool Initialize(DriverConfig config);
        bool LoadAll(Transaction transaction);
    }
}
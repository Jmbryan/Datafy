using System;

namespace DatafyCore
{
    public interface IDriver : IPlugin
    {
        void LoadAll();
    }
}
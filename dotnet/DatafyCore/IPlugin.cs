using System;
using System.Collections.Generic;
using System.Text;

namespace DatafyCore
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
    }
}
using System;
using DatafyCore;

namespace DatafyDriverJSON
{
    public class Driver : IDriver
    {
        public string Name => "JSON Driver";
        public string Description => "Simple JSON Database";

        public void LoadAll()
        {
            Console.WriteLine("JSON Driver - LoadAll");
        }
    }
}
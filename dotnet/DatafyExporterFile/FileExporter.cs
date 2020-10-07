using System;
using DatafyCore;

namespace DatafyExporter
{
    public class FileExporter : IExporter
    {
        public string Name => "File Exporter";
        public string Description => "Export to local file storage";
        public ILogger Logger { get; set; }
    }
}

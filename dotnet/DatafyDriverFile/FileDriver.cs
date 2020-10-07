using System;
using System.IO;
using DatafyCore;

namespace DatafyDriverFile
{
    public class FileDriver : IDriver
    {
        public string Name => "File Driver";
        public string Description => "Read/write to local file storage";
        public ILogger Logger { get; set; }
        public ISerializer Serializer { get; set; }

        private DriverConfig m_config;

        public bool Initialize(DriverConfig config)
        {
            m_config = config;

            // Ensure directories exists
            if (!Directory.Exists(m_config.RootDirectory))
            {
                Directory.CreateDirectory(m_config.RootDirectory);
            }
            if (!Directory.Exists(m_config.ClassDirectory))
            {
                Directory.CreateDirectory(m_config.ClassDirectory);
            }

            return true;
        }

        public bool LoadAll(Transaction transaction)
        {
            if (!LoadClasses(m_config.ClassDirectory, transaction))
            {
                // TODO
            }

            return true;
        }

        private bool LoadClasses(string directory, Transaction transaction)
        {
            string[] filenames = Directory.GetFiles(directory);
            foreach (string filename in filenames)
            {
                if (!LoadClass(filename, transaction))
                {
                    // TODO: record the error
                }
            }

            return true;
        }

        private bool LoadClass(string filename, Transaction transaction)
        {
            string json = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            var loadedClass = Serializer.DeserializeClass(json);
            if (loadedClass == null)
            {
                return false;
            }

            transaction.AddClass(loadedClass);

            return true;
        }
    }
}
using System;
using System.IO;
using Datafy.Core;

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
            if (!Directory.Exists(m_config.TypeDirectory))
            {
                Directory.CreateDirectory(m_config.TypeDirectory);
            }
            if (!Directory.Exists(m_config.ObjectDirectory))
            {
                Directory.CreateDirectory(m_config.ObjectDirectory);
            }

            return true;
        }

        public bool LoadAll(Transaction transaction)
        {
            if (!LoadTypes(m_config.TypeDirectory, transaction))
            {
                // TODO
            }
            if (!LoadObjects(m_config.ObjectDirectory, transaction))
            {
                // TODO
            }

            return true;
        }

        private bool LoadTypes(string directory, Transaction transaction)
        {
            string[] filenames = Directory.GetFiles(directory);
            foreach (string filename in filenames)
            {
                if (!LoadType(filename, transaction))
                {
                    // TODO
                }
            }

            return true;
        }

        private bool LoadType(string filename, Transaction transaction)
        {
            string json = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            var type = Serializer.DeserializeType(json);
            if (type == null)
            {
                return false;
            }

            transaction.AddType(type);

            return true;
        }

        private bool LoadObjects(string directory, Transaction transaction)
        {
            string[] filenames = Directory.GetFiles(directory);
            foreach (string filename in filenames)
            {
                if (!LoadObject(filename, transaction))
                {
                    // TODO
                }
            }

            return true;
        }

        private bool LoadObject(string filename, Transaction transaction)
        {
            string json = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            var obj = Serializer.DeserializeObject(json);
            if (obj == null)
            {
                return false;
            }

            transaction.AddObject(obj);

            return true;
        }
    }
}
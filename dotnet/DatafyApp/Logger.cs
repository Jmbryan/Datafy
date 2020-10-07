using System;

namespace DatafyApp
{
    class Logger : DatafyCore.ILogger
    {
        private readonly string m_logFilePath;
        private System.IO.StreamWriter m_logFile;

        public Logger(string logFilePath)
        {
            m_logFilePath = logFilePath;
        }

        public void Connect()
        {
            Disconnect();

            m_logFile = new System.IO.StreamWriter(m_logFilePath, true, System.Text.Encoding.UTF8)
            {
                AutoFlush = true
            };

            WriteLine("-------------------- New Log Started --------------------");
        }

        public void Disconnect()
        {
            if (m_logFile != null)
            {
                m_logFile.Close();
                m_logFile = null;
            }
        }

        public void WriteLine(string message)
        {
            Console.Error.WriteLine(message);

            if (m_logFile != null)
            {
                m_logFile.WriteLine(message);
            }
        }
    }
}
using System;

namespace DatafyCore
{
    public class TransactionError
    {
        public string Message { get; }

        public TransactionError(string message)
        {
            Message = message;
        }
    }
}

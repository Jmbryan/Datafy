using System;

namespace Datafy.Core
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

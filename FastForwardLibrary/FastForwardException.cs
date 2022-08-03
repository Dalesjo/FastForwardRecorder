using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FastForwardLibrary
{
    [Serializable]
    internal class FastForwardException : Exception
    {
        public FastForwardException()
        {
        }

        public FastForwardException(string? message) : base(message)
        {
        }

        public FastForwardException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected FastForwardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

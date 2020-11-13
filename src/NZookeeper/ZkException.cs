using System;

namespace NZookeeper
{
    public class ZkException : Exception
    {
        public ZkException(string message) : base(message)
        {

        }

        public ZkException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
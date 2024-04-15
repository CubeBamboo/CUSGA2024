using System;

namespace Shuile.Network
{
    public class ConnectionStringFormatInvalidException : Exception
    {
        public readonly string connectionString;

        public ConnectionStringFormatInvalidException(string connectionString) : base($"The connection string \"{connectionString}\" is invalid")
        {
            this.connectionString = connectionString;
        }
    }
}

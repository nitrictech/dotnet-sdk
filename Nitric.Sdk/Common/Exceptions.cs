namespace Nitric.Api.Common
{
    //Exception raised when the requested operation isn't supported by the server.
    public class UnimplementedException : System.Exception
    {
        public UnimplementedException(string message) : base(message)
        {
        }
    }
    //Exception raised when an entity already exist during a request to create a new entity.
    public class AlreadyExistsException : System.Exception
    {
        public AlreadyExistsException(string message) : base(message)
        {
        }
    }
    //Exception raised when a gRPC service is unavailable.
    class UnavailableException : System.Exception
    {
        public UnavailableException(string message) : base(message)
        {
        }
    }
}
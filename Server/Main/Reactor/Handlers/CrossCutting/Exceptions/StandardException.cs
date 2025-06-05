namespace Server.Main.Reactor.Handlers.CrossCutting.Exceptions;

public class StandardException : Exception
{
    public int StatusCode { get; }
    public StandardException(string message, int statusCode = StatusCodes.Status500InternalServerError) : base(message)
    {
        StatusCode = statusCode;
    }
}

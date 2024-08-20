namespace PamelloV6.API.Exceptions
{
    public class PamelloException : Exception
    {
        public PamelloException() : base() { }
        public PamelloException(string message) : base(message) { }
    }
}

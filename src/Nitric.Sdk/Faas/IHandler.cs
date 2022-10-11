namespace Nitric.Sdk.Faas
{
    /// <summary>
    /// Represents a function to be used to handle incoming requests.
    /// </summary>
    /// <typeparam name="Ctx">The request context.</typeparam>
    public interface IHandler<Ctx>
    {
        /// <summary>
        /// Invoke the handler with the provided context
        /// </summary>
        /// <param name="ctx">the context to invoke</param>
        /// <returns></returns>
        public Ctx Invoke(Ctx ctx);
    }
}

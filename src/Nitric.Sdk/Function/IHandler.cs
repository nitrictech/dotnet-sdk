namespace Nitric.Sdk.Function
{
    /// <summary>
    /// Represents a function to be used to handle incoming requests.
    /// </summary>
    /// <typeparam name="TCtx">The request context.</typeparam>
    public interface IHandler<TCtx>
    {
        /// <summary>
        /// Invoke the handler with the provided context
        /// </summary>
        /// <param name="ctx">the context to invoke</param>
        /// <returns></returns>
        public TCtx Invoke(TCtx ctx);
    }
}

namespace Nitric.Sdk.Function
{
    /// <summary>
    /// Represents a chainable handler for incoming requests. Useful for decorating existing handlers.
    /// </summary>
    /// <typeparam name="TCtx">The request context.</typeparam>
    public interface IMiddleware<TCtx>
    {
        /// <summary>
        /// Invoke the middleware to process the provided context object.
        /// </summary>
        /// <param name="ctx">the context to process</param>
        /// <param name="next">any future middleware or handlers to call next.</param>
        /// <returns></returns>
        public TCtx Invoke(TCtx ctx, IHandler<TCtx> next);
    }
}

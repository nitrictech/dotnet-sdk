namespace Nitric.Sdk.Faas
{
    /// <summary>
    /// Represents a chainable handler for incoming requests. Useful for decorating existing handlers.
    /// </summary>
    /// <typeparam name="Ctx">The request context.</typeparam>
    public interface IMiddleware<Ctx>
    {
        /// <summary>
        /// Invoke the middleware to process the provided context object.
        /// </summary>
        /// <param name="ctx">the context to process</param>
        /// <param name="next">any future middleware or handlers to call next.</param>
        /// <returns></returns>
        public Ctx Invoke(Ctx ctx, IHandler<Ctx> next);
    }
}

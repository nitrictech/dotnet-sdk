using System;

namespace Nitric.Sdk.Function
{
    /// <summary>
    /// Represents a chainable handler for incoming requests. Useful for decorating existing handlers.
    /// </summary>
    /// <typeparam name="TCtx">The request context.</typeparam>

    public delegate TCtx Middleware<TCtx>(TCtx ctx, Func<TCtx, TCtx> next);
}

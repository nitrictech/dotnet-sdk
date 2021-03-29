using System;
namespace Nitric.Api.Http
{
    public interface IHttpHandler
    {
        HttpResponse handle(HttpRequest request);
    }
}

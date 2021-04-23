using System;
namespace Nitric.Api.Http
{
    public interface IHttpHandler
    {
        HttpResponse Handle(HttpRequest request);
    }
}

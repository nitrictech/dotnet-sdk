using System;
namespace Nitric.Api.Faas
{
    public interface INitricFunction
    {
        //Handle the function request.
        NitricResponse handle(NitricRequest request);
    }
}

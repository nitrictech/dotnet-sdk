using System;
using Nitric.Api.Faas;

namespace TestCall
{
    class Program
    {
        static void Main(string[] args)
        {
            Faas faas = new Faas();
            faas.Start(new HelloWorld());

        }
    }
    class HelloWorld : INitricFunction
    {
        public NitricResponse Handle(NitricRequest request)
        {
            return new NitricResponse.Builder().BodyText("Hello World").Build();
        }
    }
}

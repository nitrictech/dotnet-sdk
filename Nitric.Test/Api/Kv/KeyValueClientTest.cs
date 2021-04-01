using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Nitric.Api.KeyValue;
namespace Nitric.Test.Api.Common
{
    [TestClass]
    public class KeyValueClientTest
    {
        static readonly string KnownKey = "john.smith@gmail.com";
        static readonly Dictionary<string, object> KnownDict = new Dictionary<string, object>
        { {"name", "John Smith" } };
        static readonly Struct KnownStruct = Nitric.Api.Common.Util.ObjectToStruct(KnownDict);

        [TestMethod]
        public void TestBuild()
        {
            var client = new KeyValueClient();

            Assert.IsNotNull(client);
        }
    }
}

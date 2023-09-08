// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Nitric.Sdk.Document;
using Xunit;
using Xunit.Abstractions;

namespace Nitric.Sdk.Test.Api.Document
{
    public class TestProfile
    {
        public string Name { get; set; }
        public float Age { get; set; }
        public bool Employed { get; set; }
        public List<string> Hobbies { get; set; }
        public float[][] Numbers { get; set; }
        public TestAddress Address { get; set; }
    }

    public class TestAddress
    {
        public string Street { get; set; }
    }

    public class DocumentToGenericTest : DocumentRef<TestProfile>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DocumentToGenericTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public string ToAssertableString(IDictionary<string, object> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }

        [Fact]
        public void TestStructWithScalars()
        {
            var genericStruct = new Struct();
            genericStruct.Fields.Add("name", Value.ForString("John Smith"));
            genericStruct.Fields.Add("age", Value.ForNumber(21.0));
            genericStruct.Fields.Add("employed", Value.ForBool(true));

            var actual = DocumentToGeneric(genericStruct);

            var expected = new TestProfile {
                Name = "John Smith",
                Age = 21,
                Employed = true,
            };

            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void TestStructWithList()
        {
            var genericList = new List<Value> { Value.ForString("Reading"), Value.ForString("Writing"), Value.ForString("Deleting") };

            var genericStruct = new Struct();
            genericStruct.Fields.Add("name", Value.ForString("John Smith"));
            genericStruct.Fields.Add("age", Value.ForNumber(21));
            genericStruct.Fields.Add("employed", Value.ForBool(true));
            genericStruct.Fields.Add("hobbies", Value.ForList(genericList.ToArray()));

            var expectedList = new List<string> { "Reading", "Writing", "Deleting" };

            var expected = new TestProfile {
                Name = "John Smith",
                Age = 21,
                Employed = true,
                Hobbies = expectedList,
            };

            var actual = DocumentToGeneric(genericStruct);

            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void TestStructWithNestedLists()
        {
            var genericStruct = new Struct();
            genericStruct.Fields.Add("name", Value.ForString("John Smith"));
            genericStruct.Fields.Add("age", Value.ForNumber(21));
            genericStruct.Fields.Add("employed", Value.ForBool(true));
            genericStruct.Fields.Add("numbers", Value.ForList(new[] {
                Value.ForList(Value.ForNumber(0), Value.ForNumber(1)),
                Value.ForList(Value.ForNumber(2), Value.ForNumber(3)),
            }));

            var expected = new TestProfile {
                Name = "John Smith",
                Age = 21,
                Employed = true,
                Numbers = new float[][] { new float[] { 0, 1}, new float[] { 2, 3 }}
            };

            var actual = DocumentToGeneric(genericStruct);

            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void TestStructWithNestedStruct()
        {
            var nestedStruct = new Struct();
            nestedStruct.Fields.Add("street", Value.ForString("123 street st"));

            var genericStruct = new Struct();
            genericStruct.Fields.Add("name", Value.ForString("John Smith"));
            genericStruct.Fields.Add("age", Value.ForNumber(21));
            genericStruct.Fields.Add("employed", Value.ForBool(true));
            genericStruct.Fields.Add("address", Value.ForStruct(nestedStruct));

            var expected = new TestProfile {
                Name = "John Smith",
                Age = 21,
                Employed = true,
                Address = new TestAddress {
                    Street = "123 street st"
                },
            };

            var actual = DocumentToGeneric(genericStruct);

            Assert.Equivalent(expected, actual);
        }
    }
}

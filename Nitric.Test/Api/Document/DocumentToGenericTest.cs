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
using System.Linq;
using System.Collections.Generic;
using Nitric.Api.Document;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.Protobuf.WellKnownTypes;
namespace Nitric.Test.Api.Document
{
    [TestClass]
    public class DocumentToGenericTest : DocumentRef<Dictionary<string, object>>
    {
        public string ToAssertableString(IDictionary<string, object> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                                        .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }
        [TestMethod]
        public void TestStructWithScalars()
        {
            var expected = new Dictionary<string, object>();
            expected.Add("string", "string");
            expected.Add("null", null);
            expected.Add("number", 1.0);
            expected.Add("boolean", true);

            var genericStruct = new Struct();
            genericStruct.Fields.Add("string", Value.ForString("string"));
            genericStruct.Fields.Add("null", Value.ForNull());
            genericStruct.Fields.Add("number", Value.ForNumber(1.0));
            genericStruct.Fields.Add("boolean", Value.ForBool(true));

            var genericDict = DocumentToGeneric(genericStruct);

            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(genericDict));
        }
        [TestMethod]
        public void TestStructWithList()
        {
            List<object> expectedList = new List<object>();
            expectedList.Add("string");
            expectedList.Add(null);
            expectedList.Add(1.0);
            expectedList.Add(true);

            var expected = new Dictionary<string, object>();
            expected.Add("list", expectedList);

            Value[] actualList = new Value[] {
                Value.ForString("string"),
                Value.ForNull(),
                Value.ForNumber(1),
                Value.ForBool(true),
            };

            var genericStruct = new Struct();
            genericStruct.Fields.Add("list", Value.ForList(actualList));

            var genericDict = DocumentToGeneric(genericStruct);

            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(genericDict));
        }
        [TestMethod]
        public void TestStructWithStruct()
        {
            var expected = new Struct();
            expected.Fields.Add("string", Value.ForString("string"));
            expected.Fields.Add("boolean", Value.ForBool(true));
            expected.Fields.Add("number", Value.ForNumber(1.0));
            expected.Fields.Add("null", Value.ForNull());

            var genericStruct = new Struct();
            genericStruct.Fields.Add("struct", Value.ForStruct(expected));

            var genericDict = DocumentToGeneric(genericStruct);

            var fields = new Dictionary<string, object>();
            fields.Add("string", "string");
            fields.Add("boolean", true);
            fields.Add("number", 1.0);
            fields.Add("null", null);

            var expectedResponse = new Dictionary<string, object>();
            expectedResponse.Add("struct", fields);

            Assert.AreEqual(ToAssertableString(expectedResponse), ToAssertableString(genericDict));
        }
        [TestMethod]
        public void TestStructWithNestedLists()
        {
            List<object> nestedList2 = new List<object>();
            nestedList2.Add("string");
            nestedList2.Add(null);
            nestedList2.Add(1.0);
            nestedList2.Add(true);

            List<object> nestedList1 = new List<object>();
            nestedList1.Add(nestedList2);
            nestedList1.Add(nestedList2);
            nestedList1.Add(nestedList2);

            List<object> expectedList = new List<object>();
            expectedList.Add(nestedList1);
            expectedList.Add(nestedList1);
            expectedList.Add(nestedList1);

            var expected = new Dictionary<string, object>();
            expected.Add("list", expectedList);

            Value[] actualNestedList2 = new Value[]
            {
                Value.ForString("string"),
                Value.ForNull(),
                Value.ForNumber(1),
                Value.ForBool(true),
            };
            Value[] actualNestedList1 = new Value[]
            {
                Value.ForList(actualNestedList2),
                Value.ForList(actualNestedList2),
                Value.ForList(actualNestedList2),
            };
            Value[] actualList = new Value[]
            {
                Value.ForList(actualNestedList1),
                Value.ForList(actualNestedList1),
                Value.ForList(actualNestedList1),
            };

            var genericStruct = new Struct();
            genericStruct.Fields.Add("list", Value.ForList(actualList));

            var genericDict = DocumentToGeneric(genericStruct);

            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(genericDict));
        }
        [TestMethod]
        public void TestStructWithNestedListAndNestedStructs()
        {

            Value[] nestedList3 = new Value[]
            {
                Value.ForString("string"),
                Value.ForNull(),
                Value.ForNumber(1),
                Value.ForBool(true),
            };

            Value[] nestedList2 = new Value[]
            {
                Value.ForList(nestedList3),
                Value.ForList(nestedList3),
                Value.ForList(nestedList3),
            };

            var nestedStruct1 = new Struct();
            nestedStruct1.Fields.Add("struct-list", Value.ForList(nestedList2));

            List<object> nestedList1 = new List<object>();
            nestedList1.Add(nestedStruct1);
            nestedList1.Add(nestedStruct1);
            nestedList1.Add(nestedStruct1);

            List<object> expectedList = new List<object>();
            expectedList.Add(nestedList1);
            expectedList.Add(nestedList1);
            expectedList.Add(nestedList1);

            var expected = new Dictionary<string, object>();
            expected.Add("list", expectedList);

            Value[] actualNestedList3 = new Value[]
            {
                Value.ForString("string"),
                Value.ForNull(),
                Value.ForNumber(1),
                Value.ForBool(true),
            };
            Value[] actualNestedList2 = new Value[]
            {
                Value.ForList(actualNestedList3),
                Value.ForList(actualNestedList3),
                Value.ForList(actualNestedList3),
            };

            var actualNestedStruct1 = new Struct();
            actualNestedStruct1.Fields.Add("struct-list", Value.ForList(actualNestedList2));

            Value[] actualNestedList1 = new Value[]
            {
                Value.ForStruct(actualNestedStruct1),
                Value.ForStruct(actualNestedStruct1),
                Value.ForStruct(actualNestedStruct1),
            };
            Value[] actualList = new Value[]
            {
                Value.ForList(actualNestedList1),
                Value.ForList(actualNestedList1),
                Value.ForList(actualNestedList1),
            };

            var genericStruct = new Struct();
            genericStruct.Fields.Add("list", Value.ForList(actualList));

            var genericDict = DocumentToGeneric(genericStruct);

            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(genericDict));
        }
    }
}

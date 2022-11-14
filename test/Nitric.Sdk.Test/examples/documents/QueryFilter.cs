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

// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class QueryFilterExample
    {
        public static void QueryFilter()
        {
            // [START snippet]
            var docs = new DocumentsClient();

            var query = docs.Collection<Dictionary<string, object>>("Customers").Query()
                .Where("country", "==", "US")
                .Where("age", ">=", "21");

            var results = query.Fetch();
            // [END snippet]
        }
    }
}

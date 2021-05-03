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
﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Nitric.Test.Api.Event
{
    [TestClass]

    public class TopicTest
    {
        [TestMethod]
        public void TestBuild()
        {

            var topic = new Nitric.Api.Event.Topic.Builder()
                .Name("topic")
                .Build();

            Assert.IsNotNull(topic);
            Assert.AreEqual("topic", topic.Name);

            try
            {
                topic = new Nitric.Api.Event.Topic.Builder()
                .Build();
                Assert.IsTrue(false);
            } catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'name')", ane.Message);
            }
        }
        [TestMethod]
        public void TestToString()
        {
            var topic = new Nitric.Api.Event.Topic.Builder()
                .Name("Test Topic")
                .Build();
            Assert.AreEqual("Topic[name=Test Topic]", topic.ToString());
        }
    }
}

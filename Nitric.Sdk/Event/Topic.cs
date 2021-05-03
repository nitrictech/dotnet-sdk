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
namespace Nitric.Api.Event
{
    public class Topic
    {
        public string Name { get; private set; }
        public Topic(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return GetType().Name + "[name=" + Name + "]";
        }
        public class Builder
        {
            string name;

            public Builder Name(string name)
            {
                this.name = name;
                return this;
            }
            public Topic Build()
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }
                return new Topic(name);
            }
            public Topic Build(string name)
            {
                return Name(name).Build();
            }
        }
    }
}

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
using Nitric.Proto.Sql.v1;
using GrpcClient = Nitric.Proto.Sql.v1.Sql.SqlClient;
using Nitric.Proto.Resources.v1;
using ResourceType = Nitric.Proto.Resources.v1.ResourceType;
using Nitric.Sdk.Common;
using System.Threading.Tasks;

namespace Nitric.Sdk.Resource
{
    public class SqlResource : BaseResource
    {
        internal readonly GrpcClient Client;

        string Migrations;

        internal SqlResource(string name, string migrations, GrpcClient client = null) : base(name, ResourceType.SqlDatabase)
        {
            this.Client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
            this.Migrations = migrations;
        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest
            {
                Id = this.AsProtoResource(),
                SqlDatabase = new SqlDatabaseResource
                {
                    Migrations = new SqlDatabaseMigrations
                    {
                        MigrationsPath = this.Migrations
                    }
                }
            };
            BaseResource.client.Declare(request);
            return this;
        }

        /// <summary>
        /// Retrieves the connection string of this SQL Database at runtime.
        /// </summary>
        /// <returns>The connection string of this SQL Database</returns>        
        public string ConnectionString()
        {
            var request = new SqlConnectionStringRequest { DatabaseName = this.Name };

            var resp = this.Client.ConnectionString(request);

            return resp.ConnectionString;
        }

        /// <summary>
        /// Retrieves the connection string of this SQL Database at runtime.
        /// </summary>
        /// <returns>The connection string of this SQL Database</returns>  
        public async Task<string> ConnectionStringAsync()
        {
            var request = new SqlConnectionStringRequest { DatabaseName = this.Name };

            var resp = await this.Client.ConnectionStringAsync(request);

            return resp.ConnectionString;
        }
    }
}

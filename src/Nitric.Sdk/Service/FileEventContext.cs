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
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Storage;
using ProtoBlobEventResponse = Nitric.Proto.Storage.v1.BlobEventResponse;
using ProtoBlobEventType = Nitric.Proto.Storage.v1.BlobEventType;

namespace Nitric.Sdk.Service
{
    /// <summary>
    /// Represents a bucket notification being created by a write or delete event in the bucket
    /// </summary>
    public class FileEventRequest : TriggerRequest
    {
        /// <summary>
        /// A reference to the file that triggered this request
        /// </summary>
        public File File { get; private set; }

        /// <summary>
        /// The type of event that triggered this request
        /// </summary>
        /// <returns></returns>
        public BlobEventType NotificationType { get; private set; }

        /// <summary>
        /// Construct a bucket notification request
        /// </summary>
        /// <param name="key">the file that triggered the notification</param>
        /// <param name="notificationType">the type of bucket notification</param>
        public FileEventRequest(File file, BlobEventType notificationType) : base()
        {
            this.File = file;
            this.NotificationType = notificationType;
        }
    }

    /// <summary>
    /// Represents the results of processing a bucket notification.
    /// </summary>
    public class FileEventResponse : TriggerResponse
    {
        /// <summary>
        /// Indicates whether the event was successfully processed.
        ///
        /// If this value is false, the event may be resent.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Construct a bucket notification response.
        /// </summary>
        /// <param name="BlobEventResponse">Indicates whether the event was successfully processed.</param>
        public FileEventResponse(bool success)
        {
            this.Success = success;
        }
    }

    /// <summary>
    /// Represents the request/response context for a permissioned bucket notification.
    /// </summary>
    public class FileEventContext : TriggerContext<FileEventRequest, FileEventResponse>
    {
        /// <summary>
        /// Construct a new BucketNotificationContext.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public FileEventContext(string id, FileEventRequest req, FileEventResponse res) : base(id, req, res)
        {
        }

        /// <summary>
        /// Construct a bucket notification context from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into a BucketNotificationContext.</param>
        /// <param name="options">The bucket notification worker options describing the worker options.</param>
        /// <returns>the new bucket notification context</returns>
        public static FileEventContext FromRequest(ServerMessage trigger, Bucket bucket)
        {
            var notificationType = FromGrpcBlobEventType(trigger.BlobEventRequest.BlobEvent.Type);

            return new FileEventContext(
                trigger.Id,
                new FileEventRequest(
                    bucket.File(trigger.BlobEventRequest.BlobEvent.Key),
                    notificationType
                ),
                new FileEventResponse(true));
        }

        /// <summary>
        /// Convert a gRPC blob event type into a SDK blob event type.
        /// </summary>
        /// <param name="blobEventType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static BlobEventType FromGrpcBlobEventType(
            ProtoBlobEventType blobEventType)
        {
            return blobEventType switch
            {
                ProtoBlobEventType.Created => BlobEventType.Write,
                ProtoBlobEventType.Deleted => BlobEventType.Delete,
                _ => throw new ArgumentException("Unsupported blob event type")
            };
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns>the new trigger response</returns>
        public ClientMessage ToResponse()
        {
            return new ClientMessage { Id = Id, BlobEventResponse = new ProtoBlobEventResponse { Success = this.Res.Success } };
        }
    }
}

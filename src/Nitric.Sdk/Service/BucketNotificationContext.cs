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

namespace Nitric.Sdk.Service
{
    public enum BucketNotificationType
    {
        Write,
        Delete
    }

    /// <summary>
    /// Represents a bucket notification being created by a write or delete event in the bucket
    /// </summary>
    public class BucketNotificationRequest : TriggerRequest
    {
        /// <summary>
        /// A reference to the file that triggered this request
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// The type of event that triggered this request
        /// </summary>
        /// <returns></returns>
        public BucketNotificationType NotificationType { get; private set; }

        /// <summary>
        /// Construct a bucket notification request
        /// </summary>
        /// <param name="data">the payload of the message</param>
        /// <param name="key">the file that triggered the notification</param>
        /// <param name="notificationType">the type of bucket notification</param>
        /// <param name="bucket">the bucket that triggered the notification</param>
        public BucketNotificationRequest(string key, BucketNotificationType notificationType) : base()
        {
            this.Key = key;
            this.NotificationType = notificationType;
        }
    }

    /// <summary>
    /// Represents the results of processing a bucket notification.
    /// </summary>
    public class BucketNotificationResponse : TriggerResponse
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
        /// <param name="success">Indicates whether the event was successfully processed.</param>
        public BucketNotificationResponse(bool success)
        {
            this.Success = success;
        }
    }

    /// <summary>
    /// Represents the request/response context for a bucket notification.
    /// </summary>
    public class BucketNotificationContext : TriggerContext<BucketNotificationRequest, BucketNotificationResponse>
    {
        /// <summary>
        /// Construct a new BucketNotificationContext.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public BucketNotificationContext(string id, BucketNotificationRequest req, BucketNotificationResponse res) : base(id, req, res)
        {
        }

        /// <summary>
        /// Construct a bucket notification context from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into a BucketNotificationContext.</param>
        /// <param name="options">The bucket notification worker options describing the worker options.</param>
        /// <returns>the new bucket notification context</returns>
        public static BucketNotificationContext FromRequest(ServerMessage trigger)
        {
            var notificationType = FromGrpcBucketNotificationType(trigger.BlobEventRequest.BlobEvent.Type);

            return new BucketNotificationContext(
                trigger.Id,
                new BucketNotificationRequest(
                    trigger.BlobEventRequest.BlobEvent.Key,
                    notificationType
                ),
                new BucketNotificationResponse(true));
        }


        private static BucketNotificationType FromGrpcBucketNotificationType(
            BlobEventType notificationType)
        {
            return notificationType switch
            {
                BlobEventType.Created => BucketNotificationType.Write,
                BlobEventType.Deleted => BucketNotificationType.Delete,
                _ => throw new ArgumentException("Unsupported bucket notification type")
            };
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns>the new trigger response</returns>
        public ClientMessage ToResponse()
        {
            return new ClientMessage { Id = Id, BlobEventResponse = new BlobEventResponse { Success = this.Res.Success } };
        }
    }
}

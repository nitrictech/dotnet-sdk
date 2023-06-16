using System;
using System.Text;
using Nitric.Proto.Faas.v1;
using Nitric.Sdk.Storage;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;
using BucketNotificationTypeProto = Nitric.Proto.Faas.v1.BucketNotificationType;

namespace Nitric.Sdk.Function
{
    public enum BucketNotificationType
    {
        Write,
        Delete
    }

    /// <summary>
    /// Represents a bucket notification being created by a write or delete event in the bucket
    /// </summary>
    public class BucketNotificationRequest : AbstractRequest
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
        public BucketNotificationRequest(byte[] data, string key, BucketNotificationType notificationType) : base(data)
        {
            this.Key = key;
            this.NotificationType = notificationType;
        }
    }


    /// <summary>
    /// Represents a bucket notification being created by a write or delete event in the bucket
    /// </summary>
    public class FileNotificationRequest : AbstractRequest
    {
        /// <summary>
        /// A reference to the file that triggered this request
        /// </summary>
        public File File { get; private set; }

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
        public FileNotificationRequest(byte[] data, string key, BucketNotificationType notificationType, Bucket bucket) : base(data)
        {
            this.File = bucket.File(key);
            this.NotificationType = notificationType;
        }
    }

    /// <summary>
    /// Represents the results of processing a bucket notification.
    /// </summary>
    public class BucketNotificationResponse
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
        public BucketNotificationContext(BucketNotificationRequest req, BucketNotificationResponse res) : base(req, res)
        {
        }

        /// <summary>
        /// Construct a bucket notification context from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into a BucketNotificationContext.</param>
        /// <param name="options">The bucket notification worker options describing the worker options.</param>
        /// <returns>the new bucket notification context</returns>
        public static BucketNotificationContext FromGrpcTriggerRequest(TriggerRequestProto trigger, BucketNotificationWorkerOptions options)
        {
            var notificationType = FromGrpcBucketNotificationType(trigger.Notification.Bucket.Type);

            return new BucketNotificationContext(
                new BucketNotificationRequest(trigger.Data.ToByteArray(), trigger.Notification.Bucket.Key,
                    notificationType),
                new BucketNotificationResponse(true));
        }


        private static BucketNotificationType FromGrpcBucketNotificationType(
            BucketNotificationTypeProto notificationType)
        {
            return notificationType switch
            {
                BucketNotificationTypeProto.Created => BucketNotificationType.Write,
                BucketNotificationTypeProto.Deleted => BucketNotificationType.Delete,
                _ => throw new ArgumentException("Unsupported bucket notification type")
            };
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns>the new trigger response</returns>
        public override TriggerResponse ToGrpcTriggerContext()
        {
            return new TriggerResponse { Notification = new NotificationResponseContext { Success = this.Res.Success } };
        }
    }

        /// <summary>
    /// Represents the request/response context for a bucket notification.
    /// </summary>
    public class FileNotificationContext : TriggerContext<FileNotificationRequest, BucketNotificationResponse>
    {
        /// <summary>
        /// Construct a new BucketNotificationContext.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public FileNotificationContext(FileNotificationRequest req, BucketNotificationResponse res) : base(req, res)
        {
        }

        /// <summary>
        /// Construct a bucket notification context from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into a BucketNotificationContext.</param>
        /// <param name="options">The bucket notification worker options describing the worker options.</param>
        /// <returns>the new bucket notification context</returns>
        public static FileNotificationContext FromGrpcTriggerRequest(TriggerRequestProto trigger, FileNotificationWorkerOptions options)
        {
            var notificationType = FromGrpcBucketNotificationType(trigger.Notification.Bucket.Type);

            return new FileNotificationContext(
                new FileNotificationRequest(trigger.Data.ToByteArray(), trigger.Notification.Bucket.Key,
                    notificationType, options.Bucket),
                new BucketNotificationResponse(true));
        }

        private static BucketNotificationType FromGrpcBucketNotificationType(
            BucketNotificationTypeProto notificationType)
        {
            return notificationType switch
            {
                BucketNotificationTypeProto.Created => BucketNotificationType.Write,
                BucketNotificationTypeProto.Deleted => BucketNotificationType.Delete,
                _ => throw new ArgumentException("Unsupported bucket notification type")
            };
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns>the new trigger response</returns>
        public override TriggerResponse ToGrpcTriggerContext()
        {
            return new TriggerResponse { Notification = new NotificationResponseContext { Success = this.Res.Success }};
        }
    }
}

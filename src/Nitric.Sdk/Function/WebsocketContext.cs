using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Nitric.Proto.Faas.v1;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;
using WebsocketNotificationTypeProto = Nitric.Proto.Faas.v1.WebsocketEvent;

namespace Nitric.Sdk.Function
{
    public enum WebsocketEventType
    {
        Connected,
        Disconnected,
        Message
    }

    public class WebsocketRequest : AbstractRequest
    {
        /// <summary>
        /// The name of the websocket that triggered this request
        /// </summary>
        public string Socket { get; private set; }

        /// <summary>
        /// The type of websocket notification
        /// </summary>
        public WebsocketEventType NotificationType { get; private set; }

        /// <summary>
        /// The ID of the individual connection
        /// </summary>
        public string ConnectionId { get; private set; }

        /// <summary>
        /// The websocket query parameters
        /// </summary>
        public Dictionary<string, string[]> Query { get; private set; }

        public T Message<T>() => JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(this.data));

        public string Message() => Encoding.Default.GetString(this.data);

        /// <summary>
        /// Construct an event request
        /// </summary>
        /// <param name="data">the payload of the message</param>
        /// <param name="socket">the source websocket</param>
        /// <param name="notificationType">the type of websocket notification</param>
        /// <param name="connectionId">the id of the individual connection</param>
        /// <param name="query">the websocket query parameters </param>
        public WebsocketRequest(byte[] data, string socket, WebsocketEventType notificationType, string connectionId, Dictionary<string, string[]> query) : base(data)
        {
            this.Socket = socket;
            this.NotificationType = notificationType;
            this.ConnectionId = connectionId;
            this.Query = query;
        }
    }

    public class WebsocketResponse
    {
        /// <summary>
        /// Indicates whether the event was successfully processed.
        ///
        /// If this value is false, the event may be resent.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Construct an event response.
        /// </summary>
        /// <param name="success">Indicates whether the event was successfully processed.</param>
        public WebsocketResponse(bool success)
        {
            this.Success = success;
        }
    }

    public class WebsocketContext : TriggerContext<WebsocketRequest, WebsocketResponse>
    {
        /// <summary>
        /// Construct a new Websocket Context.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public WebsocketContext(WebsocketRequest req, WebsocketResponse res) : base(req, res)
        {
        }

        /// <summary>
        /// Construct an event topic from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into an EventContext.</param>
        /// <returns>the new event context</returns>
        public static WebsocketContext FromGrpcTriggerRequest(TriggerRequestProto trigger)
        {
            var type = FromGrpcWebsocketNotificationType(trigger.Websocket.Event);
            Dictionary<string, string[]> query = trigger.Websocket.QueryParams.Select
                (kv =>
                    new KeyValuePair<string, string[]>(kv.Key, kv.Value.Value.ToArray())
                ).ToDictionary(x => x.Key, x => x.Value);
            return new WebsocketContext(
                new WebsocketRequest(trigger.Data.ToByteArray(), trigger.Websocket.Socket, type, trigger.Websocket.ConnectionId, query),
                new WebsocketResponse(true));
        }

        private static WebsocketEventType FromGrpcWebsocketNotificationType(
            WebsocketNotificationTypeProto notificationType)
        {
            return notificationType switch
            {
                WebsocketNotificationTypeProto.Connect => WebsocketEventType.Connected,
                WebsocketNotificationTypeProto.Disconnect => WebsocketEventType.Disconnected,
                WebsocketNotificationTypeProto.Message => WebsocketEventType.Message,
                _ => throw new ArgumentException("Unsupported bucket notification type")
            };
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns></returns>
        public override TriggerResponse ToGrpcTriggerContext()
        {
            return new TriggerResponse { Websocket = new WebsocketResponseContext { Success = this.Res.Success } };
        }
    }
}

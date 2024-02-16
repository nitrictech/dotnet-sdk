// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: nitric/proto/http/v1/http.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Nitric.Proto.Http.v1 {
  /// <summary>
  /// Service for proxying HTTP requests
  /// </summary>
  public static partial class Http
  {
    static readonly string __ServiceName = "nitric.proto.http.v1.Http";

    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    static readonly grpc::Marshaller<global::Nitric.Proto.Http.v1.ClientMessage> __Marshaller_nitric_proto_http_v1_ClientMessage = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Nitric.Proto.Http.v1.ClientMessage.Parser));
    static readonly grpc::Marshaller<global::Nitric.Proto.Http.v1.ServerMessage> __Marshaller_nitric_proto_http_v1_ServerMessage = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Nitric.Proto.Http.v1.ServerMessage.Parser));

    static readonly grpc::Method<global::Nitric.Proto.Http.v1.ClientMessage, global::Nitric.Proto.Http.v1.ServerMessage> __Method_Proxy = new grpc::Method<global::Nitric.Proto.Http.v1.ClientMessage, global::Nitric.Proto.Http.v1.ServerMessage>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "Proxy",
        __Marshaller_nitric_proto_http_v1_ClientMessage,
        __Marshaller_nitric_proto_http_v1_ServerMessage);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Nitric.Proto.Http.v1.HttpReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Http</summary>
    [grpc::BindServiceMethod(typeof(Http), "BindService")]
    public abstract partial class HttpBase
    {
      /// <summary>
      /// Proxy an HTTP server
      /// </summary>
      /// <param name="requestStream">Used for reading requests from the client.</param>
      /// <param name="responseStream">Used for sending responses back to the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>A task indicating completion of the handler.</returns>
      public virtual global::System.Threading.Tasks.Task Proxy(grpc::IAsyncStreamReader<global::Nitric.Proto.Http.v1.ClientMessage> requestStream, grpc::IServerStreamWriter<global::Nitric.Proto.Http.v1.ServerMessage> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Http</summary>
    public partial class HttpClient : grpc::ClientBase<HttpClient>
    {
      /// <summary>Creates a new client for Http</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public HttpClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Http that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public HttpClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected HttpClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected HttpClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// Proxy an HTTP server
      /// </summary>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::Nitric.Proto.Http.v1.ClientMessage, global::Nitric.Proto.Http.v1.ServerMessage> Proxy(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Proxy(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Proxy an HTTP server
      /// </summary>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::Nitric.Proto.Http.v1.ClientMessage, global::Nitric.Proto.Http.v1.ServerMessage> Proxy(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_Proxy, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override HttpClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new HttpClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(HttpBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Proxy, serviceImpl.Proxy).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, HttpBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Proxy, serviceImpl == null ? null : new grpc::DuplexStreamingServerMethod<global::Nitric.Proto.Http.v1.ClientMessage, global::Nitric.Proto.Http.v1.ServerMessage>(serviceImpl.Proxy));
    }

  }
}
#endregion

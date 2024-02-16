// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: nitric/proto/secrets/v1/secrets.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Nitric.Proto.Secrets.v1 {
  /// <summary>
  /// The Nitric Secret Service
  /// </summary>
  public static partial class SecretManager
  {
    static readonly string __ServiceName = "nitric.proto.secrets.v1.SecretManager";

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

    static readonly grpc::Marshaller<global::Nitric.Proto.Secrets.v1.SecretPutRequest> __Marshaller_nitric_proto_secrets_v1_SecretPutRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Nitric.Proto.Secrets.v1.SecretPutRequest.Parser));
    static readonly grpc::Marshaller<global::Nitric.Proto.Secrets.v1.SecretPutResponse> __Marshaller_nitric_proto_secrets_v1_SecretPutResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Nitric.Proto.Secrets.v1.SecretPutResponse.Parser));
    static readonly grpc::Marshaller<global::Nitric.Proto.Secrets.v1.SecretAccessRequest> __Marshaller_nitric_proto_secrets_v1_SecretAccessRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Nitric.Proto.Secrets.v1.SecretAccessRequest.Parser));
    static readonly grpc::Marshaller<global::Nitric.Proto.Secrets.v1.SecretAccessResponse> __Marshaller_nitric_proto_secrets_v1_SecretAccessResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Nitric.Proto.Secrets.v1.SecretAccessResponse.Parser));

    static readonly grpc::Method<global::Nitric.Proto.Secrets.v1.SecretPutRequest, global::Nitric.Proto.Secrets.v1.SecretPutResponse> __Method_Put = new grpc::Method<global::Nitric.Proto.Secrets.v1.SecretPutRequest, global::Nitric.Proto.Secrets.v1.SecretPutResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Put",
        __Marshaller_nitric_proto_secrets_v1_SecretPutRequest,
        __Marshaller_nitric_proto_secrets_v1_SecretPutResponse);

    static readonly grpc::Method<global::Nitric.Proto.Secrets.v1.SecretAccessRequest, global::Nitric.Proto.Secrets.v1.SecretAccessResponse> __Method_Access = new grpc::Method<global::Nitric.Proto.Secrets.v1.SecretAccessRequest, global::Nitric.Proto.Secrets.v1.SecretAccessResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Access",
        __Marshaller_nitric_proto_secrets_v1_SecretAccessRequest,
        __Marshaller_nitric_proto_secrets_v1_SecretAccessResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Nitric.Proto.Secrets.v1.SecretsReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of SecretManager</summary>
    [grpc::BindServiceMethod(typeof(SecretManager), "BindService")]
    public abstract partial class SecretManagerBase
    {
      /// <summary>
      /// Updates a secret, creating a new one if it doesn't already exist
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      public virtual global::System.Threading.Tasks.Task<global::Nitric.Proto.Secrets.v1.SecretPutResponse> Put(global::Nitric.Proto.Secrets.v1.SecretPutRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      /// <summary>
      /// Gets a secret from a Secret Store
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      public virtual global::System.Threading.Tasks.Task<global::Nitric.Proto.Secrets.v1.SecretAccessResponse> Access(global::Nitric.Proto.Secrets.v1.SecretAccessRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for SecretManager</summary>
    public partial class SecretManagerClient : grpc::ClientBase<SecretManagerClient>
    {
      /// <summary>Creates a new client for SecretManager</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public SecretManagerClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for SecretManager that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public SecretManagerClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected SecretManagerClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected SecretManagerClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// Updates a secret, creating a new one if it doesn't already exist
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::Nitric.Proto.Secrets.v1.SecretPutResponse Put(global::Nitric.Proto.Secrets.v1.SecretPutRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Put(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Updates a secret, creating a new one if it doesn't already exist
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::Nitric.Proto.Secrets.v1.SecretPutResponse Put(global::Nitric.Proto.Secrets.v1.SecretPutRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Put, null, options, request);
      }
      /// <summary>
      /// Updates a secret, creating a new one if it doesn't already exist
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::Nitric.Proto.Secrets.v1.SecretPutResponse> PutAsync(global::Nitric.Proto.Secrets.v1.SecretPutRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PutAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Updates a secret, creating a new one if it doesn't already exist
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::Nitric.Proto.Secrets.v1.SecretPutResponse> PutAsync(global::Nitric.Proto.Secrets.v1.SecretPutRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Put, null, options, request);
      }
      /// <summary>
      /// Gets a secret from a Secret Store
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::Nitric.Proto.Secrets.v1.SecretAccessResponse Access(global::Nitric.Proto.Secrets.v1.SecretAccessRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Access(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Gets a secret from a Secret Store
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::Nitric.Proto.Secrets.v1.SecretAccessResponse Access(global::Nitric.Proto.Secrets.v1.SecretAccessRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Access, null, options, request);
      }
      /// <summary>
      /// Gets a secret from a Secret Store
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::Nitric.Proto.Secrets.v1.SecretAccessResponse> AccessAsync(global::Nitric.Proto.Secrets.v1.SecretAccessRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return AccessAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Gets a secret from a Secret Store
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::Nitric.Proto.Secrets.v1.SecretAccessResponse> AccessAsync(global::Nitric.Proto.Secrets.v1.SecretAccessRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Access, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override SecretManagerClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new SecretManagerClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(SecretManagerBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Put, serviceImpl.Put)
          .AddMethod(__Method_Access, serviceImpl.Access).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, SecretManagerBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Put, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Nitric.Proto.Secrets.v1.SecretPutRequest, global::Nitric.Proto.Secrets.v1.SecretPutResponse>(serviceImpl.Put));
      serviceBinder.AddMethod(__Method_Access, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Nitric.Proto.Secrets.v1.SecretAccessRequest, global::Nitric.Proto.Secrets.v1.SecretAccessResponse>(serviceImpl.Access));
    }

  }
}
#endregion

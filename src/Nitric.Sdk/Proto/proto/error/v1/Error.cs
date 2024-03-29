// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: proto/error/v1/error.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Nitric.Proto.Error.v1 {

  /// <summary>Holder for reflection information generated from proto/error/v1/error.proto</summary>
  public static partial class ErrorReflection {

    #region Descriptor
    /// <summary>File descriptor for proto/error/v1/error.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ErrorReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Chpwcm90by9lcnJvci92MS9lcnJvci5wcm90bxIPbml0cmljLmVycm9yLnYx",
            "Io8BCgpFcnJvclNjb3BlEg8KB3NlcnZpY2UYASABKAkSDgoGcGx1Z2luGAIg",
            "ASgJEjMKBGFyZ3MYAyADKAsyJS5uaXRyaWMuZXJyb3IudjEuRXJyb3JTY29w",
            "ZS5BcmdzRW50cnkaKwoJQXJnc0VudHJ5EgsKA2tleRgBIAEoCRINCgV2YWx1",
            "ZRgCIAEoCToCOAEiWgoMRXJyb3JEZXRhaWxzEg8KB21lc3NhZ2UYASABKAkS",
            "DQoFY2F1c2UYAiABKAkSKgoFc2NvcGUYAyABKAsyGy5uaXRyaWMuZXJyb3Iu",
            "djEuRXJyb3JTY29wZUKJAQoYaW8ubml0cmljLnByb3RvLmVycm9yLnYxQgZF",
            "cnJvcnNQAVozZ2l0aHViLmNvbS9uaXRyaWN0ZWNoL25pdHJpYy9jb3JlL3Br",
            "Zy9hcGkvbml0cmljL3YxqgIVTml0cmljLlByb3RvLkVycm9yLnYxygIVTml0",
            "cmljXFByb3RvXEVycm9yXFYxYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Nitric.Proto.Error.v1.ErrorScope), global::Nitric.Proto.Error.v1.ErrorScope.Parser, new[]{ "Service", "Plugin", "Args" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { null, }),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nitric.Proto.Error.v1.ErrorDetails), global::Nitric.Proto.Error.v1.ErrorDetails.Parser, new[]{ "Message", "Cause", "Scope" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ErrorScope : pb::IMessage<ErrorScope>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ErrorScope> _parser = new pb::MessageParser<ErrorScope>(() => new ErrorScope());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ErrorScope> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nitric.Proto.Error.v1.ErrorReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ErrorScope() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ErrorScope(ErrorScope other) : this() {
      service_ = other.service_;
      plugin_ = other.plugin_;
      args_ = other.args_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ErrorScope Clone() {
      return new ErrorScope(this);
    }

    /// <summary>Field number for the "service" field.</summary>
    public const int ServiceFieldNumber = 1;
    private string service_ = "";
    /// <summary>
    /// The API service invoked, e.g. 'Service.Method'.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Service {
      get { return service_; }
      set {
        service_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "plugin" field.</summary>
    public const int PluginFieldNumber = 2;
    private string plugin_ = "";
    /// <summary>
    /// The plugin method invoked, e.g. 'PluginService.Method'.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Plugin {
      get { return plugin_; }
      set {
        plugin_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "args" field.</summary>
    public const int ArgsFieldNumber = 3;
    private static readonly pbc::MapField<string, string>.Codec _map_args_codec
        = new pbc::MapField<string, string>.Codec(pb::FieldCodec.ForString(10, ""), pb::FieldCodec.ForString(18, ""), 26);
    private readonly pbc::MapField<string, string> args_ = new pbc::MapField<string, string>();
    /// <summary>
    /// The plugin method arguments, ensure only non-sensitive data is specified.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<string, string> Args {
      get { return args_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ErrorScope);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ErrorScope other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Service != other.Service) return false;
      if (Plugin != other.Plugin) return false;
      if (!Args.Equals(other.Args)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Service.Length != 0) hash ^= Service.GetHashCode();
      if (Plugin.Length != 0) hash ^= Plugin.GetHashCode();
      hash ^= Args.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Service.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Service);
      }
      if (Plugin.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Plugin);
      }
      args_.WriteTo(output, _map_args_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Service.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Service);
      }
      if (Plugin.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Plugin);
      }
      args_.WriteTo(ref output, _map_args_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Service.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Service);
      }
      if (Plugin.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Plugin);
      }
      size += args_.CalculateSize(_map_args_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ErrorScope other) {
      if (other == null) {
        return;
      }
      if (other.Service.Length != 0) {
        Service = other.Service;
      }
      if (other.Plugin.Length != 0) {
        Plugin = other.Plugin;
      }
      args_.Add(other.args_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Service = input.ReadString();
            break;
          }
          case 18: {
            Plugin = input.ReadString();
            break;
          }
          case 26: {
            args_.AddEntriesFrom(input, _map_args_codec);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            Service = input.ReadString();
            break;
          }
          case 18: {
            Plugin = input.ReadString();
            break;
          }
          case 26: {
            args_.AddEntriesFrom(ref input, _map_args_codec);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class ErrorDetails : pb::IMessage<ErrorDetails>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ErrorDetails> _parser = new pb::MessageParser<ErrorDetails>(() => new ErrorDetails());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ErrorDetails> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nitric.Proto.Error.v1.ErrorReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ErrorDetails() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ErrorDetails(ErrorDetails other) : this() {
      message_ = other.message_;
      cause_ = other.cause_;
      scope_ = other.scope_ != null ? other.scope_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ErrorDetails Clone() {
      return new ErrorDetails(this);
    }

    /// <summary>Field number for the "message" field.</summary>
    public const int MessageFieldNumber = 1;
    private string message_ = "";
    /// <summary>
    /// The developer error message, explaining the error and ideally solution.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Message {
      get { return message_; }
      set {
        message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "cause" field.</summary>
    public const int CauseFieldNumber = 2;
    private string cause_ = "";
    /// <summary>
    /// The error root cause.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Cause {
      get { return cause_; }
      set {
        cause_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "scope" field.</summary>
    public const int ScopeFieldNumber = 3;
    private global::Nitric.Proto.Error.v1.ErrorScope scope_;
    /// <summary>
    /// The scope of the error.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Nitric.Proto.Error.v1.ErrorScope Scope {
      get { return scope_; }
      set {
        scope_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ErrorDetails);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ErrorDetails other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Message != other.Message) return false;
      if (Cause != other.Cause) return false;
      if (!object.Equals(Scope, other.Scope)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Message.Length != 0) hash ^= Message.GetHashCode();
      if (Cause.Length != 0) hash ^= Cause.GetHashCode();
      if (scope_ != null) hash ^= Scope.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Message.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Message);
      }
      if (Cause.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Cause);
      }
      if (scope_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Scope);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Message.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Message);
      }
      if (Cause.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Cause);
      }
      if (scope_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Scope);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Message.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Message);
      }
      if (Cause.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Cause);
      }
      if (scope_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Scope);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ErrorDetails other) {
      if (other == null) {
        return;
      }
      if (other.Message.Length != 0) {
        Message = other.Message;
      }
      if (other.Cause.Length != 0) {
        Cause = other.Cause;
      }
      if (other.scope_ != null) {
        if (scope_ == null) {
          Scope = new global::Nitric.Proto.Error.v1.ErrorScope();
        }
        Scope.MergeFrom(other.Scope);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Message = input.ReadString();
            break;
          }
          case 18: {
            Cause = input.ReadString();
            break;
          }
          case 26: {
            if (scope_ == null) {
              Scope = new global::Nitric.Proto.Error.v1.ErrorScope();
            }
            input.ReadMessage(Scope);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            Message = input.ReadString();
            break;
          }
          case 18: {
            Cause = input.ReadString();
            break;
          }
          case 26: {
            if (scope_ == null) {
              Scope = new global::Nitric.Proto.Error.v1.ErrorScope();
            }
            input.ReadMessage(Scope);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code

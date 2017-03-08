// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: can_read_barcode.proto
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Org.SilaStandard.V2.RealeaseCandidate.CanReadCode {
  public static class can_read_barcode
  {
    static readonly string __ServiceName = "org.sila_standard.v2.realease_candidate.canReadCode.can_read_barcode";

    static readonly Marshaller<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue> __Marshaller_PhysicalValue = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue.Parser.ParseFrom);
    static readonly Marshaller<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> __Marshaller_String = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String.Parser.ParseFrom);
    static readonly Marshaller<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request> __Marshaller_SiLA_Property_Request = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request.Parser.ParseFrom);
    static readonly Marshaller<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean> __Marshaller_Boolean = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean.Parser.ParseFrom);

    static readonly Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> __Method_read_code = new Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String>(
        MethodType.ServerStreaming,
        __ServiceName,
        "read_code",
        __Marshaller_PhysicalValue,
        __Marshaller_String);

    static readonly Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> __Method_read_code_noexp = new Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String>(
        MethodType.ServerStreaming,
        __ServiceName,
        "read_code_noexp",
        __Marshaller_PhysicalValue,
        __Marshaller_String);

    static readonly Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> __Method_barcode = new Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String>(
        MethodType.ServerStreaming,
        __ServiceName,
        "barcode",
        __Marshaller_SiLA_Property_Request,
        __Marshaller_String);

    static readonly Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean> __Method_is_barcode_found = new Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request, global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean>(
        MethodType.ServerStreaming,
        __ServiceName,
        "is_barcode_found",
        __Marshaller_SiLA_Property_Request,
        __Marshaller_Boolean);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Org.SilaStandard.V2.RealeaseCandidate.CanReadCode.CanReadBarcodeReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of can_read_barcode</summary>
    public abstract class can_read_barcodeBase
    {
      /// <summary>
      ///  returns a scanned Barcode
      /// </summary>
      public virtual global::System.Threading.Tasks.Task read_code(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue request, IServerStreamWriter<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> responseStream, ServerCallContext context)
      {
        throw new RpcException(new Status(StatusCode.Unimplemented, ""));
      }

      /// <summary>
      ///  returns a scanned Barcode without exception
      /// </summary>
      public virtual global::System.Threading.Tasks.Task read_code_noexp(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue request, IServerStreamWriter<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> responseStream, ServerCallContext context)
      {
        throw new RpcException(new Status(StatusCode.Unimplemented, ""));
      }

      /// <summary>
      /// retunrs the property barcode
      /// </summary>
      public virtual global::System.Threading.Tasks.Task barcode(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request request, IServerStreamWriter<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> responseStream, ServerCallContext context)
      {
        throw new RpcException(new Status(StatusCode.Unimplemented, ""));
      }

      /// <summary>
      ///   
      /// </summary>
      public virtual global::System.Threading.Tasks.Task is_barcode_found(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request request, IServerStreamWriter<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean> responseStream, ServerCallContext context)
      {
        throw new RpcException(new Status(StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for can_read_barcode</summary>
    public class can_read_barcodeClient : ClientBase<can_read_barcodeClient>
    {
      /// <summary>Creates a new client for can_read_barcode</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public can_read_barcodeClient(Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for can_read_barcode that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public can_read_barcodeClient(CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected can_read_barcodeClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected can_read_barcodeClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      ///  returns a scanned Barcode
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> read_code(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return read_code(request, new CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///  returns a scanned Barcode
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> read_code(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue request, CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_read_code, null, options, request);
      }
      /// <summary>
      ///  returns a scanned Barcode without exception
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> read_code_noexp(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return read_code_noexp(request, new CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///  returns a scanned Barcode without exception
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> read_code_noexp(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue request, CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_read_code_noexp, null, options, request);
      }
      /// <summary>
      /// retunrs the property barcode
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> barcode(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return barcode(request, new CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// retunrs the property barcode
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> barcode(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request request, CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_barcode, null, options, request);
      }
      /// <summary>
      ///   
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean> is_barcode_found(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return is_barcode_found(request, new CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///   
      /// </summary>
      public virtual AsyncServerStreamingCall<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean> is_barcode_found(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request request, CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_is_barcode_found, null, options, request);
      }
      protected override can_read_barcodeClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new can_read_barcodeClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    public static ServerServiceDefinition BindService(can_read_barcodeBase serviceImpl)
    {
      return ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_read_code, serviceImpl.read_code)
          .AddMethod(__Method_read_code_noexp, serviceImpl.read_code_noexp)
          .AddMethod(__Method_barcode, serviceImpl.barcode)
          .AddMethod(__Method_is_barcode_found, serviceImpl.is_barcode_found).Build();
    }

  }
}
#endregion
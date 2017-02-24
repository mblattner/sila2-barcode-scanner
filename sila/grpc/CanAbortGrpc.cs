// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: can_abort.proto
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Org.SilaStandard.V2.RealeaseCandidate.CanAbort {
  /// <summary>
  ///  Interface exported by the server.
  /// </summary>
  public static class abort
  {
    static readonly string __ServiceName = "org.sila_standard.v2.realease_candidate.canAbort.abort";

    static readonly Marshaller<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty> __Marshaller_Empty = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty.Parser.ParseFrom);
    static readonly Marshaller<global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply> __Marshaller_abortReply = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply.Parser.ParseFrom);

    static readonly Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty, global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply> __Method_abort = new Method<global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty, global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply>(
        MethodType.Unary,
        __ServiceName,
        "abort",
        __Marshaller_Empty,
        __Marshaller_abortReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.CanAbortReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of abort</summary>
    public abstract class abortBase
    {
      /// <summary>
      ///  A simple RPC.
      ///   
      /// </summary>
      public virtual global::System.Threading.Tasks.Task<global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply> abort(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty request, ServerCallContext context)
      {
        throw new RpcException(new Status(StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for abort</summary>
    public class abortClient : ClientBase<abortClient>
    {
      /// <summary>Creates a new client for abort</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public abortClient(Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for abort that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public abortClient(CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected abortClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected abortClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      ///  A simple RPC.
      ///   
      /// </summary>
      public virtual global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply abort(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return abort(request, new CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///  A simple RPC.
      ///   
      /// </summary>
      public virtual global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply abort(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty request, CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_abort, null, options, request);
      }
      /// <summary>
      ///  A simple RPC.
      ///   
      /// </summary>
      public virtual AsyncUnaryCall<global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply> abortAsync(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return abortAsync(request, new CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///  A simple RPC.
      ///   
      /// </summary>
      public virtual AsyncUnaryCall<global::Org.SilaStandard.V2.RealeaseCandidate.CanAbort.abortReply> abortAsync(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty request, CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_abort, null, options, request);
      }
      protected override abortClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new abortClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    public static ServerServiceDefinition BindService(abortBase serviceImpl)
    {
      return ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_abort, serviceImpl.abort).Build();
    }

  }
}
#endregion

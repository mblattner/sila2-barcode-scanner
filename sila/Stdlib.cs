// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: stdlib.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Org.SilaStandard.V2.RealeaseCandidate.Stdlib {

  /// <summary>Holder for reflection information generated from stdlib.proto</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class StdlibReflection {

    #region Descriptor
    /// <summary>File descriptor for stdlib.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static StdlibReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxzdGRsaWIucHJvdG8SLm9yZy5zaWxhX3N0YW5kYXJkLnYyLnJlYWxlYXNl",
            "X2NhbmRpZGF0ZS5zdGRsaWIiBwoFRW1wdHkiFgoGU3RyaW5nEgwKBGRhdGEY",
            "ASABKAkiFgoGRG91YmxlEgwKBGRhdGEYASABKAFiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty.Parser, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String.Parser, new[]{ "Data" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Double), global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Double.Parser, new[]{ "Data" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  ///  Empty Message
  ///  Used for Properties
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class Empty : pb::IMessage<Empty> {
    private static readonly pb::MessageParser<Empty> _parser = new pb::MessageParser<Empty>(() => new Empty());
    public static pb::MessageParser<Empty> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.StdlibReflection.Descriptor.MessageTypes[0]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public Empty() {
      OnConstruction();
    }

    partial void OnConstruction();

    public Empty(Empty other) : this() {
    }

    public Empty Clone() {
      return new Empty(this);
    }

    public override bool Equals(object other) {
      return Equals(other as Empty);
    }

    public bool Equals(Empty other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
    }

    public int CalculateSize() {
      int size = 0;
      return size;
    }

    public void MergeFrom(Empty other) {
      if (other == null) {
        return;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  /// <summary>
  ///  String Message
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class String : pb::IMessage<String> {
    private static readonly pb::MessageParser<String> _parser = new pb::MessageParser<String>(() => new String());
    public static pb::MessageParser<String> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.StdlibReflection.Descriptor.MessageTypes[1]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public String() {
      OnConstruction();
    }

    partial void OnConstruction();

    public String(String other) : this() {
      data_ = other.data_;
    }

    public String Clone() {
      return new String(this);
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 1;
    private string data_ = "";
    public string Data {
      get { return data_; }
      set {
        data_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    public override bool Equals(object other) {
      return Equals(other as String);
    }

    public bool Equals(String other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Data != other.Data) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Data.Length != 0) hash ^= Data.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Data.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Data);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Data.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Data);
      }
      return size;
    }

    public void MergeFrom(String other) {
      if (other == null) {
        return;
      }
      if (other.Data.Length != 0) {
        Data = other.Data;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Data = input.ReadString();
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class Double : pb::IMessage<Double> {
    private static readonly pb::MessageParser<Double> _parser = new pb::MessageParser<Double>(() => new Double());
    public static pb::MessageParser<Double> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Org.SilaStandard.V2.RealeaseCandidate.Stdlib.StdlibReflection.Descriptor.MessageTypes[2]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public Double() {
      OnConstruction();
    }

    partial void OnConstruction();

    public Double(Double other) : this() {
      data_ = other.data_;
    }

    public Double Clone() {
      return new Double(this);
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 1;
    private double data_;
    public double Data {
      get { return data_; }
      set {
        data_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as Double);
    }

    public bool Equals(Double other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Data != other.Data) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Data != 0D) hash ^= Data.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Data != 0D) {
        output.WriteRawTag(9);
        output.WriteDouble(Data);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Data != 0D) {
        size += 1 + 8;
      }
      return size;
    }

    public void MergeFrom(Double other) {
      if (other == null) {
        return;
      }
      if (other.Data != 0D) {
        Data = other.Data;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 9: {
            Data = input.ReadDouble();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
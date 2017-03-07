using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Org.SilaStandard.V2.RealeaseCandidate.CanReadCode;
using Org.SilaStandard.V2.RealeaseCandidate.IsSila;

namespace BarcodeReaderGetProperty
{
    class BarcodeReaderGetProperty
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new is_sila.is_silaClient(channel);
            try
            {
                Metadata mdata = new Metadata();
                //var reply = client.isSiLAService(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty(), mdata);
                DeviveIdentification devInfo = client.device_identification(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Void());
                Console.WriteLine("Name: " + devInfo.DeviceName);
                Console.WriteLine("SerialNumber: " + devInfo.DeviceSerialNumber);
                Console.WriteLine("Manufacturer: " + devInfo.DeviceManufacturer);
                Console.WriteLine("Firmware-Version: " + devInfo.DeviceFirmwareVersion);
            }
            catch (RpcException e)
            {
                if (e.Status.StatusCode == StatusCode.Unavailable)
                {
                    Console.WriteLine("Can't connect to device!");
                    System.Environment.Exit(-1);
                }
                Console.WriteLine("Hallo was ist denn jetzt passiert: " + e.ToString());
                Console.WriteLine("Exception Message: " + e.Message);
            }

            //now reading the barcode Propety:
            Console.WriteLine("NonSerializedAttribute reading continously");
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var client2 = new can_read_barcode.can_read_barcodeClient(channel);
            var barcodeCall = client2.barcode(
                new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request
                {
                    Frequency = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue { Value = 3000 },
                    //Frequency = null, // 
                    Threshold = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue { Value = 2 }
                }, null, null, tokenSource.Token);

            var isBarcodeFoundCall = client2.is_barcode_found(
                new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request
                {
                    Frequency = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue { Value = 3000 },
                    //Frequency = null, // 
                    Threshold = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue { Value = 2 }
                }, null, null, tokenSource.Token);


            Task rt = new Task(() =>
            {
                try
                {
                    readingBarcodeStream(barcodeCall, token).Wait();
                }
                catch (AggregateException e)
                {
                    Console.WriteLine("Exception messages:");
                    foreach (var ie in e.InnerExceptions)
                        Console.WriteLine("   {0}: {1}", ie.GetType().Name, ie.Message);
                }
            }, tokenSource.Token);
            rt.Start();

            Task isBCFoundTask = new Task(() =>
            {
                try
                {
                    readingisBarcodeFoundStream(isBarcodeFoundCall, token).Wait();
                }
                catch (AggregateException e)
                {
                    Console.WriteLine("Exception messages:");
                    foreach (var ie in e.InnerExceptions)
                        Console.WriteLine("   {0}: {1}", ie.GetType().Name, ie.Message);
                }
            }, tokenSource.Token);
            isBCFoundTask.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            tokenSource.Cancel();
            try
            {
                rt.Wait();
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Exception messages:");
                foreach (var ie in e.InnerExceptions)
                    Console.WriteLine("   {0}: {1}", ie.GetType().Name, ie.Message);

                Console.WriteLine("\nTask status: {0}", rt.Status);
            }

            try
            {
                channel.ShutdownAsync().Wait();
            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static async Task readingisBarcodeFoundStream(AsyncServerStreamingCall<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean> isBarcodeFoundCall, CancellationToken ct)
        {
            while (await isBarcodeFoundCall.ResponseStream.MoveNext())
            {
                Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean found = isBarcodeFoundCall.ResponseStream.Current;
                Console.WriteLine("is barcode found? " + found.Value);
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("Task was cancelled");
                    ct.ThrowIfCancellationRequested();
                }
            }

        }

        static async Task readingBarcodeStream(AsyncServerStreamingCall<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> barcodeCall, CancellationToken ct)
        {
            while (await barcodeCall.ResponseStream.MoveNext())
            {
                Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String barcode = barcodeCall.ResponseStream.Current;
                Console.WriteLine("reading barcode Property: " + barcode.Value);
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("Task was cancelled");
                    ct.ThrowIfCancellationRequested();
                }
            }
            
        }
    }
}

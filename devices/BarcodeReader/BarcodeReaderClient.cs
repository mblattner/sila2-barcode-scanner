using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Org.SilaStandard.V2.RealeaseCandidate.CanReadCode;
using Org.SilaStandard.V2.RealeaseCandidate.IsSila;

namespace BarcodeSila2Client
{
    class Program
    {
        public static void Main(string[] args)
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
                    Console.WriteLine("Can't connect to device!" );
                    System.Environment.Exit(-1);
                }
                Console.WriteLine("Hallo was ist denn jetzt passiert: " + e.ToString());
                Console.WriteLine("Exception Message: " + e.Message);
            }

            /*var configurationClient = new canBarcodeReaderConfiguration.canBarcodeReaderConfigurationClient(channel);
            try
            {
                BarcodeReaderConfiguration reply = configurationClient.getConfiguration(new Org.SilaStandard.V2.RealeaseCandidate.CanSetConfiguration.Empty());
                reply.PortName = "COM1";
                var status = configurationClient.setConfiguration(reply);
            } catch (RpcException e)
            {
                if (e.Status.StatusCode == StatusCode.Unavailable)
                {
                    Console.WriteLine("Can't connect to device!");
                    System.Environment.Exit(-1);
                }
                Console.WriteLine("Hallo was ist denn jetzt passiert: " + e.ToString());
                Console.WriteLine("Exception Message: " + e.Message);
            }
            */
            var client2 = new can_read_barcode.can_read_barcodeClient(channel);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            
            Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue timeout = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue
                {
                Unit = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Unit { Value = "s" },
                Value = 5 
                };
            
            var is_bc_found_call = client2.is_barcode_found(
                new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request
                {
                    
                });

            Task is_bc = new Task(async () => {
            while (await is_bc_found_call.ResponseStream.MoveNext())
            {
                Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean found = is_bc_found_call.ResponseStream.Current;
                Console.WriteLine("is Barcode scanned? " + found.Value);
            }});

            is_bc.Start();
            
            Task scanTask = new Task(async () =>
                {
                    AsyncServerStreamingCall<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> readCodeCall=null;
                    while (!token.IsCancellationRequested)
                    {
                        readCodeCall = client2.read_code_noexp(timeout, null, null, token);
                        while (await readCodeCall.ResponseStream.MoveNext())
                        {
                            Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String response = readCodeCall.ResponseStream.Current;
                            if (response.Metadata.Progress == 100)
                            {
                                Console.WriteLine("scanned barcode: " + response.Value);
                            }
                            else
                            {
                                Console.WriteLine("Progess: " + response.Metadata.Progress);
                            }
                            if (token.IsCancellationRequested)
                            {
                                Console.WriteLine("Task was cancelled");
                                token.ThrowIfCancellationRequested();
                            }
                        }
                        Thread.Sleep(500);
                    }
                }, tokenSource.Token);
            scanTask.Start();


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            tokenSource.Cancel();
            scanTask.Wait();

            /*
            Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String scanner = null;
            for (int i = 0; i <= 10; i++)
            {
                try
                {
                    scanner = client2.read_code(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Void());
                    Console.WriteLine(scanner.Value);
                }
                catch (RpcException e)
                {
                    if (e.Status.StatusCode == StatusCode.Aborted)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("waiting ...");
                        Task.Delay(1000);
                    }
                }
            }
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Console.WriteLine("Reading Property");
            using (var barcodeCall1 = client2.barcode(
                new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request {
                    Frequency = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue { Value = 0 },
                    Threshold = null}
                    )
                  )
            {
                readingBarcodeStream(barcodeCall1, token).Wait();
            }

            Console.WriteLine("NonSerializedAttribute reading continously");
            var barcodeCall = client2.barcode(
                new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.SiLA_Property_Request
                {
                    Frequency = new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.PhysicalValue { Value = 2000 },
                    Threshold = null
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

            for (int j = 0; j<=2; j++) {
                Console.WriteLine("sleeping...." + j);
                Thread.Sleep(2000);
            }

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
            */
            
            /*
            //var client3 = new canReadCodeContinuously.canReadCodeContinuouslyClient(channel);
            var client3 = new readCodeContinuously.readCodeContinuouslyClient(channel);
            readCodeContinuously(client3).Wait();


            var client4 = new canReadCodeContinuously.canReadCodeContinuouslyClient(channel);
            readCodeContinuously(client4).Wait();
            */
            try
            {
                channel.ShutdownAsync().Wait();
            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        static async Task readingBarcodeStream(AsyncServerStreamingCall<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> barcodeCall, CancellationToken ct)
        {
            while (await barcodeCall.ResponseStream.MoveNext() )
            {
                Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String barcode = barcodeCall.ResponseStream.Current;
                Console.WriteLine("reading barcode Property: " + barcode.Value);
                if (ct.IsCancellationRequested) {
                    Console.WriteLine("Task was cancelled");
                    ct.ThrowIfCancellationRequested();
                }
            }
            /*if (ct.IsCancellationRequested)
            {
                //barcodeCall.ResponseStream.Dispose();
                Console.WriteLine("Task was cancelled");
                ct.ThrowIfCancellationRequested();
            }*/

        }
        


        //static public async Task readCodeContinuously(can_read_barcode.can_read_barcodeClient client)
        //{
        //    int i = 0;
        //    bool retry = true;
        //    AsyncServerStreamingCall<ScannerResponse> scannerStream = null;

        //    while (retry)
        //    {
        //        scannerStream = client.startReading(new ScannerRequest { Id = i.ToString() });
        //        retry = false;
        //        try
        //        {
        //            while (await scannerStream.ResponseStream.MoveNext())
        //            {
        //                Console.WriteLine((i++).ToString() + ": " + scannerStream.ResponseStream.Current.Barocde);
        //                if (i == 10)
        //                    break;
        //            }
        //        }
        //        catch (RpcException e)
        //        {
        //            if (e.Status.StatusCode == StatusCode.Internal)
        //            {
        //                retry = true;
        //            }
        //        }
        //    }
        //    var response = client.stopReading(new ScannerRequest { Id = i.ToString() });
        //    Console.WriteLine((i++).ToString() + ": " + response.Barocde);
        //    //somit liest man, die METADATA, die vom Server geschickt werden, aus

        //    while (await scannerStream.ResponseStream.MoveNext())
        //        ;
        //    Metadata responseTrailers = scannerStream.GetTrailers();
        //    Console.WriteLine(responseTrailers);
        //}

        //static public async Task readCodeContinuously(read_CodeContinuously.read_CodeContinuouslyClient client)
        //{
        //    int i = 0;
        //    bool retry = true;
        //    AsyncDuplexStreamingCall<ScannerContinousRequest, ScannerResponse> scannerStream = null;

        //    while (retry)
        //    {
        //        using (scannerStream = client.openReader())
        //        {
        //            await scannerStream.RequestStream.WriteAsync(new ScannerContinousRequest { Id = (i++).ToString(), Command = "START" });
        //            retry = false;
        //            try
        //            {
        //                while (await scannerStream.ResponseStream.MoveNext())
        //                {
        //                    Console.WriteLine((i++).ToString() + ": " + scannerStream.ResponseStream.Current.Barocde);
        //                    if (i == 10)
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //            catch (RpcException e)
        //            {
        //                if (e.Status.StatusCode == StatusCode.Internal)
        //                {
        //                    retry = true;
        //                }
        //                else
        //                {
        //                    throw e;
        //                }
        //            }
        //        }
        //        try
        //        {
        //            await scannerStream.RequestStream.WriteAsync(new ScannerContinousRequest { Id = (i++).ToString(), Command = "STOP" });
        //            await scannerStream.RequestStream.CompleteAsync();

        //            while (await scannerStream.ResponseStream.MoveNext())
        //            {
        //                Console.WriteLine((i++).ToString() + ": " + scannerStream.ResponseStream.Current.Barocde);
        //                if (i == 10)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            System.String err = string.Format("Main.Finally: Exception: {0} :  {1}", e.Source, e.Message);
        //            Console.WriteLine(err);
        //        }
        //    }
        //}
    }
}

using System;
using Grpc.Core;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Org.SilaStandard.V2.RealeaseCandidate.IsSila;
using Org.SilaStandard.V2.RealeaseCandidate.CanReadCode;
using Org.SilaStandard.V2.RealeaseCandidate.CanSetConfiguration;

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

                Console.WriteLine("Name: " + client.service_name(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty()));
                Console.WriteLine("SerialNumber: " + client.device_serial_number(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty()));
                Console.WriteLine("Manufacturer: " + client.device_manufacturer(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty()));
                Console.WriteLine("Firmware-Version: " + client.device_firmware_version(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty()));
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

            var configurationClient = new canBarcodeReaderConfiguration.canBarcodeReaderConfigurationClient(channel);
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

            var client2 = new can_ReadCode.can_ReadCodeClient(channel);
            
            ScannerResponse scanner = null;

            for (int i = 0; i <= 10; i++)
            {

                try
                {
                    scanner = client2.read_Code(new ScannerRequest {EmptyBarocdeString="no BC deteced",  Id = i.ToString() });
                    Console.WriteLine(scanner.Barocde);
                } catch (RpcException e)
                {
                    if (e.Status.StatusCode == StatusCode.Internal)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("waiting ...");
                        Task.Delay(1000);
                    } else
                    {
                        Console.WriteLine(e.Message);
                    }
                    
                } 
            }


            for (int i = 0; i <= 10; i++)
            {

                try
                {
                    scanner = client2.read_CodeWithException(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Empty());
                    Console.WriteLine(scanner.Barocde);
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

            /*
            //var client3 = new canReadCodeContinuously.canReadCodeContinuouslyClient(channel);
            var client3 = new readCodeContinuously.readCodeContinuouslyClient(channel);
            readCodeContinuously(client3).Wait();


            var client4 = new canReadCodeContinuously.canReadCodeContinuouslyClient(channel);
            readCodeContinuously(client4).Wait();
            */
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static public async Task readCodeContinuously(can_ReadCodeContinuously.can_ReadCodeContinuouslyClient client)
        {
            int i = 0;
            bool retry = true;
            AsyncServerStreamingCall<ScannerResponse> scannerStream = null;

            while (retry)
            {
                scannerStream = client.startReading(new ScannerRequest { Id = i.ToString() });
                retry = false;
                try
                {
                    while (await scannerStream.ResponseStream.MoveNext())
                    {
                        Console.WriteLine((i++).ToString() + ": " + scannerStream.ResponseStream.Current.Barocde);
                        if (i == 10)
                            break;
                    }
                }
                catch (RpcException e)
                {
                    if (e.Status.StatusCode == StatusCode.Internal)
                    {
                        retry = true;
                    }
                }
            }
            var response = client.stopReading(new ScannerRequest { Id = i.ToString() });
            Console.WriteLine((i++).ToString() + ": " + response.Barocde);
            //somit liest man, die METADATA, die vom Server geschickt werden, aus

            while (await scannerStream.ResponseStream.MoveNext())
                ;
            Metadata responseTrailers = scannerStream.GetTrailers();
            Console.WriteLine(responseTrailers);
        }
        static public async Task readCodeContinuously(read_CodeContinuously.read_CodeContinuouslyClient client)
        {
            int i = 0;
            bool retry = true;
            AsyncDuplexStreamingCall<ScannerContinousRequest, ScannerResponse> scannerStream = null;

            while (retry)
            {
                using (scannerStream = client.openReader())
                {
                    await scannerStream.RequestStream.WriteAsync(new ScannerContinousRequest { Id = (i++).ToString(), Command = "START" });
                    retry = false;
                    try
                    {
                        while (await scannerStream.ResponseStream.MoveNext())
                        {
                            Console.WriteLine((i++).ToString() + ": " + scannerStream.ResponseStream.Current.Barocde);
                            if (i == 10)
                            {
                                break;
                            }
                        }
                    }
                    catch (RpcException e)
                    {
                        if (e.Status.StatusCode == StatusCode.Internal)
                        {
                            retry = true;
                        }
                        else
                        {
                            throw e;
                        }
                    }
                }
                try
                {
                    await scannerStream.RequestStream.WriteAsync(new ScannerContinousRequest { Id = (i++).ToString(), Command = "STOP" });
                    await scannerStream.RequestStream.CompleteAsync();

                    while (await scannerStream.ResponseStream.MoveNext())
                    {
                        Console.WriteLine((i++).ToString() + ": " + scannerStream.ResponseStream.Current.Barocde);
                        if (i == 10)
                        {
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    System.String err = string.Format("Main.Finally: Exception: {0} :  {1}", e.Source, e.Message);
                    Console.WriteLine(err);
                }


            }
        }
    }
}

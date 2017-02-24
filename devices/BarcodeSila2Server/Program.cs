using Grpc.Core;
using Sila2.Org.SilaStandard.ReleaseCandidate.Common;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using DatalogicMatrix200;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CommonServices.DeviceExceptions;
using static DatalogicMatrix200.ConfigurationDatalogicMatrix200;

namespace BarcodeSila2Sever
{

    /*
    public sealed class ReaderDevice
    {
        private static readonly ReaderDevice instance = new ReaderDevice();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ReaderDevice()
        {
        }

        private ReaderDevice()
        {
        }

        public static ReaderDevice Instance
        {
            get
            {
                return instance;
            }
        }
    }*/

    public sealed class ReaderDevice {
        //private static readonly ReaderDevice instance = new ReaderDevice();

        private static readonly Device device = new Device();
        public static Status DeviceResetStatus = new Status (StatusCode.Internal, "Device Reset");
        static ReaderDevice()
        {
            var t = device.GetType();
            reset();
            setConfig();
            initialize();
            t = device.GetType();
        }

        static public string ReadCode(System.String EmptyBarocdeString=null, int tryRun = 0)
        {
            try
            {
                return device.ReadCode();
            }
            catch (DeviceException deviceException)
            {
                var errorCode = deviceException.ErrorCode;
                System.String err = string.Format("device.ReadCode: deviceException: {0} - Error Code {1}: {2}", deviceException.Source, (int)errorCode, DeviceErrors.GetErrorMessage(errorCode));
                switch (errorCode)
                {
                    case DeviceErrors.ErrorCode.SerialPortResponseError:
                    case DeviceErrors.ErrorCode.SerialPortReadError:
                        Console.WriteLine(System.String.Format("{0} received!", errorCode));
                        reInitialize();
                        return null;
                    case DeviceErrors.ErrorCode.SerialPortNoReadTimeoutError:
                        //wait a bit and retry
                        Console.WriteLine(System.String.Format("{0} received!", errorCode));
                        Console.WriteLine("waiting and retry");
                        Task.Delay(250);
                        if (tryRun > 1)
                        {
                            reInitialize();
                        }
                        else
                        {
                            return ReadCode(EmptyBarocdeString, ++tryRun);
                        }
                        return null;

                    case DeviceErrors.ErrorCode.CodeReaderNoBarcodeDetected:
                        if (!System.String.IsNullOrEmpty(EmptyBarocdeString))
                        {
                            return EmptyBarocdeString;
                        }
                        throw new RpcException(new Status(StatusCode.Aborted, "No Barcode detected"), "No Barcode detected");
                            
                    default:
                        Console.WriteLine(err);
                        return err;
                }
            }
            catch (Exception exception)
            {
                throw new RpcException(new Status(StatusCode.Unknown, exception.Message), exception.Message);                
            }            
        }

        static public void setOneShotRead()
        {
            //Task.Delay(500);
            try
            {
                ParametersDatalogicMatrix200 parameter = device.GetParameters();
                parameter.OperatingMode = ParametersDatalogicMatrix200.EOperatingMode.OneShot;
                device.SetParameters(parameter);
            }
            catch (DeviceException deviceException)
            {
                var errorCode = deviceException.ErrorCode;
                System.String err = string.Format("deviceException: {0} - Error Code {1}: {2}", deviceException.Source, (int)errorCode, DeviceErrors.GetErrorMessage(errorCode));
                /*if (errorCode == DeviceErrors.ErrorCode.SerialPortNoReadTimeoutError)
                {
                    reset();
                    setConfig();
                    initialize();
                    throw new RpcException(new Status(StatusCode.Internal, "Device Reset"), "device Reset  - please wait");
                }*/
                Console.WriteLine(err);
            }
            catch (Exception exception)
            {
                System.String err = string.Format("Exception: {0} - Error Code {1}: {2}", exception.Source, DeviceErrors.ErrorCode.UnknownError, exception.Message);
                Console.WriteLine(err);
            }
            
            //Task.Delay(500);
        }

        static public void setContinousRead()
        {
            try
            {
                ParametersDatalogicMatrix200 parameter = device.GetParameters();
                parameter.OperatingMode = ParametersDatalogicMatrix200.EOperatingMode.Continuous;
                device.SetParameters(parameter);
            }
            catch (DeviceException deviceException)
            {
                var errorCode = deviceException.ErrorCode;
                System.String err = string.Format("deviceException: {0} - Error Code {1}: {2}", deviceException.Source, (int)errorCode, DeviceErrors.GetErrorMessage(errorCode));
                Console.WriteLine(err);
            }
            catch (Exception exception)
            {
                System.String err = string.Format("Exception: {0} - Error Code {1}: {2}", exception.Source, DeviceErrors.ErrorCode.UnknownError, exception.Message);
                Console.WriteLine(err);
            }
        }

        static public void setConfig(ConfigurationDatalogicMatrix200 configParam = null)
        {
            System.String configFileName = @"DatalogicMatrix200Configuration.xml";
            ConfigurationDatalogicMatrix200 param = configParam;
            if (param == null)
            {
                try
                {
                    var reader = new StreamReader(configFileName);

                    var ser = new XmlSerializer(typeof(ConfigurationDatalogicMatrix200));
                    var xmlReader = new XmlTextReader(reader);
                    param = (ConfigurationDatalogicMatrix200)ser.Deserialize(xmlReader);
                    xmlReader.Close();
                    reader.Close();
                } catch (Exception e)
                {
                    
                }
            } else
            {
                ConfigurationDatalogicMatrix200 oldConfig = device.GetConfiguration();
                if (!( configParam.BaudRate.Equals(oldConfig.BaudRate) 
                    && configParam.ConnectionType.Equals(oldConfig.ConnectionType)
                    && configParam.DeviceAddress.Equals(oldConfig.DeviceAddress)
                    && configParam.PortName.Equals(oldConfig.PortName))
                    )
                {
                    var writer = new StreamWriter(configFileName);
                    var ser = new XmlSerializer(typeof(ConfigurationDatalogicMatrix200));
                    var xmlWriter = new XmlTextWriter(writer);
                    ser.Serialize(xmlWriter, param);
                    xmlWriter.Close();
                    writer.Close();
                }
            }
            ReaderDevice.device.SetConfiguration((ConfigurationDatalogicMatrix200)param);
        }


        static public ConfigurationDatalogicMatrix200 getConfig()
        {
            return device.GetConfiguration();
        }



        static public void reset()
        {
            device.Reset();
            /*setConfig();
            initialize();
            */
        }

        static public void initialize()
        {
            try
            {
                device.Initialize();
            }
            catch (DeviceException e)
            {
                System.String err = string.Format("Main.Finally: Exception: {0} - Error Code {1}: {2}", e.Source, DeviceErrors.ErrorCode.UnknownError, e.Message);
                Console.WriteLine(err);
            }
        }
        static public void reInitialize()
        {            
            Console.WriteLine("reInitialize reader");
            reset();
            setConfig();
            initialize();
            Console.WriteLine("throwing rpcException");
            throw new RpcException(DeviceResetStatus, "device Reset  - please wait");
        }
        
    }
    static class AbortScanning
    {
        public static bool abort = false;         
    }

    class isSilaBaseImpl : isSila.isSilaBase
    {
        
        public override Task<isSilaReply> SiLAService(Empty request, ServerCallContext context)
        {
            Status nixGemacht = new Status(StatusCode.PermissionDenied, "Hab noch nix gemacht hier.");
            //throw new RpcException(nixGemacht, "JA aber Hallo");
            //Console.WriteLine("got Request: " + request.Id);
            //throw new Exception("Mist");
            return Task.FromResult(new isSilaReply { Manufacturer = "Datalogic", Model = "matrix200", SerialNumber = "N2468", ManufacturerUrl = "www.datalogic.com" });            
        }
    }

    class readCodeContinuouslyImpl : readCodeContinuously.readCodeContinuouslyBase
    {
        public override async Task openReader(IAsyncStreamReader<ScannerContinousRequest> requestStream, IServerStreamWriter<ScannerResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("here is readCodeContinuously" );
            AbortScanning.abort = false;            
            int i = 0;

            Task readRequestTask = Task.Run(async () =>
                {
                    while (await requestStream.MoveNext())
                    {
                        Console.WriteLine("got Request readCodeContinuously: " + requestStream.Current.Id);
                        Console.WriteLine("command: " + requestStream.Current.Command);
                        if (requestStream.Current.Command.Equals("START"))
                        {
                            ReaderDevice.setContinousRead();
                            Task t = Task.Run(async () =>
                            {
                                System.String barcode = "";
                                while (!AbortScanning.abort)
                                {
                                    /*try
                                    {*/
                                        barcode = ReaderDevice.ReadCode();
                                        await responseStream.WriteAsync(new ScannerResponse { Barocde = barcode });
                                        //await Task.Delay(1000);
                                    /*}
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("openReader exception caught");
                                    }*/

                                }
                                Console.WriteLine("abort received!");
                            });
                        }
                        if (requestStream.Current.Command.Equals("STOP"))
                        {
                            AbortScanning.abort = true;
                            //ReaderDevice.setOneShotRead();
                            Console.WriteLine("STOP received from the client!");
                            
                            //await responseStream.WriteAsync(new ScannerResponse { Barocde = System.String.Format("MB{0, 8 :d8}", i++) });
                            
                        }
                    }
                }
            );
            
            while (!AbortScanning.abort)
            {
                await Task.Delay(1000);
                if (readRequestTask.Exception != null)
                {
                    throw readRequestTask.Exception.InnerException;
                }
            }

            ReaderDevice.setOneShotRead();
            Console.WriteLine("end of request channel!");
        }
    }

    class canReadCodeContinuouslyImpl : canReadCodeContinuously.canReadCodeContinuouslyBase
    {
        public override async Task startReading(ScannerRequest request, IServerStreamWriter<ScannerResponse> responseStream, ServerCallContext context)
        {
            ReaderDevice.setContinousRead();
             
            AbortScanning.abort = false;
            context.Status = Status.DefaultSuccess;
            context.ResponseTrailers.Add(new Metadata.Entry("Erorr", "None"));

            Console.WriteLine("got Request startReading: " + request.Id);
            //System.String barcode = "MB000";
            Task t = Task.Run(async () => {
                int i = 0;
                System.String barcode;
                while (!AbortScanning.abort)
                {
                    /*try
                    {*/
                        barcode = ReaderDevice.ReadCode();
                        //await responseStream.WriteAsync(new ScannerResponse { Barocde = System.String.Format("MB{0, 8 :d8}", i++) });
                        await responseStream.WriteAsync(new ScannerResponse { Barocde = barcode });
                        //await Task.Delay(1000);
                    /*}
                    catch (Exception e)
                    {
                        Console.WriteLine("startReading Excetpion caught");
                    }*/
                    
                }
                             
                //ReaderDevice.setConfig();
                //ReaderDevice.initialize();
                //ReaderDevice.reset();
                context.ResponseTrailers.Add(new Metadata.Entry("Ready", "But of course not finished"));
            });
            
            while (!AbortScanning.abort)
            {
                await Task.Delay(1000);
                if (t.Exception != null)
                {
                    throw t.Exception.InnerException;
                }
            }
            ReaderDevice.setOneShotRead();
        }
        public override Task<ScannerResponse> stopReading(ScannerRequest request, ServerCallContext context)
        {
            AbortScanning.abort = true;            
            Console.WriteLine("got Request stopReading: " + request.Id);
            return Task.FromResult(new ScannerResponse { Barocde = "Stopping..."});
        }
    }

    class canReadCodeImpl : canReadCode.canReadCodeBase
    {
        public override Task<ScannerResponse> readCode(ScannerRequest request, ServerCallContext context)
        {
            System.String barcode;
            Console.WriteLine("got Request: " + request.Id);
            /*try
            {*/
                barcode = ReaderDevice.ReadCode(request.EmptyBarocdeString);
                return Task.FromResult(new ScannerResponse { Barocde = barcode });
            /*} catch (RpcException e)
            {
                throw new RpcException(e.Status, e.Message);
            }*/
        }
        public override Task<ScannerResponse> readCodeWithException(Empty request, ServerCallContext context)
        {
            System.String barcode;
            Console.WriteLine("got readCodeWithException Request: ");
            barcode = ReaderDevice.ReadCode();
            return Task.FromResult(new ScannerResponse { Barocde = barcode });
        }
    }

    class abortImpl : abort.abortBase
    {
        public override Task<abortReply> abort(Empty request, ServerCallContext context)
        {
            AbortScanning.abort = true;
            return Task.FromResult(new abortReply { Status = "Abort Request Received" });            
        }
    }

    class canBarocdeReaderConfigurationImpl : canBarcodeReaderConfiguration.canBarcodeReaderConfigurationBase
    {
        public override Task<status> setConfiguration(BarcodeReaderConfiguration request, ServerCallContext context)
        {
            ConfigurationDatalogicMatrix200 param = new ConfigurationDatalogicMatrix200();
            param.BaudRate = (EBaudRate)  Enum.Parse(typeof(EBaudRate), request.BaudRate);
            param.ConnectionType = (EConnectionType) Enum.Parse(typeof(EConnectionType), request.ConnetionType);
            param.DeviceAddress = request.DeviceAddress;
            param.PortName = request.PortName;

            ReaderDevice.setConfig(param);
            try
            {
                ReaderDevice.reInitialize();
            } catch (RpcException e)
            {
                if (e.Status.Equals( ReaderDevice.DeviceResetStatus))
                {
                    //do nothing;
                }
            }
            return Task.FromResult(new status { Value="OK"});
        }
        public override Task<BarcodeReaderConfiguration> getConfiguration(Empty request, ServerCallContext context)
        {
            ConfigurationDatalogicMatrix200 param = ReaderDevice.getConfig() ;            
            return Task.FromResult(new BarcodeReaderConfiguration { BaudRate = param.BaudRate.ToString() , ConnetionType= param.ConnectionType.ToString(), DeviceAddress = param.DeviceAddress, PortName = param.PortName });
         
        }
    }

    class Program
    {
        const int Port = 50051;        
                
        static void Main(string[] args)
        {   try
            {
                //try
                //{
                    ReaderDevice device = new ReaderDevice();
                //}
                //catch 

                Server server = new Server
                {
                    Services = {
                    isSila.BindService(new isSilaBaseImpl())
                    , canReadCode.BindService(new canReadCodeImpl())
                    , canReadCodeContinuously.BindService(new canReadCodeContinuouslyImpl())
                    , abort.BindService(new abortImpl())
                    , readCodeContinuously.BindService (new readCodeContinuouslyImpl())
                    , canBarcodeReaderConfiguration.BindService( new canBarocdeReaderConfigurationImpl())
                },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };
                server.Start();

                Console.WriteLine("BarcodeScanner listening on port " + Port);
                Console.WriteLine("Press any key to stop the server...");
                Console.ReadKey();                

                server.ShutdownAsync().Wait();
            }
            catch (System.TypeInitializationException e)
            {
                Console.WriteLine("Could not inialize the device");
            }
            finally
            {
                try
                {
                    ReaderDevice.setOneShotRead();
                }
                catch (Exception e)
                {
                    System.String err = string.Format("Main.Finally: Exception: {0} - Error Code {1}: {2}", e.Source, DeviceErrors.ErrorCode.UnknownError, e.Message);
                    Console.WriteLine(err);

                }
                /*ReaderDevice.reset();
                ReaderDevice.setConfig();
                ReaderDevice.initialize();
                */
            }
        }
    }
}

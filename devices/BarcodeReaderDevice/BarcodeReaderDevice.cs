using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatalogicMatrix200;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CommonServices.DeviceExceptions;
using Org.SilaStandard.V2.RealeaseCandidate.Stdlib;
using Org.SilaStandard.V2.RealeaseCandidate.CanAbort;
using Org.SilaStandard.V2.RealeaseCandidate.IsSila;
using Org.SilaStandard.V2.RealeaseCandidate.CanReadCode;
using Org.SilaStandard.V2.RealeaseCandidate.CanSetConfiguration;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace BarcodeSila2Sever
{
    public sealed class ReaderDevice {
        //private static readonly ReaderDevice instance = new ReaderDevice();
        private static Object thisLock = new Object();
        private static bool configured = false;
        private static bool initialized = false;
        private static bool resetted = false;

        public static bool is_barcode_found = false;

        private static readonly Device device = new Device();
        public static Status DeviceResetStatus = new Status (StatusCode.Internal, "Device Reset");
        static ReaderDevice()
        {
            //var t = device.GetType();
            //reset();
            //setConfig();
            //initialize();
            //t = device.GetType();
        }

        static public string ReadCode(System.String EmptyBarocdeString=null, int tryRun = 0)
        {
            lock (thisLock)
            {
                try
                {
                    if (!resetted)
                    {
                        reset();
                    }
                    if (!configured)
                    {
                        setConfig();
                    }
                    if (!initialized)
                    {
                        initialize();
                    }
                    
                    System.String bc = device.ReadCode();
                    is_barcode_found = true;
                    return bc;
                }
                catch (DeviceException deviceException)
                {
                    is_barcode_found = false;
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
            configured = true;
        }

        static public ConfigurationDatalogicMatrix200 getConfig()
        {
            return device.GetConfiguration();
        }

        //resetting barcode.dvice
        static public void reset()
        {
            device.Reset();
            resetted = true;
            /*setConfig();
            initialize();
            */
        }

        static public void initialize()
        {
            try
            {
                device.Initialize();
                initialized = true;
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

    class is_SilaBaseImpl : is_sila.is_silaBase
    {
        private System.String interface_version = "2.0";
        private System.String manufacturer = "Datalogic";
        private System.String serial_number = "N2468";
        private System.String firmware_version = "0.0.0.1";
        private System.String name = "hi";

        public override Task<DeviveIdentification> device_identification(Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Void request, ServerCallContext context)
        {
            return Task.FromResult(new DeviveIdentification { 
                DeviceName=name
                , DeviceFirmwareVersion= firmware_version
                , DeviceManufacturer=manufacturer
                , DeviceSerialNumber=serial_number
                , SilaInterfaceVersion=interface_version});
        }
    }
      
    //class readCodeContinuouslyImpl : read_codeContinuously.read_CodeContinuouslyBase
    //{
    //    public override async Task openReader(IAsyncStreamReader<ScannerContinousRequest> requestStream, IServerStreamWriter<ScannerResponse> responseStream, ServerCallContext context)
    //    {
    //        Console.WriteLine("here is readCodeContinuously" );
    //        AbortScanning.abort = false;            
    //        int i = 0;

    //        Task readRequestTask = Task.Run(async () =>
    //            {
    //                while (await requestStream.MoveNext())
    //                {
    //                    Console.WriteLine("got Request readCodeContinuously: " + requestStream.Current.Id);
    //                    Console.WriteLine("command: " + requestStream.Current.Command);
    //                    if (requestStream.Current.Command.Equals("START"))
    //                    {
    //                        ReaderDevice.setContinousRead();
    //                        Task t = Task.Run(async () =>
    //                        {
    //                            System.String barcode = "";
    //                            while (!AbortScanning.abort)
    //                            {
    //                                /*try
    //                                {*/
    //                                    barcode = ReaderDevice.ReadCode();
    //                                    await responseStream.WriteAsync(new ScannerResponse { Barocde = barcode });
    //                                    //await Task.Delay(1000);
    //                                /*}
    //                                catch (Exception e)
    //                                {
    //                                    Console.WriteLine("openReader exception caught");
    //                                }*/

    //                            }
    //                            Console.WriteLine("abort received!");
    //                        });
    //                    }
    //                    if (requestStream.Current.Command.Equals("STOP"))
    //                    {
    //                        AbortScanning.abort = true;
    //                        //ReaderDevice.setOneShotRead();
    //                        Console.WriteLine("STOP received from the client!");
                            
    //                        //await responseStream.WriteAsync(new ScannerResponse { Barocde = System.String.Format("MB{0, 8 :d8}", i++) });
                            
    //                    }
    //                }
    //            }
    //        );
            
    //        while (!AbortScanning.abort)
    //        {
    //            await Task.Delay(1000);
    //            if (readRequestTask.Exception != null)
    //            {
    //                throw readRequestTask.Exception.InnerException;
    //            }
    //        }

    //        ReaderDevice.setOneShotRead();
    //        Console.WriteLine("end of request channel!");
    //    }
    //}

    //class canReadCodeContinuouslyImpl : can_ReadCodeContinuously.can_ReadCodeContinuouslyBase
    //{
    //    public override async Task startReading(ScannerRequest request, IServerStreamWriter<ScannerResponse> responseStream, ServerCallContext context)
    //    {
    //        ReaderDevice.setContinousRead();
             
    //        AbortScanning.abort = false;
    //        context.Status = Status.DefaultSuccess;
    //        context.ResponseTrailers.Add(new Metadata.Entry("Erorr", "None"));

    //        Console.WriteLine("got Request startReading: " + request.Id);
    //        //System.String barcode = "MB000";
    //        Task t = Task.Run(async () => {
    //            int i = 0;
    //            System.String barcode;
    //            while (!AbortScanning.abort)
    //            {
    //                /*try
    //                {*/
    //                    barcode = ReaderDevice.ReadCode();
    //                    //await responseStream.WriteAsync(new ScannerResponse { Barocde = System.String.Format("MB{0, 8 :d8}", i++) });
    //                    await responseStream.WriteAsync(new ScannerResponse { Barocde = barcode });
    //                    //await Task.Delay(1000);
    //                /*}
    //                catch (Exception e)
    //                {
    //                    Console.WriteLine("startReading Excetpion caught");
    //                }*/
                    
    //            }
                             
    //            //ReaderDevice.setConfig();
    //            //ReaderDevice.initialize();
    //            //ReaderDevice.reset();
    //            context.ResponseTrailers.Add(new Metadata.Entry("Ready", "But of course not finished"));
    //        });
            
    //        while (!AbortScanning.abort)
    //        {
    //            await Task.Delay(1000);
    //            if (t.Exception != null)
    //            {
    //                throw t.Exception.InnerException;
    //            }
    //        }
    //        ReaderDevice.setOneShotRead();
    //    }
    //    public override Task<ScannerResponse> stopReading(ScannerRequest request, ServerCallContext context)
    //    {
    //        AbortScanning.abort = true;            
    //        Console.WriteLine("got Request stopReading: " + request.Id);
    //        return Task.FromResult(new ScannerResponse { Barocde = "Stopping..."});
    //    }
    //}
    
    
    class canReadCodeImpl : can_read_barcode.can_read_barcodeBase
    {
        //stores last scanned barcode
        private bool barcode_changed = false;
        private System.String emptyBarcodString = "no barcode scanned";
        private System.String _lastScannedBarcode = "no barcode scanned yet";
        private System.String lastScannedBarcode
        {
            get { return _lastScannedBarcode; }
            set
            {
                _lastScannedBarcode = value;
                barcode_changed = true;
            }        
        }

        public override async Task barcode(SiLA_Property_Request request, IServerStreamWriter<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> responseStream, ServerCallContext context)
        {
            if ((request.Frequency == null || request.Frequency.Value == 0) && (request.Threshold == null || request.Threshold.Value == 0))
            {
                await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String { Value = lastScannedBarcode });
            }
            else
            {
                if (request.Frequency != null)
                {
                    while (true)
                    {
                        await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String { Value = lastScannedBarcode });
                        Thread.Sleep((int)request.Frequency.Value);
                    }
                }
                else
                {
                    while (true)
                    {
                        if (barcode_changed)
                        {
                            barcode_changed = false;
                            await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String { Value = lastScannedBarcode });
                        }
                        Thread.Sleep(250);
                    }
                }
            }
        }

        public override async Task is_barcode_found(SiLA_Property_Request request, IServerStreamWriter<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean> responseStream, ServerCallContext context)
        {
            if ((request.Frequency == null || request.Frequency.Value == 0) && (request.Threshold == null || request.Threshold.Value == 0))
            {
                await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean { Value = ReaderDevice.is_barcode_found });
            }
            else
            {
                if (request.Frequency != null)
                {
                    while (true)
                    {
                        await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean { Value = ReaderDevice.is_barcode_found });
                        Thread.Sleep((int)request.Frequency.Value);
                    }
                }
                else
                {
                    while (true)
                    {
                        if (barcode_changed)
                        {
                            barcode_changed = false;
                            await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Boolean { Value = ReaderDevice.is_barcode_found });
                        }
                        Thread.Sleep(250);
                    }
                }
            }
        }
        
        public override async Task read_code_noexp(PhysicalValue request, IServerStreamWriter<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> responseStream, ServerCallContext context)
        {
            System.String newBarcode = "";
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan();
            //First write reposonse
            await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String 
                { 
                    Metadata = new SiLA_Metadata { Progress = 0, EstimatedDuration = new PhysicalValue{Unit= new Unit {Value="s"}, Value=0.001} } 
                });
             //getting timeout            
            if ("s".Equals(request.Unit.Value)) {
               timeout = new TimeSpan(0,0,(int)request.Value);
            }

            newBarcode = ReaderDevice.ReadCode(emptyBarcodString);  
            stopWatch.Start();            
            while (stopWatch.Elapsed < timeout && emptyBarcodString.Equals(newBarcode))
            {
                newBarcode = ReaderDevice.ReadCode(emptyBarcodString);
                int progress = (int) ((stopWatch.Elapsed.CompareTo(timeout)>0 ? timeout : stopWatch.Elapsed).TotalMilliseconds / timeout.TotalMilliseconds * 100);
                await responseStream.WriteAsync(
                    new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String {
                        Metadata = new SiLA_Metadata { Progress = progress }
                    }
                );
            }

            if (!emptyBarcodString.Equals(newBarcode))
                {
                    lastScannedBarcode = newBarcode;
                }
            try
            {
                await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String
                    {
                        Value = newBarcode,
                        Metadata = new SiLA_Metadata { Progress = 100 }
                    }
                );
            }
            catch (Exception e)
            {
                
            }
        }

        public override async Task read_code(PhysicalValue request, IServerStreamWriter<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> responseStream, ServerCallContext context)
        {
            System.String newBarcode = ReaderDevice.ReadCode();
            if (!emptyBarcodString.Equals(newBarcode))
            {
                lastScannedBarcode = newBarcode;
            }
            await responseStream.WriteAsync(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String { Value = lastScannedBarcode, Metadata = { Progress = 100 } });
        }

        /*
        public override Task<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> read_code_noexp (Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Void request, ServerCallContext context)
        {
            lastScannedBarcode = ReaderDevice.ReadCode();
            return Task.FromResult(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String { Value = lastScannedBarcode, Metadata = new SiLA_Metadata { } });         
        }

        public override Task<Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String> read_code(Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Void request, ServerCallContext context)
        {            
            try
            {
                lastScannedBarcode = ReaderDevice.ReadCode();
                return Task.FromResult(new Org.SilaStandard.V2.RealeaseCandidate.Stdlib.String { Value = lastScannedBarcode, Metadata = new SiLA_Metadata { } });
            } catch (RpcException e)
            {
                throw new RpcException(e.Status, e.Message);
            }
        }
        */
    }

    class abortImpl : abort.abortBase
    {
        public override Task<abortReply> abort(Org.SilaStandard.V2.RealeaseCandidate.Stdlib.Void request, ServerCallContext context)
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
            param.BaudRate = (ConfigurationDatalogicMatrix200.EBaudRate)Enum.Parse(typeof(ConfigurationDatalogicMatrix200.EBaudRate), request.BaudRate);
            param.ConnectionType = (ConfigurationDatalogicMatrix200.EConnectionType)Enum.Parse(typeof(ConfigurationDatalogicMatrix200.EConnectionType), request.ConnetionType);
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
        public override Task<BarcodeReaderConfiguration> getConfiguration(Org.SilaStandard.V2.RealeaseCandidate.CanSetConfiguration.Empty request, ServerCallContext context)
        {
            ConfigurationDatalogicMatrix200 param = ReaderDevice.getConfig();
            return Task.FromResult(new BarcodeReaderConfiguration { BaudRate = param.BaudRate.ToString(), ConnetionType = param.ConnectionType.ToString(), DeviceAddress = param.DeviceAddress, PortName = param.PortName });
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
                      is_sila.BindService(new is_SilaBaseImpl())
                    , can_read_barcode.BindService(new canReadCodeImpl())
                    //, can_ReadCodeContinuously.BindService(new canReadCodeContinuouslyImpl())
                    , abort.BindService(new abortImpl())
                    //, read_CodeContinuously.BindService (new readCodeContinuouslyImpl())
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
                Console.WriteLine("Could not inialize the device" + e.Message);
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

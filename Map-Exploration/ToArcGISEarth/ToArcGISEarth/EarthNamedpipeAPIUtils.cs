using ArcGISEarth.WCFNamedPipeIPC;
using NamedpipeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace EarthAPITest
{
    public class MessageStringEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class EarthNamedpipeAPIUtils : IEarthNamedpipeCallbackService
    {
        public const string c_processName = "ArcGISEarth";
        public const string cBasePipeAddress = "net.pipe://localhost/arcgisearth";
        public const string cNeedConnect = "Please start earth then connect to it";
        public const string cSuccess = "Success";
        public const string cFailed = "Failed";
        public const int cMaxBuffer = 2147483647;

        private IEarthNamedpipeService _channel = null;
        private ChannelFactory<IEarthNamedpipeService> _factory = null;
        public EventHandler OnNotify;

        public void CloseConnect()
        {
            if(_factory != null)
            {
                _factory.Close();
                _factory = null;
            }
            _channel = null;
        }

        private bool ProcessStdOutCallBack(string message, ref string address)
        {
            if (!String.IsNullOrEmpty(message) && message.Contains("net.pipe"))
            {
                address = message;
                return true;
            }
            return false;
        }

        public void Notify(string message)
        {
            if(OnNotify != null)
            {
                OnNotify(this, new MessageStringEventArgs() { Message = message });
            }
        }

        public IEarthNamedpipeService CreateChannel(string address)
        {
            try
            {
                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                binding.MaxBufferPoolSize = cMaxBuffer;
                binding.MaxBufferSize = cMaxBuffer;
                binding.MaxReceivedMessageSize = cMaxBuffer;
                binding.ReceiveTimeout = TimeSpan.MaxValue;

                ServiceEndpoint se = new ServiceEndpoint(
                    ContractDescription.GetContract(typeof(IEarthNamedpipeService)),
                    binding,
                    new EndpointAddress(address));

                _factory = new DuplexChannelFactory<IEarthNamedpipeService>(new InstanceContext(this), se);
                IEarthNamedpipeService channel = _factory.CreateChannel();
                return channel;
            }
            catch
            {
                Console.WriteLine("Something wrong with the namedpipe communication!");
                return null;
            }
        }

        public async Task<string> Connect(string exePath = null)
        {
            try
            {
              string address = null;
              ProcessUtils proc = new ProcessUtils(c_processName, exePath);
              if (!proc.IsRunning())
              {
                  await proc.Start((msg) => ProcessStdOutCallBack(msg, ref address));
                  if(String.IsNullOrEmpty(address))
                  {
                      return cFailed;
                  }
              }
              else
              {
                  address = cBasePipeAddress;
              }

              if (!String.IsNullOrEmpty(address))
              {
                  _channel = CreateChannel(address);

                  // call a function to test consistency of contract file.
                  string test = _channel.GetCameraJson();

                  if (_channel != null)
                  {
                      return cSuccess;
                  }
                  else
                  {
                      _channel = null;
                      return cFailed;
                  }
              }
              return cFailed;
            }
            catch
            {
                return cFailed;
            }
        }

        public string AddLayer(string json)
        {
            if(_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                _channel.AddLayer(json);
            }
            catch (FaultException<EarthNamedpipeFault> ex)
            {
                return ex.Message;
            }
            return "Waiting for the result of adding layer";
        }

        public string ClearLayers(string json)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                if (_channel.ClearLayers(json))
                {
                    return cSuccess;
                }
            }
            catch (FaultException<EarthNamedpipeFault> ex)
            {
                return ex.Message;
            }

            return cFailed;
        }

        public string FlyTo(string json)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                if (_channel.FlyTo(json))
                {
                    return cSuccess;
                }
            }
            catch (FaultException<EarthNamedpipeFault> ex)
            {
                return ex.Message;
            }

            return cFailed;
        }

        public string SetCamera(string json)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                if (_channel.SetCamera(json))
                {
                    return cSuccess;
                }
            }
            catch (FaultException<EarthNamedpipeFault> ex)
            {
                return ex.Message;
            }
            catch(Exception)
            {

            }

            return cFailed;
        }

        public string GetCamera()
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                return _channel.GetCameraJson();
            }
            catch (FaultException<EarthNamedpipeFault> ex)
            {
                return ex.Message;
            }
        }

        public string GetSnapshot(string path)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                System.Drawing.Bitmap bitmap =  _channel.GetSnapshot();
                bitmap.Save(path);
                return cSuccess;
            }
            catch (FaultException<EarthNamedpipeFault> ex)
            {
                return ex.Message;
            }
        }

    }
}

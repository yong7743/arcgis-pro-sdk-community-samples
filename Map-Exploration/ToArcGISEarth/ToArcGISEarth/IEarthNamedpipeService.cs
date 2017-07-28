using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    public interface IEarthNamedpipeCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void Notify(string message);
    }

    [DataContract]
    public class EarthNamedpipeFault
    {
        [DataMember]
        public string Message { get; set; }
    }

    [ServiceContract(
        Namespace = "ArcGISEarth/2017/07", 
        CallbackContract = typeof(IEarthNamedpipeCallbackService))]
    public interface IEarthNamedpipeService
    {
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        string GetCameraJson();

        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        bool SetCamera(string json);

        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        bool FlyTo(string json);

        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        void AddLayer(string json);

        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        bool ClearLayers(string json);

        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        System.Drawing.Bitmap GetSnapshot();
    }
}





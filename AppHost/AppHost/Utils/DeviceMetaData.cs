using Acr.DeviceInfo;

namespace AppHost.Utils
{
    public class DeviceMetaData
    {
        public string appShortVersion;
        public string appVersion;
        public int batteryPercentage;
        public PowerStatus batteryStatus;
        public string connectivityCellularNetworkCarrier;
        public NetworkReachability connectivityInternetReachability;
        public string connectivityIpAddress;
        public string connectivityWifiSsid;
        public string hardwareDeviceId;
        public bool hardwareIsSimulator;
        public bool hardwareIsTablet;
        public string hardwareManufacturer;
        public string hardwareModel;
        public string hardwareOperatingSystem;
        public OperatingSystemType hardwareOS;
        public int hardwareScreenHeight;
        public int hardwareScreenWidth;
    }
}

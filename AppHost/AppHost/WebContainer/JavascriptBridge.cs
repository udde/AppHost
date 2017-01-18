using Acr.DeviceInfo;
//using Android.Content.PM; //tog bort för att göra mobile center glad
using AppHost.Utils;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AppHost.WebContainer
{
    public class JavascriptBridge
    {
        public EventHandler<string> CallJavaScript
        {
            get { return mCallJavaScript; }
            set { mCallJavaScript = value; }
        }

        public EventHandler<CallbackEventArgs> CallJavaScriptCallback
        {
            get { return mCallJavaScriptCallback; }
            set { mCallJavaScriptCallback = value; }
        }

        private EventHandler<string> mCallJavaScript;
        private EventHandler<CallbackEventArgs> mCallJavaScriptCallback;
        private DeviceMetaData mDeviceMetaData;
        private AdvancedWebView mWebview;

        public JavascriptBridge(AdvancedWebView webview)
        {
            mWebview = webview;

            mDeviceMetaData = GetDeviceMetaData();

            RegisterFunctions(mWebview);
        }

        private void RegisterFunctions(AdvancedWebView webview)
        {
            webview.RegisterNativeFunction("getMetaData", GetMetaData);
            webview.RegisterNativeFunction("getGeoPositionStatus", GetGeoPositionStatus);
            webview.RegisterNativeFunction("getGeoPos", GetGeoPos);
            webview.RegisterNativeFunction("geoPosStartListening", GeoPosStartListening);
            webview.RegisterNativeFunction("geoPosStopListening", GeoPosStopListening);
            webview.RegisterNativeListenerFunction("registerGeoPosListener", RegisterGeoPosListener);
            webview.RegisterNativeFunction("browserReload", BrowserReload);
            webview.RegisterNativeFunction("browserGoBack", BrowserGoBack);
            webview.RegisterNativeFunction("browserGoForward", BrowserGoForward);
            webview.RegisterNativeFunction("saveProperty", SaveProperty);
            webview.RegisterNativeFunction("hasSavedProperty", HasSavedProperty);
            webview.RegisterNativeFunction("loadProperty", LoadProperty);
            webview.RegisterNativeFunction("loadAllProperties", LoadAllProperties);
        }

        public void UnRegisterFunctions(AdvancedWebView webview)
        {
            CrossGeolocator.Current.StopListeningAsync();
        }

        private async Task<object[]> GetMetaData(string indata)
        {
            mDeviceMetaData = UpdateDeviceMetaData(mDeviceMetaData);
            return new object[] { mDeviceMetaData };
        }

        private DeviceMetaData UpdateDeviceMetaData(DeviceMetaData deviceMetaData)
        {
            deviceMetaData.appShortVersion = DeviceInfo.App.ShortVersion;
            deviceMetaData.appVersion = DeviceInfo.App.Version;
            deviceMetaData.batteryPercentage = DeviceInfo.Battery.Percentage;
            deviceMetaData.batteryStatus = DeviceInfo.Battery.Status;
            deviceMetaData.connectivityCellularNetworkCarrier = DeviceInfo.Connectivity.CellularNetworkCarrier;
            deviceMetaData.connectivityInternetReachability = DeviceInfo.Connectivity.InternetReachability;
            deviceMetaData.connectivityIpAddress = DeviceInfo.Connectivity.IpAddress;
            deviceMetaData.connectivityWifiSsid = DeviceInfo.Connectivity.WifiSsid;
            return deviceMetaData;
        }

        // Only run this on the main (UI thread)!
        private DeviceMetaData GetDeviceMetaData()
        {
            DeviceMetaData deviceMetaData = new DeviceMetaData();
            deviceMetaData.appShortVersion = DeviceInfo.App.ShortVersion;
            deviceMetaData.appVersion = DeviceInfo.App.Version;
            deviceMetaData.batteryPercentage = DeviceInfo.Battery.Percentage;
            deviceMetaData.batteryStatus = DeviceInfo.Battery.Status;
            deviceMetaData.connectivityCellularNetworkCarrier = DeviceInfo.Connectivity.CellularNetworkCarrier;
            deviceMetaData.connectivityInternetReachability = DeviceInfo.Connectivity.InternetReachability;
            deviceMetaData.connectivityIpAddress = DeviceInfo.Connectivity.IpAddress;
            deviceMetaData.connectivityWifiSsid = DeviceInfo.Connectivity.WifiSsid;
            //deviceMetaData.hardwareDeviceId = DeviceInfo.Hardware.DeviceId;
            deviceMetaData.hardwareIsSimulator = DeviceInfo.Hardware.IsSimulator;
            deviceMetaData.hardwareIsTablet = DeviceInfo.Hardware.IsTablet;
            deviceMetaData.hardwareManufacturer = DeviceInfo.Hardware.Manufacturer;
            deviceMetaData.hardwareModel = DeviceInfo.Hardware.Model;
            deviceMetaData.hardwareOperatingSystem = DeviceInfo.Hardware.OperatingSystem;
            deviceMetaData.hardwareOS = DeviceInfo.Hardware.OS;
            deviceMetaData.hardwareScreenHeight = DeviceInfo.Hardware.ScreenHeight;
            deviceMetaData.hardwareScreenWidth = DeviceInfo.Hardware.ScreenWidth;
            return deviceMetaData;
        }

        private async Task<object[]> GetGeoPositionStatus(string indata)
        {
            GeoPositionStatus geoPosStatus = new GeoPositionStatus();
            geoPosStatus.isGeolocationEnabled = CrossGeolocator.Current.IsGeolocationEnabled;
            geoPosStatus.isGeolocationAvailable = CrossGeolocator.Current.IsGeolocationAvailable;
            geoPosStatus.isListning = CrossGeolocator.Current.IsListening;
            return new object[] { geoPosStatus };
        }

        private async Task<object[]> GetGeoPos(string indata)
        {
            Position pos = await CrossGeolocator.Current.GetPositionAsync();
            return new object[] { pos };
        }

        private async Task<object[]> GeoPosStartListening(string indata)
        {
            GeoStartListening geoStartListening = JsonConvert.DeserializeObject<GeoStartListening>(indata);

            bool listening = await CrossGeolocator.Current.StartListeningAsync(geoStartListening.MinTime,
                geoStartListening.MinDistance,
                geoStartListening.IncludeHeading);
            return new object[] { listening };
        }

        private async Task<object[]> GeoPosStopListening(string indata)
        {
            bool listening = await CrossGeolocator.Current.StopListeningAsync();
            return new object[] { listening };
        }

        private async Task<object[]> RegisterGeoPosListener(string indata, string listenerCallback)
        {
            CrossGeolocator.Current.PositionChanged += (sender, args) =>
            {
                CallbackEventArgs eventArgs = new CallbackEventArgs(listenerCallback, new object[] { args.Position });
                mCallJavaScriptCallback?.Invoke(this, eventArgs);
            };

            bool isListening = CrossGeolocator.Current.IsListening;
            return new object[] { isListening };
        }

        private async Task<object[]> BrowserReload(string indata)
        {
            mWebview.Reload();

            return new object[] { };
        }

        private async Task<object[]> BrowserGoBack(string indata)
        {
            mWebview.GoBack();

            return new object[] { };
        }

        private async Task<object[]> BrowserGoForward(string indata)
        {
            mWebview.GoForward();

            return new object[] { };
        }

        private async Task<object[]> SaveProperty(string indata)
        {
            Property property = JsonConvert.DeserializeObject<Property>(indata);
            bool ret = false;

            if (!string.IsNullOrEmpty(property.Key) &&
                !string.IsNullOrEmpty(property.Value))
            {
                Settings.Settings.AddSetting(property.Key, property.Value);
                ret = true;
            }

            return new object[] { ret };
        }

        private async Task<object[]> HasSavedProperty(string indata)
        {
            Property property = JsonConvert.DeserializeObject<Property>(indata);

            bool ret = false;

            if (!string.IsNullOrEmpty(property.Key))
            {
                string value = Settings.Settings.GetSetting(property.Key);
                ret = !string.IsNullOrEmpty(value);
            }

            return new object[] { ret };
        }

        private async Task<object[]> LoadProperty(string indata)
        {
            Property property = JsonConvert.DeserializeObject<Property>(indata);

            string ret = "";

            if (!string.IsNullOrEmpty(property.Key))
            {
                string value = Settings.Settings.GetSetting(property.Key);
                if (!string.IsNullOrEmpty(value))
                {
                    ret = value;
                }
            }

            return new object[] { ret };
        }

        private async Task<object[]> LoadAllProperties(string indata)
        {
            List<Property> properties = new List<Property>();

            foreach (KeyValuePair<string, string> entry in Settings.Settings.GetAllSettings())
            {
                string key = entry.Key;
                string value = entry.Value as string;

                if (!string.IsNullOrEmpty(value))
                {
                    properties.Add(new Property(key, value));
                }
            }
            return new object[] { properties };
        }

        [DataContract]
        private class GeoStartListening
        {
            [DataMember(Name = "minTime")]
            public int MinTime { get; set; }
            [DataMember(Name = "minDistance")]
            public int MinDistance { get; set; }
            [DataMember(Name = "includeHeading")]
            public bool IncludeHeading { get; set; }
        }

        [DataContract]
        private class Property
        {
            [DataMember(Name = "key")]
            public string Key { get; set; }
            [DataMember(Name = "value")]
            public string Value { get; set; }

            public Property(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}

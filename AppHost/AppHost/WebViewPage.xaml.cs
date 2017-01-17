using Acr.DeviceInfo;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppHost
{
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage()
        {
            InitializeComponent();

            LoadLoaclDefaultWapp();
        }

        public void LoadLoaclDefaultWapp()
        {
            LoadLocalWapp("defaultwapp");
            //LoadLocalWapp("mapwapp");
            //LoadLocalWapp("map_draw_stuff_on_map");
        }

        public void LoadLocalWapp(string wapp)
        {
            _webview.LoadLocalContent(wapp);
        }

        public void LoadExternalUrl(Uri url)
        {
            _webview.LoadURL(url);
        }

        public void GoBackJavascript()
        {
            _webview.CallJsFunction("OnNativeGoBack");
        }

        protected override bool OnBackButtonPressed()
        {
            //_webview.GoBack();
            GoBackJavascript();
            return true; //Returning true blocks the 'normal' navigation behaviour
        }
    }
}

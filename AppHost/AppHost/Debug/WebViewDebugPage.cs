using Xamarin.Forms;

namespace AppHost.Debug
{
    public class WebViewDebugPage: MasterDetailPage
    {
        private WebViewPage mWebViewPage;
        private WebViewNavigation mWebViewNavigation;

        public WebViewDebugPage()
        {
            mWebViewPage = new WebViewPage();
            mWebViewNavigation = new WebViewNavigation();

            Detail = mWebViewPage;
            Master = mWebViewNavigation;
            mWebViewNavigation.WebViewPage = mWebViewPage;
        }
    }
}

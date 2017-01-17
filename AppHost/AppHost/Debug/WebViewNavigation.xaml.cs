using System;
using Xamarin.Forms;

namespace AppHost.Debug
{
    public partial class WebViewNavigation: ContentPage
    {
        public WebViewPage WebViewPage
        {
            get { return mWebViewPage; }
            set { mWebViewPage = value; }
        }

        private WebViewPage mWebViewPage;

        public WebViewNavigation()
        {
            InitializeComponent();

            _loadDefualtButton.Clicked += loadDefualtButtonClicked;
            _loadDefualtTestUrlButton.Clicked += loadDefualtTestUrlButtonClicked;
            _loadUrlButton.Clicked += loadDefualtTestUrlButtonClicked;
        }

        private void loadUrlButtonClicked(object sender, System.EventArgs e)
        {
            if (mWebViewPage != null)
            {
                string urlS = _urlEntry.Text;

                if (!urlS.StartsWith("http://") && !urlS.StartsWith("https://"))
                {
                    urlS = "http://" + urlS;
                }

                Uri url = new Uri(urlS);

                mWebViewPage.LoadExternalUrl(url);
            }
        }

        private void loadDefualtTestUrlButtonClicked(object sender, System.EventArgs e)
        {
            if (mWebViewPage != null)
            {
                Uri url = new Uri("http://192.168.20.121");
                mWebViewPage.LoadExternalUrl(url);
            }
        }

        private void loadDefualtButtonClicked(object sender, System.EventArgs e)
        {
            if (mWebViewPage != null)
            {
                mWebViewPage.LoadLoaclDefaultWapp();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            //mWebViewPage.GoBackJavascript();
            return false; //Returning false keeps the 'normal' behaviour
        }
    }
}

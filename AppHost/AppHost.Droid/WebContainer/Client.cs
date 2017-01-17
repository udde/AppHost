using Android.Webkit;
using System;

namespace AppHost.Droid.WebContainer
{
    public class Client : WebViewClient
    {
        protected readonly WeakReference<AdvancedWebViewRenderer> mAdvancedWebView;

        public Client(AdvancedWebViewRenderer advancedWebView)
        {
            mAdvancedWebView = new WeakReference<AdvancedWebViewRenderer>(advancedWebView);
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);

            AdvancedWebViewRenderer advancedWebView;
            if (mAdvancedWebView != null && mAdvancedWebView.TryGetTarget(out advancedWebView))
            {
                advancedWebView.OnPageFinished();
            }
        }
    }
}
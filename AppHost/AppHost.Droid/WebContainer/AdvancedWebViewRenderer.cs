using Android.Webkit;
using AppHost.WebContainer;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Java.Interop;
using System.Text;
using System.IO;
using Android.OS;
using Android.Widget;
using Android.Content;

[assembly: ExportRenderer(typeof(AdvancedWebView), typeof(AppHost.Droid.WebContainer.AdvancedWebViewRenderer))]
namespace AppHost.Droid.WebContainer
{
    public partial class AdvancedWebViewRenderer : ViewRenderer<AdvancedWebView, NativeWebView>
    {
        private const string NativeFuncCall = "Xamarin.call";

        public static Func<AdvancedWebViewRenderer, Client> GetWebViewClientDelegate;

        public static Func<AdvancedWebViewRenderer, ChromeClient> GetWebChromeClientDelegate;

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            var sizeRequest = base.GetDesiredSize(widthConstraint, heightConstraint);
            sizeRequest.Request = new Size(sizeRequest.Request.Width, 0);
            return sizeRequest;

            //return new SizeRequest(Size.Zero, Size.Zero);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdvancedWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.NewElement != null)
            {
                NativeWebView webView = new NativeWebView(this);

                if ((int)Build.VERSION.SdkInt >= 19)
                {
                    webView.SetLayerType(LayerType.Hardware, null);
                }
                else
                {
                    webView.SetLayerType(LayerType.Software, null);
                }

                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = true;
                webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                webView.Settings.CacheMode = CacheModes.NoCache;
                
                // Set the background color to transparent to fix an issue where the
                // the picture would fail to draw
                webView.SetBackgroundColor(Color.Transparent.ToAndroid());

                webView.SetWebViewClient(GetWebViewClient());
                webView.SetWebChromeClient(GetWebChromeClient());

                Java.Lang.Object scriptInterface = new ScriptInterface(Context, this);
                webView.AddJavascriptInterface(scriptInterface, "Xamarin");

                SetNativeControl(webView);

                webView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

                
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
                {
#if DEBUG
                    NativeWebView.SetWebContentsDebuggingEnabled(true);
#endif
                }
            }

            Unbind(e.OldElement);

            TryToLoadContent();

            Bind();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Element != null)
            {
                if (Control != null)
                {
                    Control.StopLoading();
                }

                Unbind(Element);
            }

            base.Dispose(disposing);
        }

        protected virtual Client GetWebViewClient()
        {
            var d = GetWebViewClientDelegate;

            if (d == null)
            {
                return new Client(this);
            }
            else
            {
                return d(this);
            }
        }

        protected virtual ChromeClient GetWebChromeClient()
        {
            var d = GetWebChromeClientDelegate;
            
            if (d == null)
            {
                return new ChromeClient();
            }
            else
            {
                return d(this);
            }
        }

        private void HandleCleanup()
        {
            if (Control == null) return;

            Control.SetWebViewClient(null);
            Control.SetWebChromeClient(null);
            Control.RemoveJavascriptInterface("Xamarin");
        }

        public void OnPageFinished()
        {
            if (Element == null)
                return;

            Inject(GetFuncScripts());
            Element.OnLoadFinished(this, EventArgs.Empty);
        }

        private string GetFuncScripts()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("function NativeFunc(action) { ");
            builder.Append(NativeFuncCall).Append("(JSON.stringify({ a: action })); ");
            builder.Append("} ");

            builder.Append("function NativeFuncData(action, data) { ");
            builder.Append(NativeFuncCall).Append("(JSON.stringify({ a: action, d: data })); ");
            builder.Append("} ");

            builder.Append("NativeFuncs = [];");

            builder.Append("function NativeFuncDataCallback(action, data, callback) { ");
            builder.Append(" var callbackIdx = NativeFuncs.push(callback) - 1; ");
            builder.Append(NativeFuncCall).Append("(JSON.stringify({ a: action, d: data, c: callbackIdx })); ");
            builder.Append("} ");

            builder.Append("function NativeFuncListener(action, data, callback, listener) { ");
            builder.Append(" var callbackIdx = NativeFuncs.push(callback) - 1; ");
            builder.Append(" var listenerIdx = NativeFuncs.push(listener) - 1; ");
            builder.Append(NativeFuncCall).Append("(JSON.stringify({ a: action, d: data, c: callbackIdx, l: listenerIdx })); ");
            builder.Append("} ");

            builder.Append("if (typeof(window.NativeFuncsReady) !== 'undefined') { ");
            builder.Append("  window.NativeFuncsReady(); ");
            builder.Append("} ");

            return builder.ToString();
        }

        private void Inject(string script)
        {
            if (Control != null)
            {
                Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }

        private void GoBack()
        {
            if (Control != null)
            {
                //bool canGoBack = Control.CanGoBack();
                //System.Diagnostics.Debug.WriteLine($"Control.CanGoBack() = {canGoBack}");
                System.Diagnostics.Debug.WriteLine("Trying to call Control.GoBack()");
                Control.GoBack();
            }
        }

        private void GoForward()
        {
            if (Control != null)
            {
                Control.GoForward();
            }
        }

        private void Reload()
        {
            if (Control != null)
            {
                Control.Reload();
            }
        }

        private void Stop()
        {
            if (Control != null)
            {
                Control.StopLoading();
            }
        }

        private void TryToLoadContent()
        {
            if (Element != null)
            {
                if (Control != null && 
                    !string.IsNullOrEmpty(Element.LocalFile) &&
                    !string.IsNullOrEmpty(Element.LocalBaseFolder))
                {
                    LoadFromLocalFile(Element.LocalFile, Element.LocalBaseFolder);
                }
                else
                {
                    if (Element.Uri != null)
                    {
                        LoadUrl(Element.Uri);
                    }
                }
            }
        }

        private void LoadUrl(Uri uri)
        {
            if (uri != null &&
                Control != null)
            {
                Control.LoadUrl(uri.AbsoluteUri);
            }
        }

        private void LoadFromLocalFile(string filePath, string basePath = null)
        {
            if (Control != null)
            {
                string baseUrl = "file:///android_asset/";

                if (!string.IsNullOrEmpty(basePath))
                {
                    baseUrl = baseUrl + basePath + "/";
                }

                string path = basePath + "/" + filePath;
                
                using (Stream stream = Context.Assets.Open(path, Android.Content.Res.Access.Streaming))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string htmlFile = sr.ReadToEnd();
                        Control.LoadDataWithBaseURL(baseUrl, htmlFile, "text/html", "UTF-8", null);
                    }
                }
            }
        }

        private void OnLoadLocalContentRequest(object sender, LoadLocalContentEventArgs contentArgs)
        {
            LoadFromLocalFile(contentArgs.LocalFile, contentArgs.LocalBaseFolder);
        }

        private void OnLoadURLRequested(object sender, LoadURLContentEventArgs contentArgs)
        {
            LoadUrl(contentArgs.Url);
        }
        
        private void OnBrowserReloadRequest(object sender, EventArgs args)
        {
            Reload();
        }

        private void OnBrowserGoBackRequest(object sender, EventArgs args)
        {
            GoBack();
        }

        private void OnBrowserGoForwardRequest(object sender, EventArgs args)
        {
            GoForward();
        }

        private void Bind()
        {
            if (Element != null)
            {
                // There should only be one renderer and thus only one event handler registered.
                // Otherwise, when Xamarin creates a new renderer, the old one stays attached
                // and crashes when called!
                Element.JavaScriptLoadRequested = OnInjectRequest;
                Element.LoadLocalContentRequested = OnLoadLocalContentRequest;
                Element.LoadURLRequested = OnLoadURLRequested;
                Element.BrowserReloadRequested = OnBrowserReloadRequest;
                Element.BrowserGoBackRequested = OnBrowserGoBackRequest;
                Element.BrowserGoForwardRequested = OnBrowserGoForwardRequest;
            }
        }

        private void OnInjectRequest(object sender, string script)
        {
            Inject(script);
        }

        private void Unbind(AdvancedWebView oldElement)
        {
            if (oldElement != null)
            {
                oldElement.JavaScriptLoadRequested -= OnInjectRequest;
                oldElement.LoadLocalContentRequested -= OnLoadLocalContentRequest;
                oldElement.LoadURLRequested -= OnLoadURLRequested;
                oldElement.PropertyChanged -= OnElementPropertyChanged;
                oldElement.BrowserReloadRequested -= OnBrowserReloadRequest;
                oldElement.BrowserGoBackRequested -= OnBrowserGoBackRequest;
                oldElement.BrowserGoForwardRequested -= OnBrowserGoForwardRequest;
            }
        }

        public class ScriptInterface: Java.Lang.Object
        {
            private readonly WeakReference<AdvancedWebViewRenderer> mAdvancedWebView;

            private Context mContext;

            public ScriptInterface(Context context, AdvancedWebViewRenderer advancedWebView)
            {
                mContext = context;
                mAdvancedWebView = new WeakReference<AdvancedWebViewRenderer>(advancedWebView);
            }

            [JavascriptInterface]
            [Export("call")]
            public void Call(string message)
            {
                AdvancedWebViewRenderer advancedWebView;
                AdvancedWebView webView;
                if (mAdvancedWebView != null && 
                    mAdvancedWebView.TryGetTarget(out advancedWebView) && 
                    ((webView = advancedWebView.Element) != null))
                {
                    webView.MessageReceived(message);
                }
            }

            /** Show a toast from the web page */
            [JavascriptInterface]
            [Export("showToast")]
            public void ShowToast(String toast)
            {
                Toast.MakeText(mContext, toast, ToastLength.Long).Show();
            }
        }
    }
}
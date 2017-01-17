using AppHost.WebContainer;
using Foundation;
using System;
using System.IO;
using System.Text;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdvancedWebView), typeof(AppHost.iOS.WebContainer.AdvancedWebViewRenderer))]
namespace AppHost.iOS.WebContainer
{
    public class AdvancedWebViewRenderer : ViewRenderer<AdvancedWebView, WKWebView>, IWKScriptMessageHandler
    {
        private const string NativeFuncCall = "window.webkit.messageHandlers.native.postMessage";

        private UISwipeGestureRecognizer mLeftSwipeGestureRecognizer;
        private UISwipeGestureRecognizer mRightSwipeGestureRecognizer;
        private WKUserContentController mUserController;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return new SizeRequest(Size.Zero, Size.Zero);
        }

        [Export("webView:didCommitNavigation:")]
        public void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
        {
            System.Diagnostics.Debug.WriteLine("DidCommitNavigation: " + webView.Url);
        }

        [Export("webView:didStartProvisionalNavigation:")]
        public void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            Element.OnNavigating(webView.Url);
            System.Diagnostics.Debug.WriteLine("DidStartProvisionalNavigation: " + webView.Url);
        }

        [Export("webView:didReceiveServerRedirectForProvisionalNavigation:")]
        public void DidReceiveServerRedirectForProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            System.Diagnostics.Debug.WriteLine("DidReceiveServerRedirectForProvisionalNavigation: " + webView.Url);
        }

        [Export("webView:didFailNavigation:withError:")]
        public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            System.Diagnostics.Debug.WriteLine("DidFailNavigation: " + webView.Url + " error: " + error.ToString());
        }

        [Export("webView:didFailProvisionalNavigation:withError:")]
        public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            //Debug.WriteLine("DidFailProvisionalNavigation: " + error.UserInfo[NSURLErrorFailingURLStringErrorKey] + " error: " + error.ToString());
            System.Diagnostics.Debug.WriteLine("DidFailProvisionalNavigation: " + webView.Url + " error: " + error.ToString());
        }

        [Export("webView:didFinishNavigation:")]
        public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            Element.OnLoadFinished(webView, EventArgs.Empty);
            System.Diagnostics.Debug.WriteLine("DidFinishNavigation: " + webView.Url);
        }
        
        [Export("webViewWebContentProcessDidTerminate:")]
        public void WebContentProcessDidTerminate(WKWebView webView)
        {
            System.Diagnostics.Debug.WriteLine("WebContentProcessDidTerminate: " + webView.Url);
        }

        //[Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        //public void DecidePolicyForNavigationAction(WKWebView webView, WKNavigation navigation, nån handler?)
        //{
        //    ;
        //}

        //[Export("webView:decidePolicyForNavigationResponse:decisionHandler:")]
        //public void DecidePolicyForNavigationResponse(WKWebView webView, WKNavigation navigation, nån handler?)
        //{
        //    ;
        //}

        //[Export("webView:didReceiveAuthenticationChallenge:completionHandler:")]
        //public void DidReceiveAuthenticationChallenge(WKWebView webView, NSUrlAuthenticationChallenge challenge, nån handler)
        //{
        //    ;
        //}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            Control.ScrollView.Frame = Control.Bounds;
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            Element.MessageReceived(message.Body.ToString());
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdvancedWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.NewElement != null)
            {
                mUserController = new WKUserContentController();
                WKWebViewConfiguration config = new WKWebViewConfiguration()
                {
                    UserContentController = mUserController
                };

                WKUserScript script = new WKUserScript(new NSString(GetFuncScripts()), WKUserScriptInjectionTime.AtDocumentEnd, false);

                mUserController.AddUserScript(script);

                mUserController.AddScriptMessageHandler(this, "native");

                WKWebView webView = new WKWebView(Frame, config)
                {
                    WeakNavigationDelegate = this
                };

                // NSUrlRequest urlRequest = new NSUrlRequest(new Uri("http://www.dn.se/"));
                // webView.LoadRequest(urlRequest);
                SetNativeControl(webView);

                // webView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                // webView.ScalesPageToFit = true;

                mLeftSwipeGestureRecognizer = new UISwipeGestureRecognizer(() => Element.OnLeftSwipe(this, EventArgs.Empty))
                {
                    Direction = UISwipeGestureRecognizerDirection.Left
                };

                mRightSwipeGestureRecognizer = new UISwipeGestureRecognizer(() => Element.OnRightSwipe(this, EventArgs.Empty))
                {
                    Direction = UISwipeGestureRecognizerDirection.Right
                };

                webView.AddGestureRecognizer(mLeftSwipeGestureRecognizer);
                webView.AddGestureRecognizer(mRightSwipeGestureRecognizer);
            }

            if ((e.NewElement == null) && 
                (Control != null))
            {
                Control.RemoveGestureRecognizer(mLeftSwipeGestureRecognizer);
                Control.RemoveGestureRecognizer(mRightSwipeGestureRecognizer);
            }

            Unbind(e.OldElement);

            TryToLoadContent();

            Bind();
        }

        private void HandleCleanup()
        {
            if (Control == null)
                return;

            Control.RemoveGestureRecognizer(mLeftSwipeGestureRecognizer);
            Control.RemoveGestureRecognizer(mRightSwipeGestureRecognizer);
        }

        private void Inject(string script)
        {
            InvokeOnMainThread(() => Control.EvaluateJavaScript(new NSString(script), (r, e) =>
            {
                if (e != null)
                    System.Diagnostics.Debug.WriteLine(e);
            }));
        }

        private void GoBack()
        {
            if (Control != null)
            {
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
            if (uri != null)
            {
                NSUrl url = new NSUrl(uri.AbsoluteUri);
                NSUrlRequest request = new NSUrlRequest(url);
                Control.LoadRequest(request);
            }
        }

        private void LoadFromLocalFile(string filePath, string basePath = null)
        {
            if (Control != null)
            {
                string path = filePath;
                NSUrl baseUrl = NSBundle.MainBundle.BundleUrl;
                if (!string.IsNullOrEmpty(basePath))
                {
                    path = basePath + "/" + path;
                    baseUrl = baseUrl.Append(basePath, true);
                }

                string htmlFile = File.ReadAllText(path);
                Control.LoadHtmlString(htmlFile, baseUrl);
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

        /*
        public static void CopyBundleDirectory(string path)
        {
            string source = Path.Combine(NSBundle.MainBundle.BundlePath, path);
            string dest = Path.Combine(GetTempDirectory(), path);

            FileManager.CopyDirectory(new DirectoryInfo(source), new DirectoryInfo(dest));
        }
        */

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
                oldElement.PropertyChanged -= this.OnElementPropertyChanged;
                oldElement.BrowserReloadRequested -= OnBrowserReloadRequest;
                oldElement.BrowserGoBackRequested -= OnBrowserGoBackRequest;
                oldElement.BrowserGoForwardRequested -= OnBrowserGoForwardRequest;
            }
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

        private static string GetTempDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("Documents", "tmp");
        }
    }
}

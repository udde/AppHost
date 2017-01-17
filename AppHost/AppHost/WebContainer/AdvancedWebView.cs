using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppHost.WebContainer
{
    public class AdvancedWebView : View
    {
        public static readonly BindableProperty UriProperty =
            BindableProperty.Create(nameof(Uri), typeof(Uri), typeof(AdvancedWebView), default(Uri));
        //public static readonly BindableProperty LocalFileProperty =
        //    BindableProperty.Create(nameof(LocalFile), typeof(string), typeof(AdvancedWebView), "");
        //public static readonly BindableProperty LocalBaseFolderProperty =
        //    BindableProperty.Create(nameof(LocalBaseFolder), typeof(string), typeof(AdvancedWebView), "");
        public static readonly BindableProperty CleanupProperty =
            BindableProperty.Create(nameof(CleanupCalled), typeof(bool), typeof(AdvancedWebView), false);

        public EventHandler<string> JavaScriptLoadRequested
        {
            get { return mJavaScriptLoadRequested; }
            set { mJavaScriptLoadRequested = value; }
        }

        public EventHandler<LoadLocalContentEventArgs> LoadLocalContentRequested
        {
            get { return mLoadLocalContentRequested; }
            set { mLoadLocalContentRequested = value; }
        }

        public EventHandler<LoadURLContentEventArgs> LoadURLRequested
        {
            get { return mLoadURLRequested; }
            set { mLoadURLRequested = value; }
        }

        public EventHandler BrowserReloadRequested
        {
            get { return mBrowserReloadRequested; }
            set { mBrowserReloadRequested = value; }
        }

        public EventHandler BrowserGoBackRequested
        {
            get { return mBrowserGoBackRequested; }
            set { mBrowserGoBackRequested = value; }
        }

        public EventHandler BrowserGoForwardRequested
        {
            get { return mBrowserGoForwardRequested; }
            set { mBrowserGoForwardRequested = value; }
        }

        public EventHandler LeftSwipe
        {
            get { return mLeftSwipe; }
            set { mLeftSwipe = value; }
        }

        public EventHandler LoadFinished
        {
            get { return mLoadFinished; }
            set { mLoadFinished = value; }
        }

        public EventHandler<UriEventArgs> Navigating
        {
            get { return mNavigating; }
            set { mNavigating = value; }
        }

        public EventHandler RightSwipe
        {
            get { return mRightSwipe; }
            set { mRightSwipe = value; }
        }

        private EventHandler mLeftSwipe;
        private EventHandler mLoadFinished;
        private EventHandler<UriEventArgs> mNavigating;
        private EventHandler mRightSwipe;

        private EventHandler<string> mJavaScriptLoadRequested;
        private EventHandler<LoadLocalContentEventArgs> mLoadLocalContentRequested;
        private EventHandler<LoadURLContentEventArgs> mLoadURLRequested;
        private EventHandler mBrowserReloadRequested;
        private EventHandler mBrowserGoBackRequested;
        private EventHandler mBrowserGoForwardRequested;

        private readonly object mInjectLock = new object();

        private readonly Dictionary<string, Action<string>> mRegisteredActions;
        private readonly Dictionary<string, Func<string, Task<object[]>>> mRegisteredFunctions;
        private readonly Dictionary<string, Func<string, string, Task<object[]>>> mRegisteredListenerFunctions;

        private string mLocalFile;
        private string mLocalBaseFolder;
        private JavascriptBridge mJavascriptBridge;

        public AdvancedWebView()
        {
            mRegisteredActions = new Dictionary<string, Action<string>>();
            mRegisteredFunctions = new Dictionary<string, Func<string, Task<object[]>>>();
            mRegisteredListenerFunctions = new Dictionary<string, Func<string, string, Task<object[]>>>();
        }

        private void SetUpJavascriptBridge()
        {
            if (mJavascriptBridge != null)
            {
                mRegisteredActions.Clear();
                mRegisteredFunctions.Clear();
                mRegisteredListenerFunctions.Clear();

                mJavascriptBridge.UnRegisterFunctions(this);
            }

            mJavascriptBridge = new JavascriptBridge(this);
            mJavascriptBridge.CallJavaScriptCallback = OnCallJavaScriptCallback;
        }

        public Uri Uri
        {
            get { return (Uri)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public bool CleanupCalled
        {
            get { return (bool)GetValue(CleanupProperty); }
            set { SetValue(CleanupProperty, value); }
        }

        public string LocalFile
        {
            get { return mLocalFile; }
        }

        public string LocalBaseFolder
        {
            get { return mLocalBaseFolder; }
        }

        public void RegisterCallback(string name, Action<string> action)
        {
            mRegisteredActions.Add(name, action);
        }

        public bool RemoveCallback(string name)
        {
            return mRegisteredActions.Remove(name);
        }

        public void RegisterNativeFunction(string name, Func<string, Task<object[]>> func)
        {
            mRegisteredFunctions.Add(name, func);
        }

        public bool RemoveNativeFunction(string name)
        {
            return mRegisteredFunctions.Remove(name);
        }

        public void RegisterNativeListenerFunction(string name, Func<string, string, Task<object[]>> func)
        {
            mRegisteredListenerFunctions.Add(name, func);
        }

        public bool RemoveNativeListenerFunction(string name)
        {
            return mRegisteredListenerFunctions.Remove(name);
        }

        public void LoadURL(Uri uri)
        {
            Uri = uri;
            mLoadURLRequested?.Invoke(this, new LoadURLContentEventArgs(Uri));
        }

        public void LoadLocalContent(string localBaseFolder, string localFile = "index.html")
        {
            mLocalFile = localFile;
            mLocalBaseFolder = localBaseFolder;
            mLoadLocalContentRequested?.Invoke(this, new LoadLocalContentEventArgs(mLocalFile, mLocalBaseFolder));
        }

        public void InjectJavaScript(string script)
        {
            lock (mInjectLock)
            {
                mJavaScriptLoadRequested?.Invoke(this, script);
            }
        }

        public void CallJsFunction(string funcName, params object[] parameters)
        {
            var builder = new StringBuilder();

            builder.Append(funcName);
            builder.Append("(");

            for (int i = 0; i < parameters.Length; i++)
            {
                string param = JsonConvert.SerializeObject(parameters[i]);
                builder.Append(param);
                if (i < parameters.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(");");

            InjectJavaScript(builder.ToString());
        }

        private bool TryGetAction(string name, out Action<string> action)
        {
            return mRegisteredActions.TryGetValue(name, out action);
        }

        private bool TryGetFunc(string name, out Func<string, Task<object[]>> func)
        {
            return mRegisteredFunctions.TryGetValue(name, out func);
        }

        private bool TryGetListenerFunc(string name, out Func<string, string, Task<object[]>> func)
        {
            return mRegisteredListenerFunctions.TryGetValue(name, out func);
        }

        public void OnLoadFinished(object sender, EventArgs e)
        {
            SetUpJavascriptBridge();

            LoadFinished?.Invoke(this, e);
            CallJsFunction("NativeReady");
        }

        public void OnLeftSwipe(object sender, EventArgs e)
        {
            LeftSwipe?.Invoke(this, e);
        }

        public void OnRightSwipe(object sender, EventArgs e)
        {
            RightSwipe?.Invoke(this, e);
        }

        public void OnNavigating(Uri uri)
        {
            Navigating?.Invoke(this, new UriEventArgs(uri));
        }

        public void Reload()
        {
            BrowserReloadRequested?.Invoke(this, new EventArgs());
        }
        
        public void GoBack()
        {
            mBrowserGoBackRequested?.Invoke(this, new EventArgs());
            System.Diagnostics.Debug.WriteLine("Trying to use CallJsFunction('Hello')");
            //CallJsFunction("Hello");
        }

        public void GoForward()
        {
            mBrowserGoForwardRequested?.Invoke(this, new EventArgs());
        }

        public void MessageReceived(string message)
        {
            Message m = JsonConvert.DeserializeObject<Message>(message);
            
            if (m?.Action == null)
                return;
            
            Action<string> action;
            
            if (TryGetAction(m.Action, out action))
            {
                action.Invoke(m.Data.ToString());
                return;
            }
            
            Func<string, Task<object[]>> func;
            
            if (TryGetFunc(m.Action, out func))
            {
                func.Invoke(m.Data.ToString()).ContinueWith(result =>
                {
                    if (!result.IsCanceled && result.IsCompleted && !result.IsFaulted)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CallJSCallback(m.Callback, result.Result);
                        });
                    }
                });

                /*
                Task.Run(async () =>
                {
                    func.Invoke(m.Data.ToString()).ContinueWith()
                    object[] result = await func.Invoke(m.Data.ToString());
                    CallJsFunction($"NativeFuncs[{m.Callback}]", result);
                });
                */
            }

            Func<string, string, Task<object[]>> listenerFunc;

            if (TryGetListenerFunc(m.Action, out listenerFunc))
            {
                listenerFunc.Invoke(m.Data.ToString(), m.Listener).ContinueWith(result =>
                {
                    if (!result.IsCanceled && result.IsCompleted && !result.IsFaulted)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            CallJSCallback(m.Callback, result.Result);
                        });
                    }
                });
            }
        }

        private void OnCallJavaScriptCallback(object sender, CallbackEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                CallJSCallback(args.CallbackId, args.Data);
            });
        }

        private void CallJSCallback(string callbackId, object[] args)
        {
            CallJsFunction($"NativeFuncs[{callbackId}]", args);
        }

        public void RemoveAllCallbacks()
        {
            mRegisteredActions.Clear();
        }

        public void RemoveAllFunctions()
        {
            mRegisteredFunctions.Clear();
        }

        public void Cleanup()
        {
            // This removes the delegates that point to the renderer
            mJavaScriptLoadRequested = null;
            mLoadLocalContentRequested = null;
            mLoadURLRequested = null;
            mNavigating = null;

            // Remove all callbacks
            mRegisteredActions.Clear();
            mRegisteredFunctions.Clear();

            // Cleanup the native stuff
            CleanupCalled = true;
        }

        [DataContract]
        private class Message
        {
            [DataMember(Name = "a")]
            public string Action { get; set; }
            [DataMember(Name = "d")]
            public object Data { get; set; }
            [DataMember(Name = "c")]
            public string Callback { get; set; }
            [DataMember(Name = "l")]
            public string Listener { get; set; }
        }
    }
}

using Android.Webkit;

namespace AppHost.Droid.WebContainer
{
    public class ChromeClient : WebChromeClient
    {
        public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        {
            callback.Invoke(origin, true, false);
        }

        public override void OnConsoleMessage(string message, 
            int lineNumber, 
            string sourceID)
        {
            System.Diagnostics.Debug.WriteLine("AppHost - " +
                message + " -- From line "
                + lineNumber + " of " + sourceID);
        }

        public override bool OnConsoleMessage(ConsoleMessage cm)
        {
            System.Diagnostics.Debug.WriteLine("AppHost - " + 
                cm.Message() + " -- From line "
                + cm.LineNumber() + " of " + cm.SourceId());
            return true;
        }
    }
}
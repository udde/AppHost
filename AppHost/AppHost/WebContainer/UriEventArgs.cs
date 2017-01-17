using System;

namespace AppHost.WebContainer
{
    public class UriEventArgs : EventArgs
    {
        public Uri Uri { get; set; }

        public UriEventArgs(Uri uri)
        {
            Uri = uri;
        }
    }
}

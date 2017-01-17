using System;

namespace AppHost.WebContainer
{
    public class LoadURLContentEventArgs : EventArgs
    {
        public Uri Url { get; private set; }

        public LoadURLContentEventArgs(Uri url)
        {
            Url = url;
        }
    }
}

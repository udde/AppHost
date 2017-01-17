using System;

namespace AppHost.WebContainer
{
    public class LoadLocalContentEventArgs : EventArgs
    {
        public string LocalFile { get; private set; }
        public string LocalBaseFolder { get; private set; }

        public LoadLocalContentEventArgs(string localFile, string localBaseFolder)
        {
            LocalFile = localFile;
            LocalBaseFolder = localBaseFolder;
        }
    }
}

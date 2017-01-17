using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHost.WebContainer
{
    public class CallbackEventArgs: EventArgs
    {
        public string CallbackId { get; set; }
        public object[] Data { get; set; }

        public CallbackEventArgs(string callbackId, object[] data)
        {
            CallbackId = callbackId;
            Data = data;
        }
    }
}

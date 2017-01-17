using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHost.Logging
{
    public class Logger : System.Diagnostics.
    {
        public override void WriteLine(string message)
        {
            DateTime date = DateTime.Now;
            string logString = date.ToString() + message;
            System.Diagnostics.Debug.WriteLine(logString);
            System.Diagnostics.Debug.WriteLine($"store \"{logString}\" in ai or textfile");
            //System.IO.File.AppendAllText("WriteText.txt", logString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Helpers
{
    public static class Log
    {
        public static void Info(string message)
        {
            Debug.WriteLine("Info: " + message);
        }

        public static void Info(object message)
        {
            Info(message.ToString());
        }

        public static void Error(string message)
        {
            Debug.WriteLine("Error: " + message);
        }
        public static void Error(string message, string method)
        {
            Debug.WriteLine("Error in '" + method + "': " + message);
        }
    }
}

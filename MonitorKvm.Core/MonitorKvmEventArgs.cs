using System;
using System.Collections.Generic;
using System.Text;

namespace MonitorKvm.Core
{
    public class MonitorKvmEventArgs
    {
        public MonitorKvmEventArgs(String message)
        {
            this.Message = message;
        }

        public String Message { get; set; }
    }
}
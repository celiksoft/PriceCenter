using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainedMarketDijitalTag.Helpers
{
    public class UpdateEventArgs
    {
        public UpdateEventArgs(UpdateType type, string value, string localServer, string eslName)
        {
            Type = type;
            NewValue = value;
            LocalServer = localServer;
            Esl = eslName;
        }

        public UpdateType Type
        {
            get;
            private set;
        }

        public string NewValue
        {
            get;
            private set;
        }

        public string LocalServer
        {
            get;
            private set;
        }

        public string Esl
        {
            get;
            private set;
        }
    }
}

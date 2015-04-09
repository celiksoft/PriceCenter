using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainedMarketDijitalTag.Messenger
{
    public static class MsgExtensions
    {
        public static void Publish(this Msg message, params object[] prms)
        {
            Messenger<Msg>.Default.Publish(message, prms);
        }
    }
}

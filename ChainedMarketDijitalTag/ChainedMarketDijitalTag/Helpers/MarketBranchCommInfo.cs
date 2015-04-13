using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ChainedMarketDijitalTag.Helpers
{
    public class MarketBranchServerInfo
    {
        public MarketBranchServerInfo(IPAddress ip,int socketNo)
        {
            Ip = ip;
            SocketNo = socketNo;
        }

        public IPAddress Ip
        {
            get;
            private set;
        }

        public int SocketNo
        {
            get;
            private set;
        }
    }
}

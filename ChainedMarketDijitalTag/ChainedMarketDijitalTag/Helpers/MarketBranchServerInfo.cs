using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChainedMarketDijitalTag.Helpers
{
    public class MarketBranchServerInfo
    {
        public MarketBranchServerInfo(IPAddress ip,int socketNo)
        {
            Ip = ip;
            SocketNo = socketNo;
        }

        public ObjectId id { get; set; }

        [BsonElementAttribute("ip")]
        public IPAddress Ip
        {
            get;
            private set;
        }

        [BsonElementAttribute("socket")]
        public int SocketNo
        {
            get;
            private set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChainedMarketDijitalTag.Models
{
    public class InfoUpdate
    {
        public InfoUpdate(DateTime date, string info, string userName)
        {
            Date = date;
            Info = info;
            UserName = userName;
        }

        public ObjectId id { get; set; }

        [BsonElementAttribute("date")]
        public DateTime Date { get; set; }

        [BsonElementAttribute("info")]
        public string Info { get; set; }

        [BsonElementAttribute("userName")]
        public string UserName { get; set; }
    }
}

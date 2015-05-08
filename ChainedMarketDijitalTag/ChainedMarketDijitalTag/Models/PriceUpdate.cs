using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChainedMarketDijitalTag.Models
{
    public class PriceUpdate
    {
        public PriceUpdate(DateTime date, double price,string userName)
        {
            Date = date;
            Price = price;
            UserName = userName;
        }

        public ObjectId id { get; set; }

        [BsonElementAttribute("date")]
        public DateTime Date { get; set; }

        [BsonElementAttribute("price")]
        public double Price { get; set; }

        [BsonElementAttribute("userName")]
        public string UserName { get; set; }
    }
}

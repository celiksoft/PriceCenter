using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChainedMarketDijitalTag.Models
{
    public class ImageUpdate
    {
        public ImageUpdate(DateTime date, string image, string userName)
        {
            Date = date;
            Image = image;
            UserName = userName;
        }

        public ObjectId id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] 
        [BsonElementAttribute("date")]
        public DateTime Date { get; set; }

        [BsonElementAttribute("image")]
        public string Image { get; set; }

        [BsonElementAttribute("userName")]
        public string UserName { get; set; }
    }
}

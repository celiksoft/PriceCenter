using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChainedMarketDijitalTag.Models
{
    public class User
    {
        public User(string userName, string password,string imagePath)
        {
            UserName = userName;
            Password = password;
            ImageSourcePath = imagePath;
        }

        public User() { }

        public ObjectId id { get; set; }

        [BsonElementAttribute("userName")]
        public string UserName { get; set; }

        [BsonElementAttribute("password")]
        public string Password { get; set; }

        [BsonElementAttribute("imagePath")]
        public string ImageSourcePath { get; set; }
    }
}

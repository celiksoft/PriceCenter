using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChainedMarketDijitalTag.Models
{
    public class SubCity
    {
        public SubCity(string name)
        {
            Name = name;
            MarketBranches = new ObservableCollection<MarketBranch>();
        }

        public SubCity()
        {
            Name = "";
            MarketBranches = new ObservableCollection<MarketBranch>();
        }

        public ObjectId id { get; set; }

        [BsonElementAttribute("name")]
        public string Name { get; private set; }

        [BsonElementAttribute("marketBranches")]
        public ObservableCollection<MarketBranch> MarketBranches { get; private set; }
    }
}


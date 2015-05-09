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
    public class Country
    {
        public Country(string name)
        {
            Name = name;
            Cities = new ObservableCollection<City>();
        }

        public Country()
        {
            Name = "";
            Cities = new ObservableCollection<City>();
        }

        public ObjectId id { get; set; }

        [BsonElementAttribute("name")]
        public string Name { get; private set; }

        [BsonElementAttribute("cities")]
        public ObservableCollection<City> Cities { get; private set; }
    }
}

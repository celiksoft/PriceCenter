using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ChainedMarketDijitalTag.Models
{
    public class City
    {
        public City(string name)
        {
            Name = name;
            MarketBranches = new ObservableCollection<MarketBranch>();
        }

        public City()
        {
            Name = "";
            MarketBranches = new ObservableCollection<MarketBranch>();
        }

        public string Name { get; private set; }
        public ObservableCollection<MarketBranch> MarketBranches { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        public string Name { get; private set; }
        public ObservableCollection<MarketBranch> MarketBranches { get; private set; }
    }
}


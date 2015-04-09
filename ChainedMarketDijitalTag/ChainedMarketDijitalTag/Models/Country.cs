using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        public string Name { get; private set; }
        public ObservableCollection<City> Cities { get; private set; }
    }
}

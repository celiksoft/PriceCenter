﻿using System;
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
            SubCities = new ObservableCollection<SubCity>();
        }

        public City()
        {
            Name = "";
            SubCities = new ObservableCollection<SubCity>();
        }

        public string Name { get; private set; }
        public ObservableCollection<SubCity> SubCities { get; private set; }
    }
}

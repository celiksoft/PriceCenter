using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ChainedMarketDijitalTag.Models
{
    public class PriceHistory
    {
        private Dictionary<DateTime, double> m_allPriceHistory;
        private ObservableCollection<DayPrice> m_dailyPrices;


        public PriceHistory()
        {

        }

        public void UpdatePrice(double newPrice)
        {
            m_allPriceHistory.Add(DateTime.Now, newPrice);
        }

        public Dictionary<DateTime, double> AllPriceHistory { get { return m_allPriceHistory; } }

        public ObservableCollection<DayPrice> DailyPrices
        {
            get { return m_dailyPrices; }

        }
    }
}

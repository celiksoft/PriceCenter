using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ChainedMarketDijitalTag.Helpers;

namespace ChainedMarketDijitalTag.Models
{
    public class Product : INotifyPropertyChanged
    {
        private static int m_productCount = 0;
        private readonly int m_id;
        private readonly int m_marketId;
        private string m_eslName;
        private string m_localServer;
        private string m_marketBranch;
        private string m_priceValue;
        private string m_imageValue;
        private string m_infoValue;
        private double m_currentPrice = 0.0;
        private ObservableCollection<PriceUpdate> m_monthlyPrices;
        private ObservableCollection<PriceUpdate> m_yearlyPrices;
        private ObservableCollection<PriceUpdate> m_historicalPrices;
        private Dictionary<DateTime, double> m_allPriceUpdates;
        private List<PriceUpdate> m_priceHistory;

        public event PropertyChangedEventHandler PropertyChanged;

        public Product(int marketId, string eslName, string localServer)
        {
            ++m_productCount;
            m_id = m_productCount;
            m_marketId = marketId;
            m_eslName = eslName;
            m_localServer = localServer;

            m_monthlyPrices = new ObservableCollection<PriceUpdate>();
            m_yearlyPrices = new ObservableCollection<PriceUpdate>();
            m_historicalPrices = new ObservableCollection<PriceUpdate>();
            m_allPriceUpdates = new Dictionary<DateTime, double>();
            m_priceHistory = new List<PriceUpdate>();

            // add test price history
            m_allPriceUpdates.Add(new DateTime(2013, 01, 10), 3.45);
            m_allPriceUpdates.Add(new DateTime(2013, 01, 15), 2.45);
            m_allPriceUpdates.Add(new DateTime(2013, 01, 20), 4.45);
            m_allPriceUpdates.Add(new DateTime(2013, 06, 10), 6.45);
            m_allPriceUpdates.Add(new DateTime(2013, 06, 20), 7.45);

            m_allPriceUpdates.Add(new DateTime(2014, 03, 22), 6.75);

            m_allPriceUpdates.Add(new DateTime(2015, 01, 10), 6.85);
            m_allPriceUpdates.Add(new DateTime(2015, 02, 22), 5.75);
            m_allPriceUpdates.Add(new DateTime(2015, 04, 20), 4.45);
            m_allPriceUpdates.Add(new DateTime(2015, 05, 02), 4.95);

            CurrentPrice = 4.95;

            FillPriceHistory();

        }

        public Product()
        {
        }

        public string Name
        {
            get { return m_eslName; }
        }

        public double CurrentPrice
        {
            get { return m_currentPrice; }
            set
            {
                m_currentPrice = value;
            }
        }

        public ObservableCollection<PriceUpdate> MonthlyPrices
        {
            get { return m_monthlyPrices; }
            set
            {
                if (m_monthlyPrices != value)
                {
                    m_monthlyPrices = value;
                    OnPropertyChanged("MonthlyPrices");
                }
            }
        }

        public ObservableCollection<PriceUpdate> YearlyPrices
        {
            get { return m_yearlyPrices; }
            set
            {
                if (m_yearlyPrices != value)
                {
                    m_yearlyPrices = value;
                    OnPropertyChanged("YearlyPrices");
                }
            }
        }

        public ObservableCollection<PriceUpdate> HistoricalPrices
        {
            get { return m_historicalPrices; }
            set
            {
                if (m_historicalPrices != value)
                {
                    m_historicalPrices = value;
                    OnPropertyChanged("HistoricalPrices");
                }
            }
        }


        public void UpdatePrice(PriceUpdate updateRequest)
        {
            if (updateRequest.Price == CurrentPrice)
                return;

            DateTime now = DateTime.Now;

            m_monthlyPrices.Add(updateRequest);

            m_allPriceUpdates.Add(updateRequest.Date, updateRequest.Price);

            YearlyPrices.ElementAt(now.Month - 1).Price = m_allPriceUpdates.Where(date => date.Key.Year == now.Year && date.Key.Month == now.Month).Average(value => value.Value);
            HistoricalPrices.Last().Price = m_allPriceUpdates.Where(date => date.Key.Year == now.Year).Average(value => value.Value);

            CurrentPrice = updateRequest.Price;
        }

        public void UpdateImage(ImageType newImage)
        {

        }

        public void UpdateInfo(string newInfo)
        {

        }

        private void FillPriceHistory()
        {
            if (m_allPriceUpdates != null && m_allPriceUpdates.Count != 0)
            {
                List<int> years = new List<int>();
                DateTime now = DateTime.Now;

                foreach (DateTime oldDate in m_allPriceUpdates.Keys)
                {
                    if (!years.Contains(oldDate.Year))
                    {
                        // fill all yearly avg prices
                        years.Add(oldDate.Year);
                        double yearAvgPrice = m_allPriceUpdates.Where(date => date.Key.Year == oldDate.Year).Average(value => value.Value);
                        HistoricalPrices.Add(new PriceUpdate(new DateTime(oldDate.Year, 1, 1), yearAvgPrice,"test"));

                        // fill all months average prices
                        if (now.Year == oldDate.Year)
                        {
                            double monthAvg = 0.0;
                            for (int i = 1; i <= now.Month; ++i)
                            {
                                var temp = m_allPriceUpdates.Where(date => date.Key.Year == oldDate.Year && date.Key.Month == i);

                                if (temp.Count() > 0)
                                {
                                    monthAvg = temp.Average(value => value.Value);
                                    YearlyPrices.Add(new PriceUpdate(new DateTime(oldDate.Year, i, 1), monthAvg,"test"));
                                }
                                else
                                {
                                    var lastChanges = m_allPriceUpdates.Where(date => date.Key.Year == oldDate.Year && date.Key.Month < i);
                                    if (lastChanges.Count() > 0)
                                        YearlyPrices.Add(new PriceUpdate(new DateTime(oldDate.Year, i, 1), lastChanges.Last().Value,"test"));
                                }
                            }

                            // for days
                            var lastMonthChanges = m_allPriceUpdates.Where(date => date.Key.Year == now.Year && date.Key.Month < now.Month);
                            if (lastMonthChanges.Count() > 0)
                                MonthlyPrices.Add(new PriceUpdate(new DateTime(now.Year, now.Month, 1), lastMonthChanges.Last().Value,"test"));

                            var currentMonthPrices = m_allPriceUpdates.Where(date => date.Key.Year == now.Year && date.Key.Month == now.Month);
                            if (currentMonthPrices.Count() > 0)
                            {
                                foreach (KeyValuePair<DateTime, double> pair in currentMonthPrices)
                                    MonthlyPrices.Add(new PriceUpdate(new DateTime(pair.Key.Year, pair.Key.Month, pair.Key.Day), pair.Value,"test"));
                            }

                            MonthlyPrices.Add(new PriceUpdate(new DateTime(now.Year, now.Month, now.Day), CurrentPrice,"test"));
                        }
                    }
                }

                years.Sort();
                int lastYear = years.Last();

                if (lastYear < now.Year)
                {
                    var lastChanges = m_allPriceUpdates.Where(date => date.Key.Year == lastYear);

                    for (int i = lastYear + 1; i <= now.Year; ++i)
                        HistoricalPrices.Add(new PriceUpdate(new DateTime(i, 1, 1), lastChanges.Last().Value,"test"));

                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

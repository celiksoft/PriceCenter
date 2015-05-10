using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ChainedMarketDijitalTag.Helpers;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChainedMarketDijitalTag.Models
{
    public class Product : INotifyPropertyChanged
    {
        private string m_eslName;
        private string m_localServer;
        private string m_currentImage = "";
        private string m_currentInfo = "";
        private double m_currentPrice = 0.0;

        private ObservableCollection<PriceUpdate> m_monthlyPrices;
        private ObservableCollection<PriceUpdate> m_yearlyPrices;
        private ObservableCollection<PriceUpdate> m_historicalPrices;
        private Dictionary<DateTime, double> m_allPriceUpdates;
        private ObservableCollection<PriceUpdate> m_priceHistory;
        private ObservableCollection<ImageUpdate> m_imageHistory;
        private ObservableCollection<InfoUpdate> m_infoHistory;

        public event PropertyChangedEventHandler PropertyChanged;

        public Product(string eslName, string localServer)
        {
            m_eslName = eslName;
            m_localServer = localServer;

            m_monthlyPrices = new ObservableCollection<PriceUpdate>();
            m_yearlyPrices = new ObservableCollection<PriceUpdate>();
            m_historicalPrices = new ObservableCollection<PriceUpdate>();
            m_allPriceUpdates = new Dictionary<DateTime, double>();
            m_priceHistory = new ObservableCollection<PriceUpdate>();
            m_imageHistory = new ObservableCollection<ImageUpdate>();
            m_infoHistory = new ObservableCollection<InfoUpdate>();
        }

        public Product()
        {
        }

        public ObjectId id { get; set; }

        [BsonElementAttribute("name")]
        public string Name
        {
            get { return m_eslName; }
            private set { m_eslName = value; }
        }

        [BsonElementAttribute("localServer")]
        public string LocalServer
        {
            get { return m_localServer; }
        }

        [BsonElementAttribute("price")]
        public double CurrentPrice
        {
            get { return m_currentPrice; }
            set
            {
                m_currentPrice = value;
            }
        }

        [BsonElementAttribute("imageCur")]
        public string CurrentImage
        {
            get { return m_currentImage; }
            set
            {
                m_currentImage = value;
            }
        }

        [BsonElementAttribute("infoCur")]
        public string CurrentInfo
        {
            get { return m_currentInfo; }
            set
            {
                m_currentInfo = value;
            }
        }

        [BsonElementAttribute("imageHistory")]
        public ObservableCollection<ImageUpdate> ImageHistory
        {
            get { return m_imageHistory; }
            set
            {
                if (m_imageHistory != value)
                {
                    m_imageHistory = value;
                    OnPropertyChanged("ImageHistory");
                }
            }
        }

        [BsonElementAttribute("infoHistory")]
        public ObservableCollection<InfoUpdate> InfoHistory
        {
            get { return m_infoHistory; }
            set
            {
                if (m_infoHistory != value)
                {
                    m_infoHistory = value;
                    OnPropertyChanged("InfoHistory");
                }
            }
        }

        [BsonElementAttribute("priceHistory")]
        public ObservableCollection<PriceUpdate> PriceHistory
        {
            get { return m_priceHistory; }
            set
            {
                if (m_priceHistory != value)
                {
                    m_priceHistory = value;
                    OnPropertyChanged("PriceHistory");
                    OnPropertyChanged("MonthlyPrices");
                    OnPropertyChanged("YearlyPrices");
                    OnPropertyChanged("HistoricalPrices");
                }
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
                    OnPropertyChanged("YearlyPrices");
                    OnPropertyChanged("HistoricalPrices");
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
            
            PriceHistory.Add(updateRequest);

            MonthlyPrices.Add(updateRequest);
            HistoricalPrices.Last().Price = PriceHistory.Where(pu => pu.Date.Year == updateRequest.Date.Year).Average(value => value.Price);
            YearlyPrices.ElementAt(updateRequest.Date.Month - 1).Price = PriceHistory.Where(pu => pu.Date.Year == updateRequest.Date.Year && pu.Date.Month == updateRequest.Date.Month).Average(value => value.Price);

            CurrentPrice = updateRequest.Price;
        }

        public void UpdateImage(ImageUpdate updateRequest)
        {
            if (updateRequest.Image == CurrentImage)
                return;


        }

        public void UpdateInfo(InfoUpdate updateRequest)
        {
            if (updateRequest.Info == CurrentInfo)
                return;
        }

        public void FillPriceHistory()
        {
            m_allPriceUpdates = new Dictionary<DateTime, double>();
            MonthlyPrices.Clear();
            YearlyPrices.Clear();
            HistoricalPrices.Clear();

            foreach (PriceUpdate update in PriceHistory)
                m_allPriceUpdates.Add(update.Date, update.Price);

            if (m_allPriceUpdates != null && m_allPriceUpdates.Count != 0)
            {
                List<int> years = new List<int>();
                DateTime now = DateTime.Now;

                foreach (DateTime newDate in m_allPriceUpdates.Keys)
                {
                    if (!years.Contains(newDate.Year))
                    {
                        // fill all yearly avg prices
                        years.Add(newDate.Year);

                        if (years.Count == 1)
                        {
                            double yearAvgPrice = m_allPriceUpdates.Where(date => date.Key.Year == newDate.Year).Average(value => value.Value);
                            HistoricalPrices.Add(new PriceUpdate(new DateTime(newDate.Year, 1, 1), yearAvgPrice, "test"));
                        }

                        if (years.Count() >= 2)
                        {
                            // year is increment by 1
                            double oldPriceLastYear = m_allPriceUpdates.Where(date => date.Key.Year == years.ElementAt(years.Count - 2)).Last().Value;

                            Dictionary<DateTime, double> copyAll = new Dictionary<DateTime, double>(m_allPriceUpdates);
                            copyAll.Add(new DateTime(newDate.Year, 1, 1), oldPriceLastYear);

                            double yearAvgPrice = copyAll.Where(date => date.Key.Year == newDate.Year).Average(value => value.Value);
                            HistoricalPrices.Add(new PriceUpdate(new DateTime(newDate.Year, 1, 1), yearAvgPrice, "test"));

                            // if there is no update for a year
                            int yearDif = newDate.Year - years.ElementAt(years.Count - 2);

                            if (yearDif > 1)
                            {
                                double oldPrice = m_allPriceUpdates.Where(date => date.Key.Year == years.ElementAt(years.Count - 2)).Last().Value;

                                for (int i = years.ElementAt(years.Count - 2) + 1; i < newDate.Year; ++i)
                                    HistoricalPrices.Add(new PriceUpdate(new DateTime(i, 1, 1), oldPrice, "test"));
                            }
                        }

                        // fill all months average prices for current year
                        if (now.Year == newDate.Year)
                        {
                            double monthAvg = 0.0;

                            for (int i = 1; i <= now.Month; ++i)
                            {
                                Dictionary<DateTime, double> copyAll = new Dictionary<DateTime, double>(m_allPriceUpdates);

                                if (i == 1)
                                {
                                    double oldPriceLastYear = m_allPriceUpdates.Where(date => date.Key.Year == years.ElementAt(years.Count - 2)).Last().Value;
                                    copyAll.Add(new DateTime(newDate.Year, 1, 1), oldPriceLastYear);
                                }

                                else
                                {
                                    double oldPriceLastMonth = m_allPriceUpdates.Where(date => date.Key.Year == now.Year && date.Key.Month == i - 1).Last().Value;
                                    copyAll.Add(new DateTime(newDate.Year, i, 1), oldPriceLastMonth);
                                }

                                var temp = copyAll.Where(date => date.Key.Year == newDate.Year && date.Key.Month == i);

                                if (temp.Count() > 0)
                                {
                                    monthAvg = temp.Average(value => value.Value);
                                    YearlyPrices.Add(new PriceUpdate(new DateTime(newDate.Year, i, 1), monthAvg, "test"));
                                }
                                else
                                {
                                    var lastChanges = m_allPriceUpdates.Where(date => date.Key.Year == newDate.Year && date.Key.Month < i);
                                    if (lastChanges.Count() > 0)
                                    {
                                        YearlyPrices.Add(new PriceUpdate(new DateTime(newDate.Year, i, 1), lastChanges.Last().Value, "test"));
                                    }
                                }
                            }

                            // for days in current month
                            var lastMonthChanges = m_allPriceUpdates.Where(date => date.Key.Year == now.Year && date.Key.Month < now.Month);
                            if (lastMonthChanges.Count() > 0)
                                MonthlyPrices.Add(new PriceUpdate(new DateTime(now.Year, now.Month, 1), lastMonthChanges.Last().Value, "test"));

                            var currentMonthPrices = m_allPriceUpdates.Where(date => date.Key.Year == now.Year && date.Key.Month == now.Month);
                            if (currentMonthPrices.Count() > 0)
                            {
                                foreach (KeyValuePair<DateTime, double> pair in currentMonthPrices)
                                    MonthlyPrices.Add(new PriceUpdate(new DateTime(pair.Key.Year, pair.Key.Month, pair.Key.Day), pair.Value, "test"));
                            }

                            MonthlyPrices.Add(new PriceUpdate(new DateTime(now.Year, now.Month, now.Day), currentMonthPrices.Last().Value, "test"));
                        }
                    }
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

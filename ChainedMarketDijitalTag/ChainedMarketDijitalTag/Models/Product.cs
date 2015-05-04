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
        private PriceHistory m_priceHistory;
        private double m_currentPrice;
        private ObservableCollection<DayPrice> m_dailyPrices;

        public event PropertyChangedEventHandler PropertyChanged;

        public Product(int marketId, string eslName, string localServer)
        {
            ++m_productCount;
            m_id = m_productCount;
            m_marketId = marketId;
            m_eslName = eslName;
            m_localServer = localServer;

            m_dailyPrices = new ObservableCollection<DayPrice>();
        }

        public Product()
        {
        }

        public string Name
        {
            get { return m_eslName; }
        }

        public PriceHistory PriceUpdateHistory { get { return m_priceHistory; } }

        public double CurrentPrice
        {
            get { return m_currentPrice; }
            set
            {
                m_currentPrice = value;
            }
        }

        public ObservableCollection<DayPrice> DailyPrices
        {
            get { return m_dailyPrices; }
            set
            {
                if (m_dailyPrices != value)
                {
                    m_dailyPrices = value;
                    OnPropertyChanged("DailyPrices");
                }
            }
        }

        public void UpdatePrice(double newPrice)
        {
            m_priceHistory.UpdatePrice(newPrice);
        }

        public void UpdateImage(ImageType newImage)
        {

        }

        public void UpdateInfo(string newInfo)
        {

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

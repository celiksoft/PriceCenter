using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using ChainedMarketDijitalTag.Models;
using ChainedMarketDijitalTag.Helpers;
using System.Windows.Controls;

using ChainedMarketDijitalTag.Messenger;

namespace ChainedMarketDijitalTag.ViewModels
{
    class UpdateInfoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Country> m_contries;
        private ObservableCollection<City> m_cities;
        private ObservableCollection<MarketBranch> m_marketBranches;
        private ObservableCollection<Product> m_products;
        private bool m_canUpdate;
        private Country m_selectedCountry = new Country();
        private City m_selectedCity = new City();
        private MarketBranch m_selectedMarketBranch = new MarketBranch();
        private Product m_selectedProduct = new Product();
        private UpdateType m_selectedInfoType;
        private string m_newInfoValue;
        private UpdateEventArgs m_updateEventArgs;

        public UpdateInfoViewModel()
        {
            #region TestData
            Product elma = new Product(1, "ELMA", "MANAV");
            Product kavurma = new Product(1, "KAVURMA", "KASAP");

            MarketBranch pendik = new MarketBranch("PENDİK", "TÜRKİYE", "İSTANBUL");
            MarketBranch yenisehir = new MarketBranch("YENİŞEHİR", "TÜRKİYE", "BURSA");

            MarketBranch hartz = new MarketBranch("HARTZ", "ALMANYA", "BERLİN");

            pendik.Products.Add(elma);
            pendik.Products.Add(kavurma);
            yenisehir.Products.Add(elma);
            hartz.Products.Add(elma);

            City bursa = new City("BURSA");
            City istanbul = new City("İSTANBUL");
            City berlin = new City("BERLİN");

            bursa.MarketBranches.Add(yenisehir);
            istanbul.MarketBranches.Add(pendik);
            berlin.MarketBranches.Add(hartz);

            Country turkiye = new Country("TÜRKİYE");
            Country almanya = new Country("ALMANYA");

            turkiye.Cities.Add(bursa);
            turkiye.Cities.Add(istanbul);
            almanya.Cities.Add(berlin);

            #endregion TestData

            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog, addAppLog);
            //Msg.UpdateInfoLog.Publish("Selected Country " + SelectedCountry );

            m_contries = new ObservableCollection<Country>();
            m_contries.Add(turkiye);
            m_contries.Add(almanya);

        }
        public bool CanUpdate
        {
            get { return m_canUpdate; }
            set
            {
                if (m_canUpdate != value)
                {
                    m_canUpdate = value;
                    OnPropertyChanged("CanUpdate");
                }
            }
        }

        public ObservableCollection<Country> Countries
        {
            get
            {
                return m_contries;
            }
            set
            {
                if (m_contries != value)
                {
                    m_contries = value;
                    OnPropertyChanged("Countries");
                    OnPropertyChanged("Cities");
                }
            }
        }

        public ObservableCollection<City> Cities
        {
            get
            {
                return m_selectedCountry.Cities;
            }
        }

        public ObservableCollection<MarketBranch> MarketBranches
        {
            get { return m_selectedCity.MarketBranches; }
        }

        public ObservableCollection<Product> Products
        {
            get { return m_selectedMarketBranch.Products; }
        }

        public Country SelectedCountry
        {
            get { return m_selectedCountry; }
            set
            {
                if (m_selectedCountry != value && value != null)
                {
                    m_selectedCountry = value;
                    OnPropertyChanged("SelectedCountry");
                    OnPropertyChanged("Cities");
                    OnPropertyChanged("MarketBranches");
                    OnPropertyChanged("Products");
                }
            }
        }

        public City SelectedCity
        {
            get { return m_selectedCity; }
            set
            {
                if (m_selectedCity != value && value != null)
                {
                    m_selectedCity = value;
                    OnPropertyChanged("SelectedCity");
                    OnPropertyChanged("Products");
                    OnPropertyChanged("MarketBranches");
                }
            }
        }

        public MarketBranch SelectedMarketBranch
        {
            get { return m_selectedMarketBranch; }
            set
            {
                if (m_selectedMarketBranch != value && value != null)
                {
                    m_selectedMarketBranch = value;
                    OnPropertyChanged("SelectedMarketBranch");
                    OnPropertyChanged("Products");
                }
            }
        }

        public Product SelectedProduct
        {
            get { return m_selectedProduct; }
            set
            {
                if (m_selectedProduct != value && value != null)
                {
                    m_selectedProduct = value;
                    OnPropertyChanged("SelectedProduct");
                }
            }
        }


        private void addAppLog(string log)
        {
            Msg.AppLog.Publish(log);
        }

        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

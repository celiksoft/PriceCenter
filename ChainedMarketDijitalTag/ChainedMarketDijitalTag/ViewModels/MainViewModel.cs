﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Collections.ObjectModel;

using ChainedMarketDijitalTag.Messenger;
using ChainedMarketDijitalTag.Helpers;
using ChainedMarketDijitalTag.Models;
using ChainedMarketDijitalTag.Services;

using SoftArcs.WPFSmartLibrary.MVVMCommands;
using SoftArcs.WPFSmartLibrary.MVVMCore;
using SoftArcs.WPFSmartLibrary.SmartUserControls;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Shared;

namespace ChainedMarketDijitalTag.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string m_appLogs;
        private string m_updateInfoLogs;
        private string m_otherLogs;
        private string m_validatedUserName;

        public MainViewModel()
        {
            // Login View Model initialization
            if (ViewModelHelper.IsInDesignModeStatic == false)
            {
                this.initializeAllCommands();
                this.getAllUser();
            }

            //#region TestData
            //Product elma = new Product("ELMA", "MANAV");
            //Product kavurma = new Product("KAVURMA", "KASAP");

            //// elma test prices
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2012, 02, 20), 1.55, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2012, 08, 20), 1.95, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2012, 11, 20), 1.85, m_validatedUserName));

            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2013, 02, 20), 2.15, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2013, 08, 20), 2.35, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2013, 11, 20), 2.55, m_validatedUserName));

            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2014, 02, 20), 2.45, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2014, 08, 20), 2.85, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2014, 11, 20), 2.95, m_validatedUserName));

            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 01, 10), 3.00, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 02, 20), 2.85, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 03, 20), 2.45, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 04, 20), 2.25, m_validatedUserName));
            //elma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 05, 02), 2.55, m_validatedUserName));

            //// kavurma test prices
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2012, 02, 20), 51.55, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2012, 08, 20), 53.95, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2012, 11, 20), 55.85, m_validatedUserName));

            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2013, 02, 20), 60.15, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2013, 08, 20), 58.35, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2013, 11, 20), 56.55, m_validatedUserName));

            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2014, 02, 20), 63.45, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2014, 08, 20), 64.85, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2014, 11, 20), 65.95, m_validatedUserName));

            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 01, 10), 70.00, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 02, 20), 71.85, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 03, 20), 75.45, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 04, 20), 73.25, m_validatedUserName));
            //kavurma.PriceHistory.Add(new PriceUpdate(new DateTime(2015, 05, 02), 78.55, m_validatedUserName));

            //MarketBranch merkez = new MarketBranch("MERKEZ", "TÜRKİYE", "İSTANBUL", "PENDİK");
            //MarketBranch sahil = new MarketBranch("SAHIL", "TÜRKİYE", "BURSA", "YENİSEHİR");
            //MarketBranch schoolstreet = new MarketBranch("SCHOOLSTREET", "ALMANYA", "BERLİN", "HERTZ");
            //MarketBranch carsi = new MarketBranch("CARSI", "TÜRKİYE", "ADIYAMAN", "MERKEZ");

            //merkez.Products.Add(elma);
            //merkez.Products.Add(kavurma);
            //sahil.Products.Add(kavurma);
            //sahil.Products.Add(elma);
            //schoolstreet.Products.Add(elma);
            //carsi.Products.Add(elma);
            //carsi.Products.Add(kavurma);

            //SubCity yenisehir = new SubCity("YENISEHIR");
            //SubCity pendik = new SubCity("PENDİK");
            //SubCity hertz = new SubCity("HERTZ");
            //SubCity adiMerkez = new SubCity("MERKEZ");

            //City bursa = new City("BURSA");
            //City istanbul = new City("İSTANBUL");
            //City berlin = new City("BERLİN");
            //City adiyaman = new City("ADIYAMAN");

            //yenisehir.MarketBranches.Add(sahil);
            //pendik.MarketBranches.Add(merkez);
            //hertz.MarketBranches.Add(schoolstreet);
            //adiMerkez.MarketBranches.Add(carsi);

            //bursa.SubCities.Add(yenisehir);
            //istanbul.SubCities.Add(pendik);
            //berlin.SubCities.Add(hertz);
            //adiyaman.SubCities.Add(adiMerkez);

            //Country turkiye = new Country("TÜRKİYE");
            //Country almanya = new Country("ALMANYA");

            //turkiye.Cities.Add(bursa);
            //turkiye.Cities.Add(istanbul);
            //turkiye.Cities.Add(adiyaman);
            //almanya.Cities.Add(berlin);

            //#endregion TestData

            m_contries = new ObservableCollection<Country>();
            //m_contries.Add(turkiye);
            //m_contries.Add(almanya);

            // connect to database to get user informations
            MongoServerSettings settings = new MongoServerSettings();
            MongoServer server = new MongoServer(settings);
            server.Connect();

            MongoDatabase db = server.GetDatabase("DigitalPriceCenter");

            var countries = db.GetCollection("Countries");

            foreach (Country country in countries.FindAllAs<Country>())
                m_contries.Add(country);

            //countries.Insert<Country>(turkiye);
            //countries.Insert<Country>(almanya);

            server.Disconnect();

            InfoTypes = new UpdateType[] { UpdateType.Price, UpdateType.Image, UpdateType.Info };
            addAppLog("Price manager is started");
            addAppLog("Price manager is ready to serve");

            Messenger<Msg>.Default.AddHandler<string>(Msg.UpdateInfoLog, addUpdateInfoLog);
        }

        public void Initialize()
        {
        }

        public string AppLogs
        {
            get { return m_appLogs; }
            set
            {
                if (m_appLogs != value)
                {
                    m_appLogs = value;
                    OnPropertyChanged("AppLogs");
                }
            }
        }

        public string UpdateInfoLogs
        {
            get { return m_updateInfoLogs; }
            set
            {
                if (m_updateInfoLogs != value)
                {
                    m_updateInfoLogs = value;
                    OnPropertyChanged("UpdateInfoLogs");
                }
            }
        }

        public string OtherLogs
        {
            get { return m_otherLogs; }
            set
            {
                if (m_otherLogs != value)
                {
                    m_otherLogs = value;
                    OnPropertyChanged("OtherLogs");
                }
            }
        }

        private void addAppLog(string log)
        {
            AppLogs = m_appLogs + log + Environment.NewLine;
        }

        private void addUpdateInfoLog(string log)
        {
            UpdateInfoLogs = m_updateInfoLogs + log + Environment.NewLine;
        }

        private void addOtherLog(string log)
        {
            OtherLogs = m_otherLogs + log + Environment.NewLine;
        }

        public void UpdateProductInfo()
        {
            // evaluete local server from db
            string localServer = m_selectedProduct.LocalServer;

            UpdateEventArgs args = new UpdateEventArgs(m_selectedType, m_updateInfoValue, localServer, m_selectedProduct.Name);

            // start a connection with market branch and wait request, Tcp client part
            AsynchronousClient tcpClient = new AsynchronousClient(m_selectedMarketBranch.TcpServiceInfo, args);
            bool response = tcpClient.StartClient();

            // according to response do smthg here 

            if (!response)
            {
                if (m_selectedType == UpdateType.Image)
                {
                    m_selectedProduct.UpdateImage(new ImageUpdate(DateTime.Now, args.NewValue, m_validatedUserName));
                    addUpdateInfoLog(string.Format("Product Image is updated to -> {0}", args.NewValue));
                }
                else if (m_selectedType == UpdateType.Info)
                {
                    m_selectedProduct.UpdateInfo(new InfoUpdate(DateTime.Now, args.NewValue, m_validatedUserName));
                    addUpdateInfoLog(string.Format("Product Info is updated to -> {0}", args.NewValue));
                }
                else if (m_selectedType == UpdateType.Price)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        double price;
                        if (double.TryParse(m_updateInfoValue, out price))
                            m_selectedProduct.UpdatePrice(new PriceUpdate(DateTime.Now, price, m_validatedUserName));
                    });

                    addUpdateInfoLog(string.Format("Product Price is updated to -> {0}", args.NewValue));
                }
            }
            
            // fail message
            else
            {
                if (m_selectedType == UpdateType.Image)
                {
                    addUpdateInfoLog(string.Format("SORRY : Product Image is not updated, TRY AGAIN!"));
                }
                else if (m_selectedType == UpdateType.Info)
                {
                    addUpdateInfoLog(string.Format("SORRY : Product Info is not updated, TRY AGAIN!"));
                }
                else if (m_selectedType == UpdateType.Price)
                {
                    addUpdateInfoLog(string.Format("SORRY : Product Price is not updated, TRY AGAIN!"));
                }
            }
        }

        #region LoginVM

        #region Fields

        List<User> userList;
        private readonly string userImagesPath = @"\Images";

        #endregion // Fields

        #region Public Properties

        public string UserName
        {
            get { return GetValue(() => UserName); }
            set
            {
                SetValue(() => UserName, value);

                this.UserImageSource = this.getUserImagePath();
            }
        }

        public string Password
        {
            get { return GetValue(() => Password); }
            set { SetValue(() => Password, value); }
        }

        public string UserImageSource
        {
            get { return GetValue(() => UserImageSource); }
            set { SetValue(() => UserImageSource, value); }
        }

        #endregion // Public Properties

        #region Submit Command Handler

        public ICommand SubmitCommand { get; private set; }

        private void ExecuteSubmit(object commandParameter)
        {
            var accessControlSystem = commandParameter as SmartLoginOverlay;

            if (accessControlSystem != null)
            {
                if (this.validateUser(this.UserName, this.Password) == true)
                {
                    accessControlSystem.Unlock();
                }
                else
                {
                    accessControlSystem.ShowWrongCredentialsMessage();
                }
            }
        }

        private bool CanExecuteSubmit(object commandParameter)
        {
            return !string.IsNullOrEmpty(this.Password);
        }

        #endregion // Submit Command Handler

        #region Private Methods

        private void initializeAllCommands()
        {
            this.SubmitCommand = new ActionCommand(this.ExecuteSubmit, this.CanExecuteSubmit);
        }

        private void getAllUser()
        {
            // connect to database to get user informations
            MongoServerSettings settings = new MongoServerSettings();
            MongoServer server = new MongoServer(settings);
            server.Connect();

            MongoDatabase db = server.GetDatabase("DigitalPriceCenter");

            var users = db.GetCollection("Users");

            this.userList = new List<User>();

            foreach (User user in users.FindAllAs<User>())
                this.userList.Add(user);

            server.Disconnect();
        }

        private bool validateUser(string username, string password)
        {
            //+ Here you would implement code, which will get the validation for the given credentials
            //+ from a database, a webservice or from somewhere else
            //! Remember : ONLY for demonstration purposes I have used a local Collection
            User validatedUser = this.userList.FirstOrDefault(user => user.UserName.Equals(username) &&
                                                                                user.Password.Equals(password));

            if (validatedUser != null)
            {
                m_validatedUserName = validatedUser.UserName;
                addAppLog(string.Format("Username : {0}", m_validatedUserName));
            }

            return validatedUser != null;
        }

        private string getUserImagePath()
        {
            User currentUser = this.userList.FirstOrDefault(user => user.UserName.Equals(this.UserName));

            if (currentUser != null)
            {
                return currentUser.ImageSourcePath;
            }

            return String.Empty;
        }

        #endregion

        #endregion

        #region UpdateInfoVM

        private ObservableCollection<Country> m_contries;
        private bool m_canUpdate;
        private Country m_selectedCountry;
        private City m_selectedCity;
        private SubCity m_selectedSubCity;
        private MarketBranch m_selectedMarketBranch;
        private Product m_selectedProduct;
        private string m_updateInfoValue;
        private UpdateEventArgs m_updateEventArgs;
        private UpdateType m_selectedType = UpdateType.Unknown;

        public string UpdateInfoValue
        {
            get { return m_updateInfoValue; }
            set
            {
                if (m_updateInfoValue != value)
                {
                    m_updateInfoValue = value;
                    OnPropertyChanged("UpdateInfoValue");
                    OnPropertyChanged("CanUpdate");
                }
            }
        }

        public bool CanUpdate
        {
            get { return m_selectedCountry != null && m_selectedCity != null && m_selectedSubCity != null && m_selectedMarketBranch != null && m_selectedProduct != null && m_selectedType != UpdateType.Unknown && m_updateInfoValue != null && m_updateInfoValue != ""; }
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
                    OnPropertyChanged("SubCities");
                    OnPropertyChanged("MarketBranches");
                    OnPropertyChanged("Products");
                }
            }
        }

        public ObservableCollection<City> Cities
        {
            get
            {
                if (m_selectedCountry != null)
                    return m_selectedCountry.Cities;
                return new ObservableCollection<City>();
            }
        }

        public ObservableCollection<SubCity> SubCities
        {
            get
            {
                if (m_selectedCity != null)
                    return m_selectedCity.SubCities;
                return new ObservableCollection<SubCity>();
            }
        }

        public ObservableCollection<MarketBranch> MarketBranches
        {
            get
            {
                if (m_selectedSubCity != null)
                    return m_selectedSubCity.MarketBranches;
                return new ObservableCollection<MarketBranch>();
            }
        }

        public ObservableCollection<Product> Products
        {
            get
            {
                if (m_selectedMarketBranch != null)
                    return m_selectedMarketBranch.Products;
                return new ObservableCollection<Product>();
            }
        }

        public Country SelectedCountry
        {
            get { return m_selectedCountry; }
            set
            {
                if (m_selectedCountry != value)
                {
                    m_selectedCountry = value;
                    Products.Clear();
                    OnPropertyChanged("SelectedCountry");
                    OnPropertyChanged("Cities");
                    OnPropertyChanged("SubCities");
                    OnPropertyChanged("MarketBranches");
                    OnPropertyChanged("Products");
                    OnPropertyChanged("CanUpdate");
                }
            }
        }

        public City SelectedCity
        {
            get { return m_selectedCity; }
            set
            {
                if (m_selectedCity != value)
                {
                    m_selectedCity = value;
                    Products.Clear();
                    OnPropertyChanged("SelectedCity");
                    OnPropertyChanged("SubCities");
                    OnPropertyChanged("Products");
                    OnPropertyChanged("CanUpdate");
                    OnPropertyChanged("MarketBranches");
                }
            }
        }

        public SubCity SelectedSubCity
        {
            get { return m_selectedSubCity; }
            set
            {
                if (m_selectedSubCity != value)
                {
                    m_selectedSubCity = value;
                    Products.Clear();
                    OnPropertyChanged("SelectedSubCity");
                    OnPropertyChanged("MarketBranches");
                    OnPropertyChanged("Products");
                    OnPropertyChanged("CanUpdate");
                }
            }
        }

        public MarketBranch SelectedMarketBranch
        {
            get { return m_selectedMarketBranch; }
            set
            {
                if (m_selectedMarketBranch != value)
                {
                    m_selectedMarketBranch = value;
                    OnPropertyChanged("SelectedMarketBranch");
                    OnPropertyChanged("CanUpdate");
                    OnPropertyChanged("Products");
                }
            }
        }

        public Product SelectedProduct
        {
            get { return m_selectedProduct; }
            set
            {
                if (m_selectedProduct != value)
                {
                    m_selectedProduct = value;
                    if (m_selectedProduct != null)
                        m_selectedProduct.FillPriceHistory();
                    OnPropertyChanged("CanUpdate");
                    OnPropertyChanged("SelectedProduct");
                }
            }
        }

        public UpdateType SelectedType
        {
            get { return m_selectedType; }
            set
            {
                if (m_selectedType != value)
                {
                    m_selectedType = value;
                    OnPropertyChanged("SelectedType");
                    OnPropertyChanged("CanUpdate");
                }
            }
        }

        public UpdateType[] InfoTypes
        {
            get;
            private set;
        }


        #endregion
    }
}

using System;
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
        private string m_orderLogs;
        private string m_validatedUserName;

        public MainViewModel()
        {
            // Login View Model initialization
            if (ViewModelHelper.IsInDesignModeStatic == false)
            {
                this.initializeAllCommands();

                //+ This is only neccessary if you want to display the appropriate image while typing the user name.
                //+ If you want a higher security level you wouldn't do this here !
                //! Remember : ONLY for demonstration purposes I have used a local Collection
                this.getAllUser();
            }

            #region TestData
            Product elma = new Product(1, "ELMA", "MANAV");
            Product kavurma = new Product(1, "KAVURMA", "KASAP");

            MarketBranch merkez = new MarketBranch("MERKEZ", "TÜRKİYE", "İSTANBUL", "PENDİK");
            MarketBranch sahil = new MarketBranch("SAHIL", "TÜRKİYE", "BURSA", "YENİSEHİR");
            MarketBranch schoolstreet = new MarketBranch("SCHOOLSTREET", "ALMANYA", "BERLİN", "HERTZ");

            merkez.Products.Add(elma);
            sahil.Products.Add(kavurma);
            sahil.Products.Add(elma);
            schoolstreet.Products.Add(elma);

            SubCity yenisehir = new SubCity("YENISEHIR");
            SubCity pendik = new SubCity("PENDİK");
            SubCity hertz = new SubCity("HERTZ");

            City bursa = new City("BURSA");
            City istanbul = new City("İSTANBUL");
            City berlin = new City("BERLİN");

            yenisehir.MarketBranches.Add(sahil);
            pendik.MarketBranches.Add(merkez);
            hertz.MarketBranches.Add(schoolstreet);

            bursa.SubCities.Add(yenisehir);
            istanbul.SubCities.Add(pendik);
            berlin.SubCities.Add(hertz);



            Country turkiye = new Country("TÜRKİYE");
            Country almanya = new Country("ALMANYA");

            turkiye.Cities.Add(bursa);
            turkiye.Cities.Add(istanbul);
            almanya.Cities.Add(berlin);

            #endregion TestData

            m_contries = new ObservableCollection<Country>();
            m_contries.Add(turkiye);
            m_contries.Add(almanya);

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

        public string OrderLogs
        {
            get { return m_orderLogs; }
            set
            {
                if (m_orderLogs != value)
                {
                    m_orderLogs = value;
                    OnPropertyChanged("OrderLogs");
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

        private void addOrderLog(string log)
        {
            OrderLogs = m_orderLogs + log + Environment.NewLine;
        }

        public void UpdateProductInfo()
        {
            // evaluete local server from db
            string localServer;
            if (m_selectedProduct.Name == "ELMA")
                localServer = "MANAV";
            else
                localServer = "KASAP";

            UpdateEventArgs args = new UpdateEventArgs(m_selectedType, m_updateInfoValue, localServer, m_selectedProduct.Name);

            // start a connection with market branch and wait request, Tcp client part
            AsynchronousClient tcpClient = new AsynchronousClient(m_selectedMarketBranch.TcpServiceInfo, args);
            tcpClient.StartClient();

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                double price;
                if (double.TryParse(m_updateInfoValue, out price))
                    m_selectedProduct.UpdatePrice(new PriceUpdate(DateTime.Now,price,m_validatedUserName));
            });
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
        private UpdateType m_selectedType;

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

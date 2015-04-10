using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System.Collections.ObjectModel;

using ChainedMarketDijitalTag.Messenger;
using ChainedMarketDijitalTag.Helpers;
using ChainedMarketDijitalTag.Models;

using SoftArcs.WPFSmartLibrary.MVVMCommands;
using SoftArcs.WPFSmartLibrary.MVVMCore;
using SoftArcs.WPFSmartLibrary.SmartUserControls;

namespace ChainedMarketDijitalTag.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string m_appLogs;
        private string m_updateInfoLogs;
        private string m_orderLogs;

        public MainViewModel()
        {
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

            m_contries = new ObservableCollection<Country>();
            m_contries.Add(turkiye);
            m_contries.Add(almanya);

            InfoTypes = new UpdateType[] { UpdateType.Price, UpdateType.Image, UpdateType.Info };

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
                    OnPropertyChanged("MarketTradeLogs");
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

        private void UpdateProductInfo()
        {   
            // evaluete local server from db
            string localServer;
            if (m_selectedProduct.Name == "ELMA")
                localServer = "MANAV";
            else
                localServer = "KASAP";

            UpdateEventArgs args = new UpdateEventArgs(m_selectedType, m_updateInfoValue, localServer, m_selectedProduct.Name);
            var updateMessage = CreateMessage(args);

            // start a connection with market branch and wait request, Tcp client part



        }

        private string CreateMessage(UpdateEventArgs args)
        {
            string updateType;

            if (args.Type == UpdateType.Price)
                updateType = "0";
            else if (args.Type == UpdateType.Image)
                updateType = "1";
            else if (args.Type == UpdateType.Info)
                updateType = "2";
            else
                updateType = "-1";

            string message = args.LocalServer + Definitions.OnPriceCenterRequestGUID + updateType + Definitions.OnPriceCenterRequestGUID +
                             args.Esl + Definitions.OnPriceCenterRequestGUID + args.NewValue + Definitions.OnPriceCenterRequestGUID + "<EOF>";
            return message;
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
            //+ Here you would implement code, which will get all user from a database,
            //+ a webservice or from somewhere else (if you want to display the right image)
            //! Remember : ONLY for demonstration purposes I have used a local Collection
            this.userList = new List<User>()
								 {
									new User() { UserName="celik", Password="celik1234",
													 ImageSourcePath = Path.Combine( userImagesPath, "user.png") },
                                    new User() { UserName="rcelik", Password="1234",
													 ImageSourcePath = Path.Combine( userImagesPath, "user.png") },
								 };
        }

        private bool validateUser(string username, string password)
        {
            //+ Here you would implement code, which will get the validation for the given credentials
            //+ from a database, a webservice or from somewhere else
            //! Remember : ONLY for demonstration purposes I have used a local Collection
            User validatedUser = this.userList.FirstOrDefault(user => user.UserName.Equals(username) &&
                                                                                user.Password.Equals(password));
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
        private Country m_selectedCountry = new Country();
        private City m_selectedCity = new City();
        private MarketBranch m_selectedMarketBranch = new MarketBranch();
        private Product m_selectedProduct = new Product();
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
                    OnPropertyChanged("MarketBranches");
                    OnPropertyChanged("Products");
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

        public UpdateType[] InfoTypes
        {
            get;
            private set;
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
                }
            }
        }

        #endregion 
    }
}

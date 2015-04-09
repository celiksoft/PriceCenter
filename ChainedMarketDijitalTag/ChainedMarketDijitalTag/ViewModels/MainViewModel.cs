using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;

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

        //// Declare the event 
        //public event PropertyChangedEventHandler PropertyChanged;
        //// Create the OnPropertyChanged method to raise the event 
        //protected void OnPropertyChanged(string name)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(name));
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

using SoftArcs.WPFSmartLibrary.MVVMCommands;
using SoftArcs.WPFSmartLibrary.MVVMCore;
using SoftArcs.WPFSmartLibrary.SmartUserControls;

using ChainedMarketDijitalTag.Models;
using ChainedMarketDijitalTag.Messenger;

namespace ChainedMarketDijitalTag.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        List<User> userList;
        private readonly string userImagesPath = @"\Images";

        #endregion // Fields

        #region Constructors

        public LoginViewModel()
        {
            if (ViewModelHelper.IsInDesignModeStatic == false)
            {
                this.initializeAllCommands();

                //+ This is only neccessary if you want to display the appropriate image while typing the user name.
                //+ If you want a higher security level you wouldn't do this here !
                //! Remember : ONLY for demonstration purposes I have used a local Collection
                this.getAllUser();
            }

            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog, addAppLog);
        }

        #endregion // Constructors

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

        private void addAppLog(string msg)
        {
            Msg.AppLog.Publish(msg);
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
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SoftArcs.WPFSmartLibrary.MVVMCommands;
using SoftArcs.WPFSmartLibrary.MVVMCore;
using SoftArcs.WPFSmartLibrary.SmartUserControls;

using ChainedMarketDijitalTag.ViewModels;

namespace ChainedMarketDijitalTag.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Fields

        public LoginViewModel ViewModel;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            this.ViewModel = new LoginViewModel();
            this.DataContext = this.ViewModel;
        }

        #endregion

        #region Event handler

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            this.SmartLoginOverlayControl.Lock();
        }

        #endregion
    }
}

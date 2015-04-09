using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using ChainedMarketDijitalTag.Messenger;


namespace ChainedMarketDijitalTag.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string m_appLogs;
        private string m_updateInfoLogs;
        private string m_orderLogs;

        public MainViewModel()
        {
            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog, addAppLog);
            addAppLog("App initialized");
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

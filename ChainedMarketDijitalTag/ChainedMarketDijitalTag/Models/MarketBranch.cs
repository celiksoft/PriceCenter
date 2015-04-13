using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net;

using ChainedMarketDijitalTag.Helpers;

namespace ChainedMarketDijitalTag.Models
{
    public class MarketBranch
    {   
        private static int m_marketCount = 0;
        private readonly int m_id;
        private string m_marketBranchLocation;
        private string m_country;
        private string m_city;
        private ObservableCollection<Product> m_products;
        private MarketBranchServerInfo m_tcpSocketInfo;      

        public MarketBranch(string name,string country,string city)
        {
            ++m_marketCount;
            m_id = m_marketCount + 1;
            m_marketBranchLocation = name;
            m_country = country;
            m_city = city;
            m_products = new ObservableCollection<Product>();
            m_tcpSocketInfo = new MarketBranchServerInfo(IPAddress.Parse("192.168.2.200"), 11000);
        }

        public ObservableCollection<Product> Products
        {
            get { return m_products; }
        }

        public string Name
        {
            get { return m_marketBranchLocation; }
            private set { m_marketBranchLocation = value; }
        }

        public string Country
        {
            get { return m_country; }
            private set { m_country = value; }
        }

        public string City
        {
            get { return m_city; }
            private set { m_city = value; }
        }

        public MarketBranchServerInfo TcpServiceInfo
        {
            get { return m_tcpSocketInfo; }
            private set { m_tcpSocketInfo = value; }
        }
    }
}

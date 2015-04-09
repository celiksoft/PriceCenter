using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainedMarketDijitalTag.Models
{
    public class Product
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

        public Product(int marketId,string eslName,string localServer)
        {
            ++m_productCount;
            m_id = m_productCount;
            m_marketId = marketId;
            m_eslName = eslName;
            m_localServer = localServer;
        }

        public Product()
        {
        }

        public string Name
        {
            get { return m_eslName; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainedMarketDijitalTag.Models
{
    public class DayPrice
    {
        public DayPrice(int dayNo, double price)
        {
            DayNo = dayNo;
            Price = price;
        }

        public int DayNo { get; set; }
        public double Price { get; set; }
    }
}

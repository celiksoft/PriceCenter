using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainedMarketDijitalTag.Models
{
    public class DatePrice
    {
        public DatePrice(DateTime date, double price)
        {
            Date = date;
            Price = price;
        }

        public DateTime Date { get; set; }
        public double Price { get; set; }
    }
}

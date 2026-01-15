using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class SalesReportDto
    {
        public int TotalOrders { get; set; }        
        public decimal TotalRevenue { get; set; }   
        public int TotalItemsSold { get; set; }     

        
        public decimal AverageOrderValue => TotalOrders > 0 ? Math.Round(TotalRevenue / TotalOrders, 2) : 0;
    }
}

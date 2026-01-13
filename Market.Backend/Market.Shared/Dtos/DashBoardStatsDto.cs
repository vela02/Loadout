using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class DashBoardStatsDto
    {
        public int TotalUsers { get; set; }
        public decimal TotalEarnings { get; set; }
        public int TotalLicensesSold { get; set; }
        public int ActivePreOrders { get; set; }
    }
}

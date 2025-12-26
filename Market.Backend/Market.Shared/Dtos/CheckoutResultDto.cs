using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class CheckoutResultDto
    {
        public int OrderId { get; set; }
        public string Message { get; set; } = null!;
        public bool IsPreOrder { get; set; }
    }
}

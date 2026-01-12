using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string Username { get; set; }=null!;
        public string Role { get; set; }=null!;
        public string RefreshToken { get; set; }=null!;
    }
}

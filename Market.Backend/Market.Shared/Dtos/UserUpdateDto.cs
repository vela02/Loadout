using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class UserUpdateDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool IsEnabled { get; set; }
        public string? Password { get; set; }

    }
}

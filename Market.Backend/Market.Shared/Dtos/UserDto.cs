using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public bool IsEnabled { get; set; }

    }
}

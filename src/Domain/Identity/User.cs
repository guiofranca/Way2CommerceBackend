using Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity;

public class User
{
    public string UserName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();

    public User(string userName, string email)
    {
        UserName = userName;
        Email = email;
    }
}

using Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity;

public class User
{
    public string Name { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }
}

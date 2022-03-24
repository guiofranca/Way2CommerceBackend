using Domain.Identity;
using Domain.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Mysql.Identity;

public class ApplicationUser : IdentityUser<Guid>, IGegenatesUser
{
    public string Name { get; set; } = String.Empty;
    public User GetUser()
    {
        return new User(Name, Email);
    }
}

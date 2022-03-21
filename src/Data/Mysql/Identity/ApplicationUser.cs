using Domain.Identity;
using Domain.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Mysql.Identity;

public class ApplicationUser : IdentityUser, IGegenatesUser
{
    public User GetUser()
    {
        return new User(UserName, Email);
    }
}

using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mysql.Context;
using Mysql.Identity;

namespace Mysql.Repositories;

public class UserRepository : IUserRepository<ApplicationUser>
{
    private readonly DataContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(DataContext dataContext, UserManager<ApplicationUser> userManager)
    {
        _db = dataContext;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _userManager
            .FindByIdAsync(id);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> EmailIsTakenAsync(string email, bool excludeOwnEmail = false)
    {
        var query = _db.Set<ApplicationUser>()
            .Where(u => u.Email == email);

        if (excludeOwnEmail) query = query.Where(u => u.Email != email);

        ApplicationUser? user =  await query
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return user != null;
    }

    public Task<bool> LogoutAsync(ApplicationUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationUser?> RegisterAsync(string name, string email, string password)
    {
        ApplicationUser user = new ApplicationUser()
        {
            Name = name,
            Email = email,
            UserName = email,
        };

        IdentityResult? result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded) return user;
        else return null;
    }

    public async Task<ApplicationUser?> TryLoginAsync(string email, string password)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return user;
        }

        if (await _userManager.CheckPasswordAsync(user, password))
        {
            return user;
        }

        return null;
    }

    public async Task<bool> AddRole(ApplicationUser user, string role)
    {
        var result = await _userManager.AddToRoleAsync(user, role);

        return result.Succeeded;
    }

    public async Task<bool> RemoveRole(ApplicationUser user, string role)
    {
        var result = await _userManager.RemoveFromRoleAsync(user, role);

        return result.Succeeded;
    }
}

using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mysql.Context;
using Mysql.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysql.Repositories;

public class UserRepository : IUserRepository<ApplicationUser>
{
    private readonly DataContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    //private readonly JWT _jwt;
    public UserRepository(DataContext dataContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = dataContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> EmailIsTakenAsync(string email, bool exclude = false)
    {
        var query = _db.Set<ApplicationUser>()
            .Where(u => u.Email == email);

        if (exclude) query = query.Where(u => u.Email != email);

        ApplicationUser? user =  await query
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return user != null;
    }

    public Task<bool> LogoutAsync(ApplicationUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationUser?> RegisterAsync(string userName, string email, string password)
    {
        ApplicationUser user = new ApplicationUser()
        {
            UserName = userName,
            Email = email,
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

namespace Domain.Repositories.Interfaces;

public interface IUserRepository<TUser> where TUser : class
{
    public Task<TUser?> GetByIdAsync(string id);
    public Task<IEnumerable<string>> GetUserRolesAsync(TUser user);
    public Task<TUser?> TryLoginAsync(string email, string password);
    public Task<bool> EmailIsTakenAsync(string email, bool exclude = false);
    public Task<TUser?> RegisterAsync(string name, string email, string password);
    public Task<bool> LogoutAsync(TUser user);
    public Task<bool> AddRole(TUser user, string role);
    public Task<bool> RemoveRole(TUser user, string role);
}

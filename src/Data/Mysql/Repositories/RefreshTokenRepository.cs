using Microsoft.EntityFrameworkCore;
using Mysql.Context;
using Mysql.Identity;

namespace Mysql.Repositories;

public class RefreshTokenRepository
{
    protected readonly DataContext _db;
    public RefreshTokenRepository(DataContext dbContext)
    {
        _db = dbContext;
    }

    public async Task<RefreshToken> GetByIdAndUser(Guid id, ApplicationUser user)
    {
        RefreshToken? refreshToken = await _db.Set<RefreshToken>()
            .Where(r => r.Id == id)
            .Where(r => r.UserId == user.Id)
            .Where(r => r.Expiry > DateTime.Now)
            .FirstOrDefaultAsync();

        if (refreshToken == null) throw new Exception($"Refresh token {id} not found");

        return refreshToken;
    }

    public async Task<bool> Create(RefreshToken refreshToken)
    {
        _db.Set<RefreshToken>()
            .Add(refreshToken);

        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> Remove(RefreshToken refreshToken)
    {
        _db.Set<RefreshToken>().Remove(refreshToken);

        return await _db.SaveChangesAsync() > 0;
    }

    public async Task RemoveAllRefreshTokensFromUser(ApplicationUser user)
    {
        var refreshTokens = _db.Set<RefreshToken>()
            .Where(refreshToken => refreshToken.UserId == user.Id)
            .ToList();
        
        _db.Set<RefreshToken>().RemoveRange(refreshTokens);

        await _db.SaveChangesAsync();
    }
}

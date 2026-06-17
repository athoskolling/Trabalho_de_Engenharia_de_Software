using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.IRepositories;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId)
            .ToListAsync();

        if (tokens.Any())
        {
            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}

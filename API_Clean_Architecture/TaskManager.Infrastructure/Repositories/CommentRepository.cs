using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.IRepositories;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Comment>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.Comments
            .Include(comment => comment.User)
            .Where(comment => comment.TaskItemId == taskId)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _context.Comments
            .Include(comment => comment.User)
            .FirstOrDefaultAsync(comment => comment.Id == id);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var comment = await GetByIdAsync(id);

        if (comment is null) return false;

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync();

        return true;
    }
}
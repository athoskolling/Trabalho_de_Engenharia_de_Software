using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.IRepositories;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include(task => task.CreatedBy) //o include faz com que o usuário que criou a task venha junto na requisição mesmo que o CreatedById esteja na tabela de Tasks, o include faz com que o objeto User completo venha junto
            .Include(task => task.AssignedTo)
            .FirstOrDefaultAsync(task => task.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
            .Include(task => task.CreatedBy)
            .Include(task => task.AssignedTo)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByAssignedUserAsync(Guid userId)
    {
        return await _context.Tasks
            .Where(task => task.AssignedToId == userId)
            .ToListAsync();
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();

        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);

        await _context.SaveChangesAsync();

        return task;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await GetByIdAsync(id);

        if (task is null) return false;

        task.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}
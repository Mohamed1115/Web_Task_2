using Microsoft.EntityFrameworkCore;
using Task2.Data;

namespace Task2.Repositories;

public class GenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context ;
    public GenericRepository(ApplicationDbContext context)
    {_context=context;}

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cn)
    {
        return await _context.Set<T>().ToListAsync(cn);
    }

    public async Task<T> GetByIdAsync(int id, CancellationToken cn)
    {
        return await _context.Set<T>().FindAsync(new object[]{id},cn);
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cn)
    {
        await _context.Set<T>().AddAsync(entity,cn);
        await _context.SaveChangesAsync(cn);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cn)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cn);
    }

    public async Task DeleteAsync(T entity, CancellationToken cn)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cn);
    }
    
    
}
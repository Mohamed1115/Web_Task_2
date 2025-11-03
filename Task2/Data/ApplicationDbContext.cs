using Microsoft.EntityFrameworkCore;
using Task2.Models;

namespace Task2.Data;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    DbSet<Todo>Todos{get;set;}
    
}
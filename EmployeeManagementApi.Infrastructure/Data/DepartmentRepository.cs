using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmployeeManagementApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagementApi.Infrastructure.Data;

public class DepartmentRepository : IRepository<Department>
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "Departments";

    public DepartmentRepository(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Department> GetByIdAsync(int id)
    {
        var departments = await GetAllAsync();
        return departments.FirstOrDefault(d => d.Id == id) ?? throw new KeyNotFoundException("Department not found");
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        if (!_cache.TryGetValue(CacheKey, out IEnumerable<Department> departments))
        {
            departments = await _context.Departments.ToListAsync();
            _cache.Set(CacheKey, departments, TimeSpan.FromMinutes(30));
        }
        return departments;
    }

    public async Task AddAsync(Department entity)
    {
        await _context.Departments.AddAsync(entity);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
    }

    public async Task UpdateAsync(Department entity)
    {
        _context.Departments.Update(entity);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        _context.Departments.Remove(entity);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
    }
}
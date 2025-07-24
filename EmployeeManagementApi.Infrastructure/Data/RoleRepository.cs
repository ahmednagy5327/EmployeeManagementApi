using EmployeeManagementApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagementApi.Infrastructure.Data;

public class RoleRepository : IRepository<Role>
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "Roles";

    public RoleRepository(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Role> GetByIdAsync(int id)
    {
        var roles = await GetAllAsync();
        return roles.FirstOrDefault(r => r.Id == id) ?? throw new KeyNotFoundException("Role not found");
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        if (!_cache.TryGetValue(CacheKey, out IEnumerable<Role> roles))
        {
            roles = await _context.Roles.ToListAsync();
            _cache.Set(CacheKey, roles, TimeSpan.FromMinutes(30));
        }
        return roles;
    }

    public async Task AddAsync(Role entity)
    {
        await _context.Roles.AddAsync(entity);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
    }

    public async Task UpdateAsync(Role entity)
    {
        _context.Roles.Update(entity);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        _context.Roles.Remove(entity);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
    }
}
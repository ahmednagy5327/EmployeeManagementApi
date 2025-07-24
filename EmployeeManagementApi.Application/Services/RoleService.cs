using EmployeeManagementApi.Application.Dtos;
using EmployeeManagementApi.Domain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementApi.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IMemoryCache _cache;

    public RoleService(IRepository<Role> roleRepository, IMemoryCache cache)
    {
        _roleRepository = roleRepository;
        _cache = cache;
    }

    public async Task<RoleDto> GetRoleByIdAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return MapToDto(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(MapToDto);
    }

    public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto, string userId)
    {
        var role = new Role
        {
            Name = roleDto.Name,
            Permissions = roleDto.Permissions,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow
        };

        await _roleRepository.AddAsync(role);
        return MapToDto(role);
    }

    public async Task UpdateRoleAsync(int id, RoleDto roleDto, string userId)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        role.Name = roleDto.Name;
        role.Permissions = roleDto.Permissions;
        role.UpdatedBy = userId;
        role.UpdatedAt = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(role);
    }

    public async Task DeleteRoleAsync(int id)
    {
        await _roleRepository.DeleteAsync(id);
    }

    private RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Permissions = role.Permissions
        };
    }
}
using EmployeeManagementApi.Application.Dtos;

namespace EmployeeManagementApi.Application.Services;

public interface IRoleService
{
    Task<RoleDto> GetRoleByIdAsync(int id);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto> CreateRoleAsync(RoleDto roleDto, string userId);
    Task UpdateRoleAsync(int id, RoleDto roleDto, string userId);
    Task DeleteRoleAsync(int id);
}
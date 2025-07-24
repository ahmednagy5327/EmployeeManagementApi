using EmployeeManagementApi.Application.Dtos;

namespace EmployeeManagementApi.Application.Services;

public interface IDepartmentService
{
    Task<DepartmentDto> GetDepartmentByIdAsync(int id);
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
    Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto, string userId);
    Task UpdateDepartmentAsync(int id, DepartmentDto departmentDto, string userId);
    Task DeleteDepartmentAsync(int id);
}
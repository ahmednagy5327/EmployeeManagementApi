using EmployeeManagementApi.Application.Dtos;

namespace EmployeeManagementApi.Application.Services;

public interface IEmployeeService
{
    Task<EmployeeDto> GetEmployeeByIdAsync(int id);
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string? name, int? departmentId, bool? isActive, DateTime? startDate, DateTime? endDate);
    Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto, string userId);
    Task UpdateEmployeeAsync(int id, EmployeeDto employeeDto, string userId);
    Task DeleteEmployeeAsync(int id);
    Task ActivateEmployeeAsync(int id, string userId);
    Task DeactivateEmployeeAsync(int id, string userId);
}
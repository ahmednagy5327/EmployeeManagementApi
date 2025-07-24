using EmployeeManagementApi.Application.Dtos;
using EmployeeManagementApi.Domain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementApi.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Department> _departmentRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IMemoryCache _cache;

    public EmployeeService(
        IRepository<Employee> employeeRepository,
        IRepository<Department> departmentRepository,
        IRepository<Role> roleRepository,
        IMemoryCache cache)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _roleRepository = roleRepository;
        _cache = cache;
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            throw new InvalidOperationException("Employee repository returned null.");
        return MapToDto(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        if (employees == null)
            throw new InvalidOperationException("Employee repository returned null.");
        return employees.Select(MapToDto);
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string? name, int? departmentId, bool? isActive, DateTime? startDate, DateTime? endDate)
    {
        var employees = await _employeeRepository.GetAllAsync();
        if (employees == null)
            throw new InvalidOperationException("Employee repository returned null.");
        var query = employees.AsQueryable();

        if (!string.IsNullOrEmpty(name))
            query = query.Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        if (isActive.HasValue)
            query = query.Where(e => e.IsActive == isActive.Value);

        if (startDate.HasValue)
            query = query.Where(e => e.DateOfJoining >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.DateOfJoining <= endDate.Value);

        return query.Select(MapToDto);
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto, string userId)
    {
        await ValidateEmployeeAsync(employeeDto);

        var employee = new Employee
        {
            Name = employeeDto.Name,
            Email = employeeDto.Email,
            Phone = employeeDto.Phone,
            DepartmentId = employeeDto.DepartmentId,
            RoleId = employeeDto.RoleId,
            DateOfJoining = employeeDto.DateOfJoining,
            IsActive = employeeDto.IsActive,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);
        return MapToDto(employee);
    }

    public async Task UpdateEmployeeAsync(int id, EmployeeDto employeeDto, string userId)
    {
        await ValidateEmployeeAsync(employeeDto);

        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} was not found."); employee.Name = employeeDto.Name;
        employee.Email = employeeDto.Email;
        employee.Phone = employeeDto.Phone;
        employee.DepartmentId = employeeDto.DepartmentId;
        employee.RoleId = employeeDto.RoleId;
        employee.DateOfJoining = employeeDto.DateOfJoining;
        employee.IsActive = employeeDto.IsActive;
        employee.UpdatedBy = userId;
        employee.UpdatedAt = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(employee);
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        await _employeeRepository.DeleteAsync(id);
    }

    public async Task ActivateEmployeeAsync(int id, string userId)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} was not found."); employee.IsActive = true;
        employee.UpdatedBy = userId;
        employee.UpdatedAt = DateTime.UtcNow;
        await _employeeRepository.UpdateAsync(employee);
    }

    public async Task DeactivateEmployeeAsync(int id, string userId)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} was not found.");

        employee.IsActive = false;
        employee.UpdatedBy = userId;
        employee.UpdatedAt = DateTime.UtcNow;
        await _employeeRepository.UpdateAsync(employee);
    }


    private async Task ValidateEmployeeAsync(EmployeeDto employeeDto)
    {
        var department = await _departmentRepository.GetByIdAsync(employeeDto.DepartmentId);
        if (department == null)
            throw new KeyNotFoundException("Department not found");

        var role = await _roleRepository.GetByIdAsync(employeeDto.RoleId);
        if (role == null)
            throw new KeyNotFoundException("Role not found");
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Phone = employee.Phone,
            DepartmentId = employee.DepartmentId,
            RoleId = employee.RoleId,
            DateOfJoining = employee.DateOfJoining,
            IsActive = employee.IsActive
        };
    }
}
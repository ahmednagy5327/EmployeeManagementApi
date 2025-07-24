using EmployeeManagementApi.Application.Dtos;
using EmployeeManagementApi.Domain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementApi.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IMemoryCache _cache;

    public DepartmentService(IRepository<Department> departmentRepository, IMemoryCache cache)
    {
        _departmentRepository = departmentRepository;
        _cache = cache;
    }

    public async Task<DepartmentDto> GetDepartmentByIdAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        return MapToDto(department);
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();
        return departments.Select(MapToDto);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto, string userId)
    {
        var department = new Department
        {
            Name = departmentDto.Name,
            Description = departmentDto.Description,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow
        };

        await _departmentRepository.AddAsync(department);
        return MapToDto(department);
    }

    public async Task UpdateDepartmentAsync(int id, DepartmentDto departmentDto, string userId)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        department.Name = departmentDto.Name;
        department.Description = departmentDto.Description;
        department.UpdatedBy = userId;
        department.UpdatedAt = DateTime.UtcNow;

        await _departmentRepository.UpdateAsync(department);
    }

    public async Task DeleteDepartmentAsync(int id)
    {
        await _departmentRepository.DeleteAsync(id);
    }

    private DepartmentDto MapToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description
        };
    }
}
using EmployeeManagementApi.Application.Dtos;
using EmployeeManagementApi.Application.Services;
using EmployeeManagementApi.Domain;
using EmployeeManagementApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementApi.Tests;

[TestClass]
public class EmployeeServiceTests
{
    private AppDbContext _context;
    private EmployeeService _employeeService;

    [TestInitialize]
    public void Setup()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        // Seed minimal required data for validation
        _context.Departments.Add(new Department { Id = 1, Name = "IT", CreatedBy = "System", CreatedAt = DateTime.UtcNow, UpdatedBy = "System", UpdatedAt = DateTime.UtcNow });
        _context.Roles.Add(new Role { Id = 1, Name = "Admin", Permissions = "{}", CreatedBy = "System", CreatedAt = DateTime.UtcNow, UpdatedBy = "System", UpdatedAt = DateTime.UtcNow });
        _context.SaveChanges();

        // Initialize memory cache
        var cache = new MemoryCache(new MemoryCacheOptions());

        // Set up mocks for repositories
        var employeeRepositoryMock = new Mock<IRepository<Employee>>();
        employeeRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => _context.Employees.FirstOrDefault(e => e.Id == id));
        employeeRepositoryMock?.Setup(r => r.GetAllAsync()).ReturnsAsync(() => _context.Employees.ToList());
        employeeRepositoryMock?.Setup(r => r.AddAsync(It.IsAny<Employee>())).Callback<Employee>(e => _context.Employees.Add(e)).Returns(Task.CompletedTask);
        employeeRepositoryMock?.Setup(r => r.UpdateAsync(It.IsAny<Employee>())).Callback<Employee>(e =>
        {
            var existing = _context.Employees.FirstOrDefault(x => x.Id == e.Id);
            if (existing != null) _context.Entry(existing).CurrentValues.SetValues(e);
        }).Returns(Task.CompletedTask);
        employeeRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).Callback<int>(id => _context.Employees.Remove(_context.Employees.FirstOrDefault(e => e.Id == id))).Returns(Task.CompletedTask);

        var departmentRepositoryMock = new Mock<IRepository<Department>>();
        departmentRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => _context.Departments.FirstOrDefault(d => d.Id == id));
        var roleRepositoryMock = new Mock<IRepository<Role>>();
        roleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => _context.Roles.FirstOrDefault(r => r.Id == id));

        // Initialize EmployeeService
        _employeeService = new EmployeeService(
            employeeRepositoryMock.Object,
            departmentRepositoryMock.Object,
            roleRepositoryMock.Object,
            cache
        );
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public async Task CreateEmployeeAsync_ValidDto_ReturnsCreatedEmployee()
    {
        // Arrange
        var employeeDto = new EmployeeDto
        {
            Name = "Test Employee",
            Email = "test@example.com",
            Phone = "+1234567890",
            DepartmentId = 1, // Matches seeded DepartmentId
            RoleId = 1,      // Matches seeded RoleId
            DateOfJoining = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _employeeService.CreateEmployeeAsync(employeeDto, "TestUser");

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.AreEqual(employeeDto.Name, result.Name, "Name should match");
        Assert.AreEqual(employeeDto.Email, result.Email, "Email should match");
        Assert.AreEqual(employeeDto.Phone, result.Phone, "Phone should match");
        Assert.AreEqual(employeeDto.DepartmentId, result.DepartmentId, "DepartmentId should match");
        Assert.AreEqual(employeeDto.RoleId, result.RoleId, "RoleId should match");
        Assert.AreEqual(employeeDto.IsActive, result.IsActive, "IsActive should match");
        Assert.IsTrue(result.Id > 0, "Id should be assigned");
    }
}
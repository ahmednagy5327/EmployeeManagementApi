using EmployeeManagementApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EmployeeManagementApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Role> Roles { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasOne<Department>(e => e.Department)
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne<Role>(e => e.Role)
            .WithMany()
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT", Description = "Information Technology", CreatedBy = "System", CreatedAt = DateTime.UtcNow, UpdatedBy = "System", UpdatedAt = DateTime.UtcNow },
            new Department { Id = 2, Name = "HR", Description = "Human Resources", CreatedBy = "System", CreatedAt = DateTime.UtcNow, UpdatedBy = "System", UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Permissions = "{\"CanManageUsers\": true, \"CanViewReports\": true}", CreatedBy = "System", CreatedAt = DateTime.UtcNow, UpdatedBy = "System", UpdatedAt = DateTime.UtcNow },
            new Role { Id = 2, Name = "HR", Permissions = "{\"CanManageUsers\": true, \"CanViewReports\": false}", CreatedBy = "System", CreatedAt = DateTime.UtcNow, UpdatedBy = "System", UpdatedAt = DateTime.UtcNow },
            new Role { Id = 3, Name = "Viewer", Permissions = "{\"CanManageUsers\": false, \"CanViewReports\": true}", CreatedBy = "System", CreatedAt = DateTime.UtcNow, UpdatedBy = "System", UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                Name = "Admin User",
                Email = "admin@example.com",
                Phone = "+1234567890",
                DepartmentId = 1,
                RoleId = 1,
                DateOfJoining = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = "System",
                UpdatedAt = DateTime.UtcNow
            }
        );
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../EmployeeManagementApi"));
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            optionsBuilder.EnableSensitiveDataLogging(); // Temporary for debugging

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
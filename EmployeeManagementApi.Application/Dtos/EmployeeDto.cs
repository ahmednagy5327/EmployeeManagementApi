namespace EmployeeManagementApi.Application.Dtos;

public class EmployeeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int RoleId { get; set; }
    public DateTime DateOfJoining { get; set; }
    public bool IsActive { get; set; }
}
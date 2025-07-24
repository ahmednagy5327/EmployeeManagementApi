namespace EmployeeManagementApi.Application.Dtos;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
}
using EmployeeManagementApi.Application.Dtos;
using FluentValidation;

namespace EmployeeManagementApi.Application.Validators;

public class EmployeeValidator : AbstractValidator<EmployeeDto>
{
    public EmployeeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?\d{10,15}$");
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.RoleId).GreaterThan(0);
        RuleFor(x => x.DateOfJoining).NotEmpty();
    }
}
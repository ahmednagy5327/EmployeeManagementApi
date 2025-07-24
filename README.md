Employee Management API
Overview
This is a .NET 8 Web API implementing an Employee Management system using Clean Architecture. It includes CRUD operations for Employees, Departments, and Roles, with search functionality, JWT authentication, role-based authorization, in-memory caching, and unit tests.
Architecture

Domain: Entities (Employee, Department, Role) and repository interfaces.
Application: DTOs, validators, and service interfaces.
Infrastructure: EF Core with SQL Server, in-memory caching, and repositories.
WebApi: REST controllers, JWT authentication, Swagger, and exception handling.
Tests: Unit tests for core services.

Key Decisions

Clean Architecture: Ensures separation of concerns and maintainability.
In-Memory Caching: Used for Departments and Roles to improve performance.
JWT Authentication: Secures endpoints with role-based authorization (Admin, HR, Viewer).
FluentValidation: For robust input validation.
EF Core Code First: Simplifies database setup with migrations and seeding.
Unit Tests: Basic tests for core services using MSTest.

Prerequisites

.NET 8 SDK
SQL Server
Visual Studio 2022

Setup Instructions

Clone the Repository:git clone <repository-url>


Install NuGet Packages:Open Package Manager Console in Visual Studio and run:Install-Package FluentValidation.AspNetCore -Version 11.10.0 -ProjectName EmployeeManagementApi.Application
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.10 -ProjectName EmployeeManagementApi.Infrastructure
Install-Package Microsoft.EntityFrameworkCore.Design -Version 8.0.10 -ProjectName EmployeeManagementApi.Infrastructure
Install-Package Microsoft.Extensions.Caching.Memory -Version 8.0.1 -ProjectName EmployeeManagementApi.Infrastructure
Install-Package Microsoft.EntityFrameworkCore.Design -Version 8.0.10 -ProjectName EmployeeManagementApi.WebApi
Install-Package Microsoft.Extensions.Caching.Memory -Version 8.0.1 -ProjectName EmployeeManagementApi.WebApi
Install-Package Microsoft.Extensions.Logging.Abstractions -Version 8.0.2 -ProjectName EmployeeManagementApi.WebApi
Install-Package FluentValidation.AspNetCore -Version 11.10.0 -ProjectName EmployeeManagementApi.WebApi
Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 8.0.10 -ProjectName EmployeeManagementApi.WebApi
Install-Package Swashbuckle.AspNetCore -Version 6.8.1 -ProjectName EmployeeManagementApi.WebApi
Install-Package Microsoft.VisualStudio.TestTools.UnitTesting -Version 17.11.1 -ProjectName EmployeeManagementApi.Tests
Install-Package Moq -Version 4.20.70 -ProjectName EmployeeManagementApi.Tests


Setup Database:
Update the connection string in appsettings.json to point to your SQL Server instance.
Run the provided CreateDatabase.sql script in SQL Server Management Studio.
Alternatively, apply migrations:dotnet ef migrations add InitialCreate
dotnet ef database update




Run the API:
Set EmployeeManagementApi.WebApi as the startup project.
Run in Visual Studio (F5) or via CLI:dotnet run --project EmployeeManagementApi.WebApi




Access Swagger:
Navigate to https://localhost:<port>/swagger for API documentation.



Sample JWT Token
For testing, generate a JWT token with the following claims (use a JWT tool like jwt.io):

Issuer: EmployeeManagementApi
Audience: EmployeeManagementApi
Key: YourSuperSecretKey1234567890ABCDEFGHIJ
Claims:
sub: admin@example.com
role: Admin (or HR, Viewer)



Example token (for Admin):
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJFbXBsb3llZU1hbmFnZW1lbnRBcGkiLCJhdWQiOiJFbXBsb3llZU1hbmFnZW1lbnRBcGkiLCJzdWIiOiJhZG1pbkBleGFtcGxlLmNvbSIsInJvbGUiOiJBZG1pbiIsImV4cCI6MTc2NDUwMDAwMH0.UabM5z8wFKjOYjT2n7QymmvWoWkba4ZJdO1vXjPZdAM
API Endpoints

Employees:
GET /api/Employees: List all employees (Viewer)
GET /api/Employees/{id}: Get employee by ID (Viewer)
GET /api/Employees/search?name={name}&departmentId={id}&isActive={bool}&startDate={date}&endDate={date}: Search employees (Viewer)
POST /api/Employees: Create employee (Admin)
PUT /api/Employees/{id}: Update employee (Admin)
DELETE /api/Employees/{id}: Delete employee (Admin)
PUT /api/Employees/{id}/activate: Activate employee (HR)
PUT /api/Employees/{id}/deactivate: Deactivate employee (HR)


Departments:
GET /api/Departments: List all departments (Viewer)
GET /api/Departments/{id}: Get department by ID (Viewer)
POST /api/Departments: Create department (Admin)
PUT /api/Departments/{id}: Update department (Admin)
DELETE /api/Departments/{id}: Delete department (Admin)


Roles:
GET /api/Roles: List all roles (Viewer)
GET /api/Roles/{id}: Get role by ID (Viewer)
POST /api/Roles: Create role (Admin)
PUT /api/Roles/{id}: Update role (Admin)
DELETE /api/Roles/{id}: Delete role (Admin)


Health Check:
GET /health: Check API health



Notes

Ensure the SQL Server instance is running and accessible.
Update appsettings.json with your JWT key for production.
The API uses in-memory caching for Departments and Roles with a 30-minute expiration.
Unit tests are included in the EmployeeManagementApi.Tests project.

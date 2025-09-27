-- Seed Demo Data for Tenant Database
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
USE HRApi_Tenant_Demo;

-- Insert Departments
DECLARE @HRDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @ITDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @FinDeptId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Departments (Id, Name, Code, Description, IsActive, CreatedAt, IsDeleted)
VALUES 
    (@HRDeptId, 'Human Resources', 'HR', 'Manages employee relations and policies', 1, GETUTCDATE(), 0),
    (@ITDeptId, 'Information Technology', 'IT', 'Manages company technology infrastructure', 1, GETUTCDATE(), 0),
    (@FinDeptId, 'Finance', 'FIN', 'Manages company finances and accounting', 1, GETUTCDATE(), 0);

-- Insert Positions
DECLARE @HRManagerId UNIQUEIDENTIFIER = NEWID();
DECLARE @DevId UNIQUEIDENTIFIER = NEWID();
DECLARE @AnalystId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Positions (Id, Title, Code, DepartmentId, MinSalary, MaxSalary, IsActive, CreatedAt, IsDeleted)
VALUES 
    (@HRManagerId, 'HR Manager', 'HRM', @HRDeptId, 60000, 90000, 1, GETUTCDATE(), 0),
    (@DevId, 'Software Developer', 'DEV', @ITDeptId, 50000, 120000, 1, GETUTCDATE(), 0),
    (@AnalystId, 'Financial Analyst', 'FA', @FinDeptId, 45000, 75000, 1, GETUTCDATE(), 0);

-- Insert Leave Types
INSERT INTO LeaveTypes (Id, Name, Code, Description, MaxDaysPerYear, RequiresApproval, IsActive, CreatedAt, IsDeleted, IsCarryForward)
VALUES 
    (NEWID(), 'Annual Leave', 'AL', 'Yearly vacation days', 25, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Sick Leave', 'SL', 'Medical leave for illness', 10, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Personal Leave', 'PL', 'Personal time off', 5, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Maternity Leave', 'ML', 'Maternity/Paternity leave', 90, 1, 1, GETUTCDATE(), 0, 0);

-- Insert Sample Employees
INSERT INTO Employees (Id, EmployeeNumber, FirstName, LastName, Email, PhoneNumber, HireDate, DepartmentId, PositionId, Status, EmploymentType, Salary, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), 'EMP001', 'John', 'Doe', 'john.doe@democompany.com', '+1-555-0101', '2023-01-15', @ITDeptId, @DevId, 1, 1, 75000, GETUTCDATE(), 0),
    (NEWID(), 'EMP002', 'Jane', 'Smith', 'jane.smith@democompany.com', '+1-555-0102', '2023-02-01', @HRDeptId, @HRManagerId, 1, 1, 80000, GETUTCDATE(), 0),
    (NEWID(), 'EMP003', 'Mike', 'Johnson', 'mike.johnson@democompany.com', '+1-555-0103', '2023-03-10', @FinDeptId, @AnalystId, 1, 1, 60000, GETUTCDATE(), 0);

PRINT 'Demo data seeded successfully!';
PRINT 'Created:';
PRINT '- 3 Departments (HR, IT, Finance)';
PRINT '- 3 Positions (HR Manager, Software Developer, Financial Analyst)';
PRINT '- 4 Leave Types (Annual, Sick, Personal, Maternity)';
PRINT '- 3 Sample Employees';

SELECT 'Departments Created:' AS Info;
SELECT Name, Code FROM Departments WHERE IsDeleted = 0;

SELECT 'Employees Created:' AS Info;
SELECT EmployeeNumber, FirstName + ' ' + LastName AS FullName, Email FROM Employees WHERE IsDeleted = 0;

-- Seed Data for Company1 Tenant Database
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
USE HRApi_Company1;
GO

PRINT 'Starting to seed Company1 database...';

-- Check if data already exists
IF EXISTS (SELECT 1 FROM Departments WHERE IsDeleted = 0)
BEGIN
    PRINT 'Company1 database already contains data. Skipping seeding.';
    RETURN;
END

-- Insert Departments for Company1
DECLARE @HRDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @ITDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @FinDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @MarketingDeptId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Departments (Id, Name, Code, Description, IsActive, CreatedAt, IsDeleted)
VALUES 
    (@HRDeptId, 'Human Resources', 'HR', 'Manages employee relations and Company1 policies', 1, GETUTCDATE(), 0),
    (@ITDeptId, 'Information Technology', 'IT', 'Manages Company1 technology infrastructure', 1, GETUTCDATE(), 0),
    (@FinDeptId, 'Finance & Accounting', 'FIN', 'Manages Company1 finances and accounting', 1, GETUTCDATE(), 0),
    (@MarketingDeptId, 'Marketing & Sales', 'MKT', 'Handles Company1 marketing and sales operations', 1, GETUTCDATE(), 0);

-- Insert Positions for Company1
DECLARE @HRManagerId UNIQUEIDENTIFIER = NEWID();
DECLARE @SeniorDevId UNIQUEIDENTIFIER = NEWID();
DECLARE @JuniorDevId UNIQUEIDENTIFIER = NEWID();
DECLARE @FinAnalystId UNIQUEIDENTIFIER = NEWID();
DECLARE @MktSpecialistId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Positions (Id, Title, Code, MinSalary, MaxSalary, IsActive, CreatedAt, IsDeleted)
VALUES 
    (@HRManagerId, 'HR Manager', 'HRM', 65000, 95000, 1, GETUTCDATE(), 0),
    (@SeniorDevId, 'Senior Software Developer', 'SDEV', 80000, 130000, 1, GETUTCDATE(), 0),
    (@JuniorDevId, 'Junior Software Developer', 'JDEV', 45000, 70000, 1, GETUTCDATE(), 0),
    (@FinAnalystId, 'Financial Analyst', 'FA', 50000, 80000, 1, GETUTCDATE(), 0),
    (@MktSpecialistId, 'Marketing Specialist', 'MS', 45000, 75000, 1, GETUTCDATE(), 0);

-- Insert Leave Types for Company1
INSERT INTO LeaveTypes (Id, Name, Code, Description, MaxDaysPerYear, RequiresApproval, IsActive, CreatedAt, IsDeleted, IsCarryForward)
VALUES 
    (NEWID(), 'Annual Leave', 'AL', 'Company1 yearly vacation days', 25, 1, 1, GETUTCDATE(), 0, 1),
    (NEWID(), 'Sick Leave', 'SL', 'Medical leave for illness', 12, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Personal Leave', 'PL', 'Personal time off', 7, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Maternity/Paternity Leave', 'ML', 'Parental leave', 120, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Study Leave', 'STL', 'Educational development leave', 10, 1, 1, GETUTCDATE(), 0, 0);

-- Insert Sample Employees for Company1
DECLARE @Employee1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee4Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee5Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO Employees (Id, EmployeeNumber, FirstName, LastName, Email, PhoneNumber, HireDate, DepartmentId, PositionId, Status, EmploymentType, Salary, IsDepartmentManager, CreatedAt, IsDeleted)
VALUES 
    (@Employee1Id, 'C1EMP001', 'Alice', 'Johnson', 'alice.johnson@company1.com', '+1-555-1001', '2023-01-15', @ITDeptId, @SeniorDevId, 1, 1, 95000, 0, GETUTCDATE(), 0),
    (@Employee2Id, 'C1EMP002', 'Robert', 'Smith', 'robert.smith@company1.com', '+1-555-1002', '2023-02-01', @HRDeptId, @HRManagerId, 1, 1, 85000, 1, GETUTCDATE(), 0),
    (@Employee3Id, 'C1EMP003', 'Emily', 'Davis', 'emily.davis@company1.com', '+1-555-1003', '2023-03-10', @FinDeptId, @FinAnalystId, 1, 1, 65000, 1, GETUTCDATE(), 0),
    (@Employee4Id, 'C1EMP004', 'Michael', 'Brown', 'michael.brown@company1.com', '+1-555-1004', '2023-04-05', @ITDeptId, @JuniorDevId, 1, 1, 55000, 0, GETUTCDATE(), 0),
    (@Employee5Id, 'C1EMP005', 'Sarah', 'Wilson', 'sarah.wilson@company1.com', '+1-555-1005', '2023-05-20', @MarketingDeptId, @MktSpecialistId, 1, 1, 60000, 1, GETUTCDATE(), 0);

-- Create Leave Balances for all employees and leave types
INSERT INTO LeaveBalances (Id, EmployeeId, LeaveTypeId, Year, AllocatedDays, UsedDays, CarryForwardDays, CreatedAt, IsDeleted)
SELECT 
    NEWID() as Id,
    e.Id as EmployeeId,
    lt.Id as LeaveTypeId,
    YEAR(GETDATE()) as Year,
    lt.MaxDaysPerYear as AllocatedDays,
    0 as UsedDays,
    0 as CarryForwardDays,
    GETUTCDATE() as CreatedAt,
    0 as IsDeleted
FROM Employees e
CROSS JOIN LeaveTypes lt
WHERE e.IsDeleted = 0 AND lt.IsDeleted = 0;

PRINT 'Company1 data seeded successfully!';
PRINT 'Created for Company1:';
PRINT '- 4 Departments (HR, IT, Finance, Marketing)';
PRINT '- 5 Positions (HR Manager, Senior Dev, Junior Dev, Financial Analyst, Marketing Specialist)';
PRINT '- 5 Leave Types (Annual, Sick, Personal, Maternity/Paternity, Study)';
PRINT '- 5 Sample Employees';
PRINT '- Leave balances for all employees';

-- Display summary
SELECT 'Company1 Departments:' AS Info;
SELECT Name, Code, Description FROM Departments WHERE IsDeleted = 0;

SELECT 'Company1 Employees:' AS Info;
SELECT EmployeeNumber, FirstName + ' ' + LastName AS FullName, Email FROM Employees WHERE IsDeleted = 0;

SELECT 'Company1 Leave Balances Summary:' AS Info;
SELECT 
    e.EmployeeNumber,
    e.FirstName + ' ' + e.LastName AS EmployeeName,
    lt.Name AS LeaveType,
    lb.AllocatedDays,
    (lb.AllocatedDays + lb.CarryForwardDays - lb.UsedDays) AS RemainingDays
FROM LeaveBalances lb
JOIN Employees e ON lb.EmployeeId = e.Id
JOIN LeaveTypes lt ON lb.LeaveTypeId = lt.Id
WHERE lb.IsDeleted = 0
ORDER BY e.EmployeeNumber, lt.Name;

PRINT 'Company1 database seeding completed successfully!';
GO

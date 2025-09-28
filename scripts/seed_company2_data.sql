-- Seed Data for Company2 Tenant Database
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
USE HRApi_Company2;
GO

PRINT 'Starting to seed Company2 database...';

-- Check if data already exists
IF EXISTS (SELECT 1 FROM Departments WHERE IsDeleted = 0)
BEGIN
    PRINT 'Company2 database already contains data. Skipping seeding.';
    RETURN;
END

-- Insert Departments for Company2
DECLARE @HRDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @ITDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @OperationsDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @SalesDeptId UNIQUEIDENTIFIER = NEWID();
DECLARE @SupportDeptId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Departments (Id, Name, Code, Description, IsActive, CreatedAt, IsDeleted)
VALUES 
    (@HRDeptId, 'Human Resources', 'HR', 'Employee management and Company2 HR policies', 1, GETUTCDATE(), 0),
    (@ITDeptId, 'Technology', 'TECH', 'Company2 technical infrastructure and development', 1, GETUTCDATE(), 0),
    (@OperationsDeptId, 'Operations', 'OPS', 'Company2 daily operations and logistics', 1, GETUTCDATE(), 0),
    (@SalesDeptId, 'Sales', 'SALES', 'Company2 sales and business development', 1, GETUTCDATE(), 0),
    (@SupportDeptId, 'Customer Support', 'SUP', 'Company2 customer service and support', 1, GETUTCDATE(), 0);

-- Insert Positions for Company2
DECLARE @HRDirectorId UNIQUEIDENTIFIER = NEWID();
DECLARE @TechLeadId UNIQUEIDENTIFIER = NEWID();
DECLARE @DevOpsId UNIQUEIDENTIFIER = NEWID();
DECLARE @OpsManagerId UNIQUEIDENTIFIER = NEWID();
DECLARE @SalesRepId UNIQUEIDENTIFIER = NEWID();
DECLARE @SupportSpecId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Positions (Id, Title, Code, MinSalary, MaxSalary, IsActive, CreatedAt, IsDeleted)
VALUES 
    (@HRDirectorId, 'HR Director', 'HRD', 75000, 110000, 1, GETUTCDATE(), 0),
    (@TechLeadId, 'Technical Lead', 'TL', 90000, 140000, 1, GETUTCDATE(), 0),
    (@DevOpsId, 'DevOps Engineer', 'DEVOPS', 70000, 110000, 1, GETUTCDATE(), 0),
    (@OpsManagerId, 'Operations Manager', 'OPM', 60000, 90000, 1, GETUTCDATE(), 0),
    (@SalesRepId, 'Sales Representative', 'SR', 40000, 80000, 1, GETUTCDATE(), 0),
    (@SupportSpecId, 'Support Specialist', 'SS', 35000, 55000, 1, GETUTCDATE(), 0);

-- Insert Leave Types for Company2 (different policy than Company1)
INSERT INTO LeaveTypes (Id, Name, Code, Description, MaxDaysPerYear, RequiresApproval, IsActive, CreatedAt, IsDeleted, IsCarryForward)
VALUES 
    (NEWID(), 'Vacation Leave', 'VL', 'Company2 annual vacation days', 20, 1, 1, GETUTCDATE(), 0, 1),
    (NEWID(), 'Sick Leave', 'SL', 'Medical and health-related leave', 15, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Emergency Leave', 'EL', 'Emergency personal situations', 5, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Parental Leave', 'PL', 'Maternity/Paternity leave', 90, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Bereavement Leave', 'BL', 'Leave for family bereavement', 5, 1, 1, GETUTCDATE(), 0, 0),
    (NEWID(), 'Training Leave', 'TL', 'Professional development and training', 8, 1, 1, GETUTCDATE(), 0, 0);

-- Insert Sample Employees for Company2
DECLARE @Employee1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee4Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee5Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Employee6Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO Employees (Id, EmployeeNumber, FirstName, LastName, Email, PhoneNumber, HireDate, DepartmentId, PositionId, Status, EmploymentType, Salary, IsDepartmentManager, CreatedAt, IsDeleted)
VALUES 
    (@Employee1Id, 'C2EMP001', 'David', 'Martinez', 'david.martinez@company2.com', '+1-555-2001', '2022-11-01', @ITDeptId, @TechLeadId, 1, 1, 115000, 1, GETUTCDATE(), 0),
    (@Employee2Id, 'C2EMP002', 'Jennifer', 'Taylor', 'jennifer.taylor@company2.com', '+1-555-2002', '2023-01-15', @HRDeptId, @HRDirectorId, 1, 1, 95000, 1, GETUTCDATE(), 0),
    (@Employee3Id, 'C2EMP003', 'James', 'Anderson', 'james.anderson@company2.com', '+1-555-2003', '2023-02-20', @OperationsDeptId, @OpsManagerId, 1, 1, 75000, 1, GETUTCDATE(), 0),
    (@Employee4Id, 'C2EMP004', 'Lisa', 'Thompson', 'lisa.thompson@company2.com', '+1-555-2004', '2023-03-15', @SalesDeptId, @SalesRepId, 1, 1, 60000, 0, GETUTCDATE(), 0),
    (@Employee5Id, 'C2EMP005', 'Kevin', 'White', 'kevin.white@company2.com', '+1-555-2005', '2023-04-10', @ITDeptId, @DevOpsId, 1, 1, 85000, 0, GETUTCDATE(), 0),
    (@Employee6Id, 'C2EMP006', 'Amanda', 'Garcia', 'amanda.garcia@company2.com', '+1-555-2006', '2023-05-01', @SupportDeptId, @SupportSpecId, 1, 1, 45000, 0, GETUTCDATE(), 0);

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

PRINT 'Company2 data seeded successfully!';
PRINT 'Created for Company2:';
PRINT '- 5 Departments (HR, Technology, Operations, Sales, Customer Support)';
PRINT '- 6 Positions (HR Director, Tech Lead, DevOps Engineer, Ops Manager, Sales Rep, Support Specialist)';
PRINT '- 6 Leave Types (Vacation, Sick, Emergency, Parental, Bereavement, Training)';
PRINT '- 6 Sample Employees';
PRINT '- Leave balances for all employees';

-- Display summary
SELECT 'Company2 Departments:' AS Info;
SELECT Name, Code, Description FROM Departments WHERE IsDeleted = 0;

SELECT 'Company2 Employees:' AS Info;
SELECT EmployeeNumber, FirstName + ' ' + LastName AS FullName, Email FROM Employees WHERE IsDeleted = 0;

SELECT 'Company2 Leave Balances Summary:' AS Info;
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

PRINT 'Company2 database seeding completed successfully!';
GO

@echo off
REM Batch script to migrate Company2 database
REM This script applies EF Core migrations to the Company2 tenant database

echo Migrating Company2 database...

REM Check if we're in the right directory
if not exist "src\HRApi.Web\HRApi.Web.csproj" (
    echo Please run this script from the project root directory
    exit /b 1
)

REM Apply migrations to Company2 database using command-line tenant specification
echo Running: dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company2
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company2

if %ERRORLEVEL% equ 0 (
    echo Company2 database migration completed successfully!
) else (
    echo Company2 database migration failed!
    exit /b 1
)

@echo off
REM Batch script to migrate Company1 database
REM This script applies EF Core migrations to the Company1 tenant database

echo Migrating Company1 database...

REM Check if we're in the right directory
if not exist "src\HRApi.Web\HRApi.Web.csproj" (
    echo Please run this script from the project root directory
    exit /b 1
)

REM Apply migrations to Company1 database using command-line tenant specification
echo Running: dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company1
dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company1

if %ERRORLEVEL% equ 0 (
    echo Company1 database migration completed successfully!
) else (
    echo Company1 database migration failed!
    exit /b 1
)

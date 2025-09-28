# PowerShell script to migrate Company1 database
# This script applies EF Core migrations to the Company1 tenant database

Write-Host "Migrating Company1 database..." -ForegroundColor Green

try {
    # Navigate to project root if not already there
    if (!(Test-Path "src/HRApi.Web/HRApi.Web.csproj")) {
        Write-Host "Please run this script from the project root directory" -ForegroundColor Red
        exit 1
    }

    # Apply migrations to Company1 database using command-line tenant specification
    Write-Host "Running: dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company1" -ForegroundColor Yellow
    dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company1

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Company1 database migration completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Company1 database migration failed!" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "Error during migration: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

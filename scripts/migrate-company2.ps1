# PowerShell script to migrate Company2 database
# This script applies EF Core migrations to the Company2 tenant database

Write-Host "Migrating Company2 database..." -ForegroundColor Green

try {
    # Navigate to project root if not already there
    if (!(Test-Path "src/HRApi.Web/HRApi.Web.csproj")) {
        Write-Host "Please run this script from the project root directory" -ForegroundColor Red
        exit 1
    }

    # Apply migrations to Company2 database using command-line tenant specification
    Write-Host "Running: dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company2" -ForegroundColor Yellow
    dotnet ef database update --context HRDbContext --project src/HRApi.Infrastructure --startup-project src/HRApi.Web -- --tenant=Company2

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Company2 database migration completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Company2 database migration failed!" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "Error during migration: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

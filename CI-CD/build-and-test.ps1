#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build and test the TimeWarp.Fixie project
.DESCRIPTION
    This script handles the build and test phase of the CI/CD pipeline.
    It restores tools, cleans, restores dependencies, builds, and runs tests.
.PARAMETER Configuration
    The build configuration (Debug or Release). Default is Release.
.PARAMETER OutputPath
    The output path for build artifacts. Default is ./artifacts
#>

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "./artifacts"
)

$ErrorActionPreference = "Stop"
$ProjectPath = "./source/TimeWarp.Fixie/TimeWarp.Fixie.csproj"

try {
    Push-Location $PSScriptRoot/..
    
    Write-Host "Starting build and test process..." -ForegroundColor Green
    
    # Restore tools
    Write-Host "Restoring tools..." -ForegroundColor Yellow
    dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw "Tool restore failed" }
    
    # Clean
    Write-Host "Cleaning..." -ForegroundColor Yellow
    dotnet cleanup -y
    if ($LASTEXITCODE -ne 0) { throw "Clean failed" }
    
    # Restore dependencies
    Write-Host "Restoring dependencies..." -ForegroundColor Yellow
    dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }
    
    # Build
    Write-Host "Building..." -ForegroundColor Yellow
    dotnet build --no-restore --configuration $Configuration
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    
    # Test
    Write-Host "Running tests..." -ForegroundColor Yellow
    dotnet fixie ./tests/TimeWarp.Fixie.Tests --configuration $Configuration --no-build
    if ($LASTEXITCODE -ne 0) { throw "Tests failed" }
    
    # Create output directory
    if (!(Test-Path $OutputPath)) {
        New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    }
    
    # Pack
    Write-Host "Creating NuGet package..." -ForegroundColor Yellow
    dotnet pack $ProjectPath --no-build --configuration $Configuration --output $OutputPath
    if ($LASTEXITCODE -ne 0) { throw "Pack failed" }
    
    Write-Host "Build and test completed successfully!" -ForegroundColor Green
    
    # List created packages
    $packages = Get-ChildItem -Path $OutputPath -Filter "*.nupkg"
    if ($packages) {
        Write-Host "Created packages:" -ForegroundColor Green
        foreach ($package in $packages) {
            Write-Host "  - $($package.Name)" -ForegroundColor Cyan
        }
    }
}
catch {
    Write-Error "Build and test failed: $_"
    exit 1
}
finally {
    Pop-Location
}

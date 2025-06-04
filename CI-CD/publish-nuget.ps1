#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publish NuGet packages to NuGet.org
.DESCRIPTION
    This script publishes NuGet packages to NuGet.org. It validates the API key,
    finds packages in the specified directory, and publishes them.
.PARAMETER PackagePath
    The path containing the NuGet packages to publish. Default is ./artifacts
.PARAMETER NuGetApiKey
    The NuGet API key. If not provided, will use the NUGET_API_KEY environment variable.
.PARAMETER Source
    The NuGet source URL. Default is https://api.nuget.org/v3/index.json
.PARAMETER SkipDuplicate
    Skip packages that already exist on the server. Default is true.
#>

param(
    [string]$PackagePath = "./artifacts",
    [string]$NuGetApiKey = $env:NUGET_API_KEY,
    [string]$Source = "https://api.nuget.org/v3/index.json",
    [bool]$SkipDuplicate = $true
)

$ErrorActionPreference = "Stop"

try {
    Push-Location $PSScriptRoot/..
    
    Write-Host "Starting NuGet publish process..." -ForegroundColor Green
    
    # Validate API key
    if ([string]::IsNullOrWhiteSpace($NuGetApiKey)) {
        throw "NuGet API key is not set. Please provide it via -NuGetApiKey parameter or NUGET_API_KEY environment variable."
    }
    
    # Validate package path
    if (!(Test-Path $PackagePath)) {
        throw "Package path '$PackagePath' does not exist."
    }
    
    # Find packages
    $packages = Get-ChildItem -Path $PackagePath -Filter "*.nupkg" -Recurse
    if (!$packages) {
        throw "No NuGet packages found in '$PackagePath'."
    }
    
    Write-Host "Found $($packages.Count) package(s) to publish:" -ForegroundColor Yellow
    foreach ($package in $packages) {
        Write-Host "  - $($package.Name)" -ForegroundColor Cyan
    }
    
    # Publish packages
    foreach ($package in $packages) {
        Write-Host "Publishing $($package.Name)..." -ForegroundColor Yellow
        
        $publishArgs = @(
            "nuget", "push", $package.FullName,
            "--source", $Source,
            "--api-key", $NuGetApiKey
        )
        
        if ($SkipDuplicate) {
            $publishArgs += "--skip-duplicate"
        }
        
        & dotnet @publishArgs
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to publish package: $($package.Name)"
        }
        
        Write-Host "Successfully published $($package.Name)" -ForegroundColor Green
    }
    
    Write-Host "All packages published successfully!" -ForegroundColor Green
}
catch {
    Write-Error "Publish failed: $_"
    exit 1
}
finally {
    Pop-Location
}

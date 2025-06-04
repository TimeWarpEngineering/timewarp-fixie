#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Main CI/CD orchestration script for TimeWarp.Fixie
.DESCRIPTION
    This script orchestrates the complete CI/CD pipeline including build, test, and publish phases.
    It can be run locally for testing or called from GitHub Actions.
.PARAMETER Phase
    The phase to run: 'build', 'publish', or 'all'. Default is 'all'.
.PARAMETER Configuration
    The build configuration (Debug or Release). Default is Release.
.PARAMETER PublishToNuGet
    Whether to publish to NuGet. Default is false for safety.
.PARAMETER NuGetApiKey
    The NuGet API key. If not provided, will use the NUGET_API_KEY environment variable.
#>

param(
    [ValidateSet("build", "publish", "all")]
    [string]$Phase = "all",
    [string]$Configuration = "Release",
    [bool]$PublishToNuGet = $false,
    [string]$NuGetApiKey = $env:NUGET_API_KEY
)

$ErrorActionPreference = "Stop"
$ArtifactsPath = "./artifacts"

function Write-PhaseHeader {
    param([string]$PhaseName)
    Write-Host ""
    Write-Host "=" * 60 -ForegroundColor Magenta
    Write-Host "  $PhaseName" -ForegroundColor Magenta
    Write-Host "=" * 60 -ForegroundColor Magenta
    Write-Host ""
}

try {
    Push-Location $PSScriptRoot
    
    Write-Host "TimeWarp.Fixie CI/CD Pipeline" -ForegroundColor Green
    Write-Host "Phase: $Phase" -ForegroundColor Cyan
    Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
    Write-Host "Publish to NuGet: $PublishToNuGet" -ForegroundColor Cyan
    
    # Build and Test Phase
    if ($Phase -eq "build" -or $Phase -eq "all") {
        Write-PhaseHeader "BUILD AND TEST PHASE"
        
        & ./build-and-test.ps1 -Configuration $Configuration -OutputPath $ArtifactsPath
        if ($LASTEXITCODE -ne 0) {
            throw "Build and test phase failed"
        }
    }
    
    # Publish Phase
    if (($Phase -eq "publish" -or $Phase -eq "all") -and $PublishToNuGet) {
        Write-PhaseHeader "PUBLISH PHASE"
        
        if ([string]::IsNullOrWhiteSpace($NuGetApiKey)) {
            Write-Warning "NuGet API key not provided. Skipping publish phase."
            Write-Host "To publish, provide the API key via -NuGetApiKey parameter or NUGET_API_KEY environment variable." -ForegroundColor Yellow
        } else {
            & ./publish-nuget.ps1 -PackagePath $ArtifactsPath -NuGetApiKey $NuGetApiKey
            if ($LASTEXITCODE -ne 0) {
                throw "Publish phase failed"
            }
        }
    } elseif ($Phase -eq "publish" -or $Phase -eq "all") {
        Write-Host "Publish phase skipped (PublishToNuGet = $PublishToNuGet)" -ForegroundColor Yellow
    }
    
    Write-PhaseHeader "PIPELINE COMPLETED SUCCESSFULLY"
    Write-Host "All phases completed successfully!" -ForegroundColor Green
    
    # Show artifacts
    if (Test-Path $ArtifactsPath) {
        $artifacts = Get-ChildItem -Path $ArtifactsPath -Filter "*.nupkg"
        if ($artifacts) {
            Write-Host ""
            Write-Host "Generated artifacts:" -ForegroundColor Green
            foreach ($artifact in $artifacts) {
                Write-Host "  - $($artifact.Name)" -ForegroundColor Cyan
            }
        }
    }
}
catch {
    Write-Error "CI/CD Pipeline failed: $_"
    exit 1
}
finally {
    Pop-Location
}

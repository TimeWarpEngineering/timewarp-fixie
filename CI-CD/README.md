# CI/CD Pipeline for TimeWarp.Fixie

This folder contains the CI/CD pipeline scripts for the TimeWarp.Fixie project. The pipeline is designed to be testable locally using PowerShell scripts, with minimal GitHub Actions orchestration.

## Scripts

### `ci-cd.ps1` - Main Orchestration Script
The main entry point for the CI/CD pipeline. Can run individual phases or the complete pipeline.

**Usage:**
```powershell
# Run complete pipeline (build + test only)
./CI-CD/ci-cd.ps1

# Run only build and test
./CI-CD/ci-cd.ps1 -Phase build

# Run complete pipeline with NuGet publishing
./CI-CD/ci-cd.ps1 -PublishToNuGet $true

# Run only publish phase
./CI-CD/ci-cd.ps1 -Phase publish -PublishToNuGet $true
```

**Parameters:**
- `Phase`: 'build', 'publish', or 'all' (default: 'all')
- `Configuration`: 'Debug' or 'Release' (default: 'Release')
- `PublishToNuGet`: Whether to publish to NuGet (default: false)
- `NuGetApiKey`: NuGet API key (uses NUGET_API_KEY env var if not provided)

### `build-and-test.ps1` - Build and Test Phase
Handles the build and test phase of the pipeline.

**What it does:**
1. Restores dotnet tools
2. Runs `dotnet cleanup`
3. Restores dependencies
4. Builds the project
5. Runs tests
6. Creates NuGet packages

**Usage:**
```powershell
./CI-CD/build-and-test.ps1 -Configuration Release -OutputPath ./artifacts
```

### `publish-nuget.ps1` - NuGet Publishing Phase
Handles publishing NuGet packages to NuGet.org.

**What it does:**
1. Validates NuGet API key
2. Finds packages in the specified directory
3. Publishes packages to NuGet.org
4. Supports skip-duplicate option

**Usage:**
```powershell
# Using environment variable
$env:NUGET_API_KEY = "your-api-key"
./CI-CD/publish-nuget.ps1

# Using parameter
./CI-CD/publish-nuget.ps1 -NuGetApiKey "your-api-key"
```

## GitHub Actions Integration

The pipeline integrates with GitHub Actions through [`.github/workflows/ci-cd.yml`](../.github/workflows/ci-cd.yml):

- **Build and Test**: Runs on every push and PR to master branch
- **Publish**: Runs only on GitHub releases, publishes to NuGet.org

## Local Testing

You can test the entire pipeline locally:

```powershell
# Test build and test phase
./CI-CD/ci-cd.ps1 -Phase build

# Test complete pipeline (without publishing)
./CI-CD/ci-cd.ps1

# Test with publishing (requires API key)
$env:NUGET_API_KEY = "your-test-api-key"
./CI-CD/ci-cd.ps1 -PublishToNuGet $true
```

## Environment Variables

- `NUGET_API_KEY`: Required for publishing to NuGet.org

## Migration from Manual Process

The previous manual [`publish.ps1`](../publish.ps1) script functionality has been integrated into this CI/CD pipeline:

| Old Script Step | New Location |
|----------------|--------------|
| `Push-Location $PSScriptRoot` | Handled in each script |
| `dotnet tool restore` | `build-and-test.ps1` |
| `dotnet cleanup -y` | `build-and-test.ps1` |
| `dotnet pack` | `build-and-test.ps1` |
| `dotnet nuget push` | `publish-nuget.ps1` |
| Error handling | Enhanced in all scripts |

## Security Considerations

- NuGet API key is handled through GitHub Secrets in CI/CD
- Scripts validate API key presence before attempting to publish
- Local testing can use environment variables
- No API keys are stored in source code

## Troubleshooting

### Build Failures
1. Check that all dependencies are properly restored
2. Ensure .NET 9.0 SDK is installed
3. Verify project builds locally with `dotnet build`

### Test Failures
1. Run tests locally with `dotnet fixie`
2. Check test output for specific failure details
3. Ensure test dependencies are properly configured

### Publish Failures
1. Verify NuGet API key is correctly set
2. Check that packages don't already exist (unless using skip-duplicate)
3. Ensure network connectivity to NuGet.org
4. Verify package metadata is valid

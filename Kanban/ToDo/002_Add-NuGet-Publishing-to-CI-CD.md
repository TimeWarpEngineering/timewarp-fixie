# 002_Add-NuGet-Publishing-to-CI-CD.md

## Description

Add automated NuGet package publishing to the CI/CD workflow. Currently, there is a manual [`publish.ps1`](publish.ps1:1) script that handles the publishing process, but this should be integrated into an automated CI/CD pipeline to ensure consistent and reliable package releases.

## Requirements

- Create CI/CD workflow configuration (GitHub Actions, Azure DevOps, or similar)
- Integrate the existing publish script logic into the CI/CD pipeline
- Ensure secure handling of NuGet API key through environment variables/secrets
- Trigger publishing on version tags or specific branch pushes
- Include build, test, and pack steps before publishing
- Maintain backward compatibility with existing manual process during transition
- Add proper error handling and logging

## Checklist

### Design
- [ ] Choose CI/CD platform (GitHub Actions, Azure DevOps, etc.)
- [ ] Design workflow trigger strategy (tags, branches, manual)
- [ ] Plan secret/environment variable management for NuGet API key

### Implementation
- [ ] Create CI/CD folder structure
- [ ] Move/adapt [`publish.ps1`](publish.ps1:1) logic to CI/CD workflow
- [ ] Add build and test steps to workflow
- [ ] Configure NuGet package creation step
- [ ] Implement NuGet publishing step with proper authentication
- [ ] Add conditional logic for when to publish (version tags, etc.)
- [ ] Test workflow in development environment

### Configuration
- [ ] Set up required secrets/environment variables
- [ ] Configure workflow permissions
- [ ] Add status badges to README if applicable

### Documentation
- [ ] Update README.md with CI/CD information
- [ ] Document the new publishing process
- [ ] Add troubleshooting guide for CI/CD issues
- [ ] Update .ai prompts if needed

### Review
- [ ] Consider Security Implications (secure secret handling)
- [ ] Consider Performance Implications (workflow execution time)
- [ ] Consider Monitoring and Alerting Implications (build notifications)
- [ ] Consider Accessibility Implications (N/A for CI/CD)
- [ ] Code Review

## Notes

Current [`publish.ps1`](publish.ps1:1) script includes:
- Environment validation (`$env:Nuget_Key` check)
- Tool restoration (`dotnet tool restore`)
- Cleanup (`dotnet cleanup -y`)
- Package creation (`dotnet pack ./source/TimeWarp.Fixie/TimeWarp.Fixie.csproj -c Release`)
- NuGet publishing (`dotnet nuget push **/*.nupkg --source https://api.nuget.org/v3/index.json --api-key $env:Nuget_Key`)

The CI/CD implementation should preserve this functionality while adding:
- Automated triggering
- Better error handling
- Build/test validation before publishing
- Proper logging and status reporting

## Implementation Notes

[Include notes while task is in progress]

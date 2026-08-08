# Add trusted-publishing probe mode to workflow.yml

## Description

org 458-009 probe (NuGet has no policy-enumeration API; probe = dispatch mode that runs only the nuget/login OIDC exchange and stops — success proves the workflow.yml policy matches; reference timewarp-nuru's workflow.yml).

## Checklist

- [x] probe input added
- [x] login step condition extended
- [x] probe-result step added
- [x] pipeline step skipped in probe mode
- [x] YAML valid

## Results

- Added `probe` to the `workflow_dispatch.inputs.mode` choice options and updated its description.
- `build-and-test` job: gated the `Run build and test` and `Upload artifacts` steps to skip when `mode == 'probe'`, so probe mode does no build (this job has no job-level `if:`, so it always runs and still succeeds, satisfying `publish-nuget`'s `needs: build-and-test`).
- `publish-nuget` job: extended the job-level `if:` to also enter on `mode == 'probe'` (previously it only ran on release/release-confirm, never on any dispatch mode by default).
- Inside `publish-nuget`: gated `Download artifacts` and `Publish to NuGet` steps to skip in probe mode; added a new `Trusted publishing probe result` step gated to `mode == 'probe'` that prints success after the OIDC exchange; left the `NuGet login` step's `if:` absent (unchanged) since the job-level `if:` already correctly restricts when it runs.
- `id-token: write` permission was already present at the `publish-nuget` job level — no change needed.

### How to validate

**Smoke:** `gh workflow run workflow.yml -f mode=probe` after push → expect the "Trusted publishing probe result" step to run and go green.
**Expect:** a failure of the NuGet login step means the trusted-publishing policy is missing or misconfigured on NuGet.org for this repo + workflow.yml — not a bug in this change.

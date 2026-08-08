# Migrate NuGet publish workflow to trusted publishing (nuget/login)

## Description

The trusted publishing policy for this repo already exists on NuGet.org
(owner TimeWarp.Enterprises, created 2026-08-08) but is INERT until the
publish workflow exchanges an OIDC token for a temp key instead of using a
stored secret. Org program context: timewarp-nuru kanban 458-009.

Current state (2026-08-07 org audit): secret `NUGET_API_KEY` in ci-cd.yml; publishes TimeWarp.Fixie.

Reference implementation: timewarp-nuru `.github/workflows/workflow.yml` —
`nuget/login@v1` step (user: TimeWarp.Enterprises) gated on the release
condition, `id-token: write` job permission, push via
`--api-key ${{ steps.nuget-login.outputs.NUGET_API_KEY }}`.

NOTE: if this repo's full convention conversion (reusable-workflow caller,
timewarp-nuru 458 rollout) is imminent, do the conversion instead — it
includes this migration for free.

## Checklist

- [x] Add `id-token: write` (with `contents: read`) permissions to the publish job
- [x] Add `nuget/login@v1` gated on the publish condition
- [x] Replace the stored-secret `--api-key` with the login step output
- [ ] Verify the publish path end-to-end on the next release
- [ ] AFTER verified: operator revokes the long-lived NuGet key and deletes the GitHub secret (org-wide revocation tracked in nuru 458-009)

## Notes

Created from the timewarp-nuru 458-009 rollout session (2026-08-08).

### Implementation Plan: Task 001 — Migrate NuGet publish to trusted publishing

#### Goal
Make the `publish-nuget` job in `.github/workflows/ci-cd.yml` obtain a short-lived NuGet API key via OIDC Trusted Publishing (`nuget/login@v1`) instead of `secrets.NUGET_API_KEY`, so the existing NuGet.org policy (owner TimeWarp.Enterprises, created 2026-08-08) becomes active.

#### Non-goals
- Do not rewrite PowerShell (`CI-CD/ci-cd.ps1`, `CI-CD/publish-nuget.ps1`)
- Do not convert to reusable-workflow caller / org convention rollout
- Do not delete GitHub secret or revoke long-lived NuGet key in this PR
- Do not change package IDs, build/test job, artifact names, or job trigger conditions

#### Concrete change (single file: `.github/workflows/ci-cd.yml`)

Only the `publish-nuget` job changes:

1. After job `if:`, add job-level permissions:
```yaml
permissions:
  contents: read
  id-token: write  # Required for NuGet Trusted Publishing (OIDC)
```

2. Before Publish step, insert:
```yaml
- name: NuGet login (OIDC Trusted Publishing)
  id: nuget-login
  uses: nuget/login@v1
  with:
    user: TimeWarp.Enterprises
```
(No step-level `if:` — job already gates release + workflow_dispatch publish.)

3. Change publish env from `secrets.NUGET_API_KEY` to:
```yaml
NUGET_API_KEY: ${{ steps.nuget-login.outputs.NUGET_API_KEY }}
```

#### Design choices
| Decision | Choice | Rationale |
|----------|--------|-----------|
| Permissions scope | Job-level on publish-nuget only | Least privilege |
| Login step if | None (job if is gate) | Covers release and workflow_dispatch |
| Key wiring | Env NUGET_API_KEY from step output | Matches PowerShell contract |
| User | TimeWarp.Enterprises | Matches policy + nuru reference |

#### Validation
- PR-time: YAML review; secrets.NUGET_API_KEY gone; PR CI runs build-and-test only (publish skipped)
- Live: next GitHub Release (or workflow_dispatch with publish_to_nuget=true) — nuget/login succeeds and packages push
- After live success: operator revokes long-lived key + deletes GitHub secret (nuru 458-009)

#### Commit message
`ci: migrate NuGet publish to OIDC trusted publishing`

## Session
- Created: timewarp-nuru 458-009 rollout session (2026-08-08)
- Orchestration/Plan: grok session (2026-08-08)
- Implementation (2026-08-08): Updated `publish-nuget` in `.github/workflows/ci-cd.yml` — job permissions (`contents: read`, `id-token: write`), `nuget/login@v1` (user: TimeWarp.Enterprises), publish env uses `steps.nuget-login.outputs.NUGET_API_KEY`. No PowerShell or other job changes. Not committed (orchestrator will commit).

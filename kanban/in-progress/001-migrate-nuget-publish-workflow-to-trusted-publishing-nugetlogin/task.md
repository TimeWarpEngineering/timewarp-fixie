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

Code/review work is complete; operator residual remains (live verify on next release + secret revoke after success).

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
- Review (2026-08-08): Phase 4b round 1, effort 1 (general) — 0 open findings, disposition **clean**. Checklist 4–5 left open for operator post-merge (live publish verify + secret revoke).

## Results

### What was implemented
Migrated the `publish-nuget` job in `.github/workflows/ci-cd.yml` from long-lived `secrets.NUGET_API_KEY` to NuGet OIDC Trusted Publishing via `nuget/login@v1` (user: `TimeWarp.Enterprises`). Job-level permissions: `contents: read`, `id-token: write`. Publish step env now uses `steps.nuget-login.outputs.NUGET_API_KEY`. PowerShell publish path (`CI-CD/ci-cd.ps1`, `publish-nuget.ps1`) unchanged.

### Files changed
- `.github/workflows/ci-cd.yml` — sole product change
- `kanban/in-progress/001-migrate-nuget-publish-workflow-to-trusted-publishing-nugetlogin/` — task folderize, plan notes, review artifacts

### Key decisions
- Job-scoped permissions only (not workflow-wide)
- No step-level `if` on login (job `if` already gates release + workflow_dispatch publish)
- No reusable-workflow conversion (out of scope; 458-009 bootstrapped this minimal migration)
- Secret revocation deferred until after first successful live OIDC publish

### Test outcomes
- Static review: no remaining `secrets.NUGET_API_KEY` in workflow; output id matches `nuget-login`
- Phase 4b review: effort 1 (general), round 1, **0 open** findings, disposition **clean**
- Live NuGet push: **not run** in this session (requires next GitHub Release or intentional workflow_dispatch)

### Phase 4b review
- Rounds: 1
- Effort / roster: 1 — general
- Final counts: all severity rows 0 open / 0 fixed / 0 wontfix
- Disposition: **clean** (`review/disposition.md`)
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`

### How to validate

**Smoke (static / PR)**
```bash
# From repo root
rg -n "secrets\.NUGET_API_KEY|nuget/login|id-token" .github/workflows/ci-cd.yml
```
**Expect:**
- No `secrets.NUGET_API_KEY`
- `uses: nuget/login@v1` with `user: TimeWarp.Enterprises`
- `id-token: write` under `publish-nuget` `permissions`
- `NUGET_API_KEY: ${{ steps.nuget-login.outputs.NUGET_API_KEY }}`

**Smoke (CI on PR/push to master)**
- Open Actions for a non-release push/PR
**Expect:** `build-and-test` runs; `publish-nuget` is **skipped** (job `if` false)

**Live publish (post-merge operator)**
1. Publish a GitHub Release for TimeWarp.Fixie, **or** run workflow_dispatch with `publish_to_nuget=true` when a publish is intentional
2. Open the Actions run → `Publish to NuGet` job
**Expect:**
- Step `NuGet login (OIDC Trusted Publishing)` succeeds
- Publish step pushes packages (or skip-duplicate if version already on NuGet.org)
- No dependency on GitHub secret `NUGET_API_KEY`

**Automated gate**
None specific beyond normal PR CI (`build-and-test`). Local PowerShell publish still requires a real API key env and is not required to prove this YAML change.

**Depends on**
- NuGet.org trusted publishing policy for this repo already configured (owner TimeWarp.Enterprises; task states created 2026-08-08)
- GitHub Actions OIDC available on the job (`id-token: write`)

**Not in scope / residual operator work**
- End-to-end live publish proof (next release)
- After verified success: revoke long-lived NuGet API key and delete GitHub secret `NUGET_API_KEY` (org tracker: timewarp-nuru 458-009)

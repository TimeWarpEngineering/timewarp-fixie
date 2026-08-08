# Review framework — task 001

**Date:** 2026-08-08
**Host task:** kanban/in-progress/001-migrate-nuget-publish-workflow-to-trusted-publishing-nugetlogin/
**Diff scope:** commits on master ahead of origin/master; product change is `.github/workflows/ci-cd.yml` (publish-nuget OIDC migration); kanban task bookkeeping also present
**Plan / brief:** Migrate NuGet publish from `secrets.NUGET_API_KEY` to `nuget/login@v1` OIDC trusted publishing on the `publish-nuget` job only. PowerShell unchanged. Policy already exists on NuGet.org for TimeWarp.Enterprises.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** grok orchestration (2026-08-08)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`

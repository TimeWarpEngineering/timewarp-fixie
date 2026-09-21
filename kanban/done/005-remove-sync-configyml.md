# Remove sync-config.yml

## Description

Stop template sync noise by removing sync-config.yml (and related sync workflow/config copies if present). These files drive "Sync configurable files from parent repository" PRs.

## Requirements

- Delete `.github/sync-config.yml`
- Remove or disable any related sync-configurable-files workflow if present
- Close open sync PRs after merge if appropriate
- Do not reintroduce sync-config

## Checklist

- [x] Find all sync-config copies
- [x] Delete them
- [x] Remove related workflow if any
- [x] Commit
- [x] Verify no new sync PRs
- [x] Close stale sync PRs

## Notes

Created to stop recurring template-sync PRs from an active `.github/sync-config.yml`.

## Results

Removed the leftover parent-template sync drivers so this repo can no longer spawn "Sync configurable files from parent repository" PRs.

**What was deleted (this PR)**
- `.github/sync-config.yml`
- `.github/scripts/sync-configurable-files.ps1`

**Already gone (task 002, 2026-08-08)**
- `.github/workflows/sync-configurable-files.yml`
- `.github/workflows/sync-configurable-files.md`

**Not present**
- No `workflows-temp/` staging copies
- No `copy-workflows.ps1` (or other re-copy helpers)
- No `sync-configurable-files` workflow remaining under `.github/workflows/`
- No `commit-file-via-api.ps1` (only mentioned as a comment in the deleted script)

**Cleanup PR**
- URL: https://github.com/TimeWarpEngineering/timewarp-fixie/pull/15 (draft, left open; do not merge from this task)

**Stale sync PRs**
- None open. Both historical sync PRs were already closed on 2025-07-07:
  - #13 `Sync configurable files from parent repository` (branch `sync-configurable-files-1751275353`)
  - #14 `Sync configurable files from parent repository` (branch `sync-configurable-files-1751880182`)
- No deploy-sync / similarly titled abandoned sync PRs were open.

**Leftover remote branches (do not delete until after this cleanup PR is merged)**
- `sync-configurable-files-1751275353`
- `sync-configurable-files-1751880182`
- No `sync-deployment-*` branches found.

### How to validate

```bash
# expect: no matches
git ls-files | grep -E 'sync-config\.yml|sync-configurable-files'

# expect: only workflow.yml
ls .github/workflows/

# expect: no leftover driver
test ! -e .github/sync-config.yml
test ! -e .github/scripts/sync-configurable-files.ps1
```

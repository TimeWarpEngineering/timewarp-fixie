# Round 1 — general
**Date:** 2026-08-08
**Scope reviewed:** .github/workflows/ci-cd.yml publish-nuget OIDC migration

## Summary

The `publish-nuget` job correctly gains job-scoped `contents: read` + `id-token: write`, runs `nuget/login@v1` with `user: TimeWarp.Enterprises`, and wires `NUGET_API_KEY` from `steps.nuget-login.outputs.NUGET_API_KEY` so the existing PowerShell path is unchanged. No remaining `secrets.NUGET_API_KEY` in the workflow; non-goals (no secret deletion, no reusable-workflow conversion, no PS rewrites) are respected. Aligns with plan and `nuget/login@v1` contract.

## Issues

None.

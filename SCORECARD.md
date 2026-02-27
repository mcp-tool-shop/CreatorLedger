# Scorecard

> Score a repo before remediation. Fill this out first, then use SHIP_GATE.md to fix.

**Repo:** CreatorLedger
**Date:** 2026-02-27
**Type tags:** [cli] [nuget]

## Pre-Remediation Assessment

| Category | Score | Notes |
|----------|-------|-------|
| A. Security | 5/10 | No SECURITY.md (has SECURITY_IMPROVEMENTS.md), no README data scope |
| B. Error Handling | 8/10 | Ed25519 verification, structured CLI |
| C. Operator Docs | 7/10 | Good README, no CHANGELOG, multiple docs |
| D. Shipping Hygiene | 8/10 | CI, NuGet publish, dotnet tool |
| E. Identity (soft) | 10/10 | Logo, translations, landing page, metadata |
| **Overall** | **38/50** | |

## Key Gaps

1. No SECURITY.md — no vulnerability reporting process
2. No CHANGELOG.md
3. No Security & Data Scope in README

## Remediation Priority

| Priority | Item | Estimated effort |
|----------|------|-----------------|
| 1 | Create SECURITY.md + threat model in README | 5 min |
| 2 | Add CHANGELOG.md, bump to 1.1.2 | 5 min |
| 3 | Add SHIP_GATE.md + SCORECARD.md | 5 min |

## Post-Remediation

| Category | Before | After |
|----------|--------|-------|
| A. Security | 5/10 | 10/10 |
| B. Error Handling | 8/10 | 10/10 |
| C. Operator Docs | 7/10 | 10/10 |
| D. Shipping Hygiene | 8/10 | 10/10 |
| E. Identity (soft) | 10/10 | 10/10 |
| **Overall** | **38/50** | **50/50** |

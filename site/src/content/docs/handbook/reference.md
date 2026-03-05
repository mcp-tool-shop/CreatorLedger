---
title: CLI Reference
description: Complete command reference, exit codes, and scripting guidance for the CreatorLedger CLI verifier.
sidebar:
  order: 4
---

The CreatorLedger CLI is the primary interface for verifying proof bundles and inspecting attestation data. It is designed for both interactive use and integration into automated pipelines.

## Commands

### verify

Verify a proof bundle's cryptographic integrity and trust level.

```bash
creatorledger verify <proof-file> [options]
```

**Arguments:**

| Argument | Required | Description |
|----------|----------|-------------|
| `proof-file` | Yes | Path to the JSON proof bundle |

**Options:**

| Option | Description |
|--------|-------------|
| `--asset <path>` | Path to the original asset file. When provided, the verifier computes the SHA-256 hash of this file and compares it to the hash recorded in the attestation. |
| `--json` | Output results in machine-readable JSON format. Useful for CI pipelines and scripting. |

**Examples:**

```bash
# Basic verification
creatorledger verify proof.json

# Verify with content hash check
creatorledger verify proof.json --asset artwork.png

# JSON output for CI
creatorledger verify proof.json --json

# Combine asset check with JSON output
creatorledger verify proof.json --asset artwork.png --json
```

**Verification steps performed:**

1. Parse and validate the proof bundle JSON structure.
2. Walk the event chain, checking `PreviousEventHash` linkage.
3. Verify Ed25519 signatures on attestation events.
4. Compare content hash against the asset file (if `--asset` provided).
5. Assign and report the trust level.

### inspect

Examine the internal structure of a proof bundle without performing verification.

```bash
creatorledger inspect <proof-file>
```

**Arguments:**

| Argument | Required | Description |
|----------|----------|-------------|
| `proof-file` | Yes | Path to the JSON proof bundle |

The inspect command displays:

- Number of events in the chain.
- Event types and sequence numbers.
- Public key fingerprints.
- Content hashes and asset identifiers.
- Blockchain anchor references (if present).

This is useful for debugging, understanding bundle structure, or examining a proof before committing to full verification.

## Exit Codes

CreatorLedger uses specific exit codes to communicate verification outcomes. These are designed for reliable use in shell scripts and CI pipelines.

| Code | Status | Meaning |
|------|--------|---------|
| 0 | Verified | Signature valid, chain intact. Asset is Verified Original, Signed, or Derived. |
| 2 | Unverified | Bundle is structurally valid but cannot be cryptographically verified. |
| 3 | Broken | Tampering detected. Signature mismatch, chain break, or content hash mismatch. |
| 4 | Invalid input | The proof file is not valid JSON, uses an unsupported schema version, or is malformed. |
| 5 | Error | A runtime failure occurred (file not found, permission denied, unexpected exception). |

### Using Exit Codes in Scripts

**Bash:**

```bash
#!/bin/bash
creatorledger verify proof.json --asset artwork.png
code=$?

case $code in
  0) echo "Asset verified successfully" ;;
  2) echo "Warning: asset is unverified" ;;
  3) echo "ALERT: tampering detected" ; exit 1 ;;
  4) echo "Error: invalid proof bundle" ; exit 1 ;;
  5) echo "Error: runtime failure" ; exit 1 ;;
esac
```

**CI pipeline (GitHub Actions):**

```yaml
- name: Verify asset provenance
  run: |
    creatorledger verify proof.json --asset artwork.png --json
    if [ $? -ne 0 ]; then
      echo "::error::Asset verification failed"
      exit 1
    fi
```

**PowerShell:**

```powershell
creatorledger verify proof.json --asset artwork.png
switch ($LASTEXITCODE) {
    0 { Write-Host "Verified" }
    2 { Write-Warning "Unverified" }
    3 { Write-Error "Broken - tampering detected"; exit 1 }
    4 { Write-Error "Invalid input"; exit 1 }
    5 { Write-Error "Runtime error"; exit 1 }
}
```

## JSON Output Format

When using `--json`, the verifier outputs structured JSON to stdout:

```json
{
  "status": "verified",
  "trustLevel": "Signed",
  "events": 3,
  "contentHashMatch": true,
  "chain": {
    "intact": true,
    "length": 3
  }
}
```

Key fields:

| Field | Type | Description |
|-------|------|-------------|
| `status` | string | One of: `verified`, `unverified`, `broken`, `invalid`, `error` |
| `trustLevel` | string | One of: `Verified Original`, `Signed`, `Derived`, `Unverified`, `Broken` |
| `events` | number | Number of events in the chain |
| `contentHashMatch` | boolean or null | `true` if asset hash matches, `false` if mismatch, `null` if no asset provided |
| `chain.intact` | boolean | Whether the hash chain is unbroken |
| `chain.length` | number | Number of events in the chain |

## Platform Support

The CLI verifier runs on all platforms supported by .NET 8.0:

| Platform | Runtime | Secure Key Storage |
|----------|---------|-------------------|
| Windows x64 | .NET 8.0+ | DPAPI |
| Linux x64 | .NET 8.0+ | libsecret |
| macOS x64/arm64 | .NET 8.0+ | Keychain |

Self-contained builds include the .NET runtime, so end users do not need .NET installed:

```bash
# Windows
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained

# Linux
dotnet publish CreatorLedger.Cli -c Release -r linux-x64 --self-contained

# macOS (Apple Silicon)
dotnet publish CreatorLedger.Cli -c Release -r osx-arm64 --self-contained
```

<p align="center">
  <img src="assets/logo.png" alt="CreatorLedger" width="200" />
</p>

<p align="center">
  Local-first cryptographic provenance for digital assets.
</p>

<p align="center">
  <a href="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml"><img src="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml/badge.svg" alt="CI" /></a>
  <a href="https://www.nuget.org/packages/CreatorLedger"><img src="https://img.shields.io/nuget/v/CreatorLedger" alt="NuGet" /></a>
  <a href="https://github.com/mcp-tool-shop-org/CreatorLedger/blob/main/LICENSE"><img src="https://img.shields.io/github/license/mcp-tool-shop-org/CreatorLedger" alt="License" /></a>
</p>

---

CreatorLedger proves **who created what, when** — with Ed25519 signatures, append-only event chains, and optional blockchain anchoring. No cloud required.

## What It Does

- **Sign assets locally** — Ed25519 signatures tied to creator identity
- **Track derivation chains** — Know when work is derived from other work
- **Export self-contained proofs** — JSON bundles that verify without any database
- **Anchor to blockchain** — Optional timestamping for legal-grade evidence

## Install

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

## Trust Levels

| Level | Meaning |
|-------|---------|
| **Verified Original** | Signed + anchored to blockchain |
| **Signed** | Valid signature, not yet anchored |
| **Derived** | Signed work derived from another signed work |
| **Unverified** | No attestation found |
| **Broken** | Signature invalid or content modified |

## CLI Verifier

Verify proof bundles without any infrastructure:

```bash
# Verify a proof bundle
creatorledger verify proof.json

# Verify with asset file (checks content hash)
creatorledger verify proof.json --asset artwork.png

# Machine-readable output for CI
creatorledger verify proof.json --json

# Inspect bundle structure
creatorledger inspect proof.json
```

### Exit Codes

| Code | Status | Use in scripts |
|------|--------|----------------|
| 0 | Verified | `if creatorledger verify ...` |
| 2 | Unverified | Structurally valid, can't verify |
| 3 | Broken | Tamper detected |
| 4 | Invalid input | Bad JSON, wrong version |
| 5 | Error | Runtime failure |

## Quick Start

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     CreatorLedger.Cli                        │
│                  (standalone verifier)                       │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                  CreatorLedger.Application                   │
│    CreateIdentity │ AttestAsset │ Verify │ Export │ Anchor  │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                    CreatorLedger.Domain                      │
│      CreatorIdentity │ AssetAttestation │ LedgerEvent        │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                 CreatorLedger.Infrastructure                 │
│    SQLite (WAL) │ DPAPI / libsecret / Keychain │ NullAnchor │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                       Shared.Crypto                          │
│           Ed25519 │ SHA-256 │ Canonical JSON                 │
└─────────────────────────────────────────────────────────────┘
```

## Cryptographic Guarantees

- **Signatures**: Ed25519 (RFC 8032) with official test vectors
- **Hashing**: SHA-256 for content and event chain
- **Serialization**: Canonical JSON (deterministic, UTF-8, no BOM)
- **Key storage**: Cross-platform secure storage (see below)

## Event Chain

Events form an append-only chain where each event includes the hash of the previous:

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

The chain is enforced by:
- SQLite triggers (no UPDATE/DELETE)
- `seq` ordering (not timestamps)
- `PreviousEventHash` verification on append
- Optimistic concurrency control via row versioning

## Proof Bundle Format

Self-contained JSON for offline verification:

```json
{
  "version": "proof.v1",
  "algorithms": {
    "signature": "Ed25519",
    "hash": "SHA-256",
    "encoding": "UTF-8"
  },
  "assetId": "...",
  "attestations": [...],
  "creators": [...],
  "ledgerTipHash": "...",
  "anchor": null
}
```

## Platform Support

| Component | Windows | Linux | macOS |
|-----------|---------|-------|-------|
| CLI Verifier | ✅ | ✅ | ✅ |
| Core Library | ✅ | ✅ | ✅ |
| Secure Key Storage | ✅ DPAPI | ✅ libsecret | ✅ Keychain |
| InMemory KeyVault | ✅ | ✅ | ✅ |

### Linux Requirements

```bash
# Install libsecret for secure key storage
sudo apt install libsecret-tools  # Ubuntu/Debian
sudo dnf install libsecret        # Fedora/RHEL
```

### macOS

No additional dependencies — Keychain is built-in.

## License

MIT

## Status

**v1.1.1** — Security hardening + cross-platform:
- ✅ Identity creation with Ed25519 key pairs
- ✅ Asset attestation with signatures
- ✅ Derivation tracking
- ✅ Proof bundle export
- ✅ Standalone CLI verifier
- ✅ SQLite persistence with append-only enforcement
- ✅ Cross-platform secure key storage (DPAPI / libsecret / Keychain)
- ✅ Path traversal protection
- ✅ Optimistic concurrency control
- ✅ RFC 8032 test vectors
- ✅ 222 tests passing
- ✅ NuGet package via Trusted Publishing
- ⏳ Real blockchain adapter (Polygon planned)

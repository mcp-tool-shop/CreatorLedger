<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.zh.md">中文</a> | <a href="README.es.md">Español</a> | <a href="README.fr.md">Français</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.it.md">Italiano</a> | <a href="README.pt-BR.md">Português (BR)</a>
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/mcp-tool-shop-org/brand/main/logos/CreatorLedger/readme.png" alt="CreatorLedger" width="400">
</p>

<p align="center">
  <a href="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml"><img src="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml/badge.svg" alt="CI"></a>
  <a href="https://www.nuget.org/packages/CreatorLedger"><img src="https://img.shields.io/nuget/v/CreatorLedger" alt="NuGet"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow" alt="MIT License"></a>
  <a href="https://mcp-tool-shop-org.github.io/CreatorLedger/"><img src="https://img.shields.io/badge/Landing_Page-live-blue" alt="Landing Page"></a>
</p>

**Local-first cryptographic provenance for digital assets. Proves who created what, when — with Ed25519 signatures, append-only event chains, and optional blockchain anchoring. No cloud required.**

---

## What It Does

- **Sign assets locally** — Ed25519 signatures tied to creator identity
- **Track derivation chains** — Know when work is derived from other work
- **Export self-contained proofs** — JSON bundles that verify without any database
- **Anchor to blockchain** — Optional timestamping for legal-grade evidence

---

## Install

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## Trust Levels

| Level | Meaning |
|-------|---------|
| **Verified Original** | Signed + anchored to blockchain |
| **Signed** | Valid signature, not yet anchored |
| **Derived** | Signed work derived from another signed work |
| **Unverified** | No attestation found |
| **Broken** | Signature invalid or content modified |

---

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

---

## Quick Start

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## Architecture

```
CreatorLedger.Cli              (standalone verifier)
        │
CreatorLedger.Application      (CreateIdentity, AttestAsset, Verify, Export, Anchor)
        │
CreatorLedger.Domain           (CreatorIdentity, AssetAttestation, LedgerEvent)
        │
CreatorLedger.Infrastructure   (SQLite WAL, DPAPI / libsecret / Keychain, NullAnchor)
        │
Shared.Crypto                  (Ed25519, SHA-256, Canonical JSON)
```

---

## Cryptographic Guarantees

- **Signatures**: Ed25519 (RFC 8032) with official test vectors
- **Hashing**: SHA-256 for content and event chain
- **Serialization**: Canonical JSON (deterministic, UTF-8, no BOM)
- **Key storage**: Cross-platform secure storage (DPAPI / libsecret / Keychain)

---

## Event Chain

Events form an append-only chain where each event includes the hash of the previous:

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

The chain is enforced by SQLite triggers (no UPDATE/DELETE), `seq` ordering, `PreviousEventHash` verification, and optimistic concurrency control.

---

## Platform Support

| Component | Windows | Linux | macOS |
|-----------|---------|-------|-------|
| CLI Verifier | Yes | Yes | Yes |
| Core Library | Yes | Yes | Yes |
| Secure Key Storage | DPAPI | libsecret | Keychain |

---

## Documentation

- [VERIFICATION.md](VERIFICATION.md) — Verification guide
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — Security hardening details
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — NuGet publishing
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — Release notes

---

## License

[MIT](LICENSE)

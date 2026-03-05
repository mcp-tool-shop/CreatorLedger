---
title: Architecture
description: Solution layout, event chain mechanics, and cryptographic guarantees in CreatorLedger.
sidebar:
  order: 3
---

CreatorLedger follows clean architecture principles with five focused .NET projects. Each layer has a single responsibility, and dependencies flow inward from infrastructure to domain.

## Solution Layout

```
CreatorLedger.Cli              Standalone verifier CLI
        |
CreatorLedger.Application      Use cases: CreateIdentity, AttestAsset, Verify, Export, Anchor
        |
CreatorLedger.Domain           Core types: CreatorIdentity, AssetAttestation, LedgerEvent
        |
CreatorLedger.Infrastructure   Storage: SQLite WAL, DPAPI / libsecret / Keychain, NullAnchor
        |
Shared.Crypto                  Primitives: Ed25519, SHA-256, Canonical JSON
```

### CreatorLedger.Cli

The command-line entry point. Handles argument parsing, output formatting (human-readable and JSON), and exit code mapping. This is the layer users interact with directly.

The CLI is designed to be stateless -- it reads proof bundles and asset files, performs verification, and exits. It does not maintain any persistent state of its own.

### CreatorLedger.Application

The use-case layer. Each operation (CreateIdentity, AttestAsset, Verify, Export, Anchor) is a distinct use case with clear inputs and outputs. This layer orchestrates domain objects and infrastructure services without containing business logic itself.

### CreatorLedger.Domain

The core domain model. Defines `CreatorIdentity` (an Ed25519 key pair bound to a creator), `AssetAttestation` (a signed claim about a specific piece of content), and `LedgerEvent` (an entry in the append-only chain). All validation rules and invariants live here.

### CreatorLedger.Infrastructure

Concrete implementations of storage and platform services:

- **SQLite (WAL mode)** -- Persistent storage for event chains. Write-ahead logging provides crash recovery without sacrificing append-only semantics.
- **Secure key storage** -- Platform-native key management: DPAPI on Windows, libsecret on Linux, Keychain on macOS.
- **NullAnchor** -- Default blockchain adapter that does nothing. Real anchoring adapters (such as Polygon) plug in here.

### Shared.Crypto

Cryptographic primitives shared across all layers:

- **Ed25519** -- RFC 8032 compliant signing and verification. Includes official test vectors.
- **SHA-256** -- Content hashing for assets and event chain linking.
- **Canonical JSON** -- Deterministic serialization (UTF-8, sorted keys, no BOM) ensuring that the same data always produces the same bytes for signing and hashing.

## The Event Chain

Events form the backbone of CreatorLedger's integrity model. Every significant action (identity creation, asset attestation, blockchain anchoring) is recorded as an event in an append-only chain.

### Chain Structure

```
[Genesis] --hash--> [CreatorCreated] --hash--> [AssetAttested] --hash--> [LedgerAnchored]
```

Each event contains:

- **seq** -- A monotonically increasing sequence number.
- **EventType** -- The kind of event (Genesis, CreatorCreated, AssetAttested, LedgerAnchored).
- **Payload** -- Event-specific data (public key for identity creation, content hash for attestation, transaction ID for anchoring).
- **PreviousEventHash** -- The SHA-256 hash of the preceding event's canonical JSON representation.
- **Timestamp** -- When the event was created (local clock).

### Append-Only Enforcement

The chain's integrity is enforced at multiple levels:

1. **SQLite triggers** -- Database-level triggers prevent UPDATE and DELETE operations on the events table. Any attempt to modify or remove an event fails at the storage layer.
2. **Sequence ordering** -- Events must have consecutive `seq` values. Gaps are rejected.
3. **Hash linking** -- Each event's `PreviousEventHash` must match the actual SHA-256 hash of the preceding event. A mismatch indicates tampering.
4. **Optimistic concurrency** -- Concurrent writes are detected and rejected, preventing race conditions that could corrupt the chain.

### Why Append-Only Matters

An append-only chain means that history cannot be rewritten. Once an attestation is recorded, it stays recorded. Even if a bad actor gains access to the SQLite database, they cannot:

- Delete an event without breaking the hash chain.
- Modify an event without invalidating its hash (and therefore breaking all subsequent events).
- Insert an event out of order without violating sequence constraints.

Verification detects any of these tampering attempts and reports the asset as Broken.

## Cryptographic Guarantees

### Ed25519 Signatures (RFC 8032)

CreatorLedger uses Ed25519 for all signing operations. Ed25519 provides:

- **128-bit security level** -- Equivalent to RSA-3072 but with 32-byte keys and 64-byte signatures.
- **Deterministic signing** -- The same message and key always produce the same signature. No random number generator involved during signing, eliminating an entire class of implementation vulnerabilities.
- **Fast verification** -- Signature verification is computationally cheap, making batch verification practical for large proof bundles.

All Ed25519 implementations are validated against the official RFC 8032 test vectors.

### SHA-256 Content Hashing

Content hashing serves two purposes:

1. **Asset binding** -- The SHA-256 hash of the asset file is embedded in the attestation. Verification compares this stored hash against a fresh hash of the provided file.
2. **Chain linking** -- Each event's `PreviousEventHash` is the SHA-256 hash of the preceding event's canonical JSON. This creates the tamper-evident chain.

### Canonical JSON Serialization

For signatures and hashes to be reproducible, the input bytes must be deterministic. CreatorLedger uses canonical JSON:

- **UTF-8 encoding** without byte order mark (BOM).
- **Sorted keys** -- Object keys are sorted lexicographically, ensuring consistent ordering regardless of insertion order.
- **No trailing whitespace** or optional formatting.

This guarantees that the same logical data always serializes to the same byte sequence, which is essential for signature verification across different platforms and implementations.

## Proof Bundles

A proof bundle is a self-contained JSON file that includes everything needed to verify an asset's provenance:

- The event chain (or relevant subset).
- Ed25519 public keys for signature verification.
- Content hashes for asset binding.
- Blockchain anchor references (if applicable).

Proof bundles are designed to be portable. You can email them, store them alongside assets, or include them in distribution packages. The recipient does not need access to your database, your network, or any external service to verify the claims.

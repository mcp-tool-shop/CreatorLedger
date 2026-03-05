---
title: Security Model
description: Data scope, key storage, threat model, and security boundaries for CreatorLedger.
sidebar:
  order: 5
---

CreatorLedger is a local-first tool. Understanding its security model helps you make informed decisions about how to deploy it and what threats it does (and does not) protect against.

## Data Scope

### What CreatorLedger Accesses

- **Ed25519 key pairs** -- Generated and stored in platform-native secure storage. Private keys never leave the secure enclave.
- **Source files** -- Read-only access to compute SHA-256 content hashes. Files are streamed for hashing; they are not copied or uploaded.
- **Attestation records** -- Stored in a local SQLite database using WAL (Write-Ahead Logging) mode for crash recovery.

### What CreatorLedger Does NOT Access

- **No telemetry.** The tool collects no usage data, crash reports, or analytics. Nothing phones home.
- **No cloud services.** All operations are local. There are no API calls to external servers during normal operation.
- **No network access** (unless blockchain anchoring is explicitly invoked). The default `NullAnchor` adapter performs no network operations.

### Permissions Required

| Permission | Purpose | Scope |
|------------|---------|-------|
| File system read | Compute asset content hashes | Directories you specify |
| File system write | SQLite database, proof bundle export | Working directory |
| Platform secure storage | Ed25519 key management | DPAPI / libsecret / Keychain |

## Key Storage

Private keys are the most sensitive data in CreatorLedger. They are stored using platform-native secure storage mechanisms that provide hardware-backed or OS-level encryption.

### Windows: DPAPI

Data Protection API encrypts keys using credentials tied to the current Windows user account. Keys are bound to the user profile and cannot be extracted without the user's login credentials.

### Linux: libsecret

Keys are stored in the GNOME Keyring or KDE Wallet via the libsecret API. The keyring is unlocked when the user logs in and locked when the session ends.

### macOS: Keychain

Keys are stored in the macOS Keychain, which provides hardware-backed encryption on machines with a Secure Enclave. Access is gated by the user's login credentials and optional per-item access controls.

### Key Management Best Practices

- **Back up your keys.** If you lose access to your platform's secure storage (OS reinstall, hardware failure), you lose the ability to create new attestations with that identity. Existing proof bundles remain independently verifiable.
- **One identity per role.** If you work in multiple capacities (personal art, company work), create separate identities for each. This limits the blast radius if a key is compromised.
- **Rotate on compromise.** If you suspect a key has been compromised, create a new identity immediately. Existing attestations signed with the old key remain valid but should be flagged as using a potentially compromised key.

## Threat Model

### What CreatorLedger Protects Against

**Content tampering.** If someone modifies an attested asset (even a single byte), the SHA-256 content hash will not match. Verification reports the asset as Broken.

**Chain manipulation.** If someone attempts to insert, modify, or delete events in the chain, the `PreviousEventHash` linkage breaks. SQLite triggers prevent modification at the storage layer; verification detects it at the proof bundle layer.

**Signature forgery.** Without the private key, an attacker cannot produce a valid Ed25519 signature. The 128-bit security level of Ed25519 makes brute-force attacks computationally infeasible.

**Backdating claims (with blockchain anchoring).** When an attestation is anchored to a blockchain, the timestamp is recorded by the blockchain network. An attacker cannot claim an attestation existed before the block was mined.

### What CreatorLedger Does NOT Protect Against

**Local machine compromise.** If an attacker has full access to your machine (root/admin), they can access your secure storage and sign attestations with your identity. CreatorLedger cannot protect against a fully compromised host. Use standard system hardening practices.

**Pre-attestation theft.** CreatorLedger proves that a specific key attested a specific piece of content at a specific time. It does not prove that the key holder is the original creator. If someone steals your work and attests it first, their attestation will be valid. Blockchain anchoring mitigates this by establishing an early timestamp.

**Key loss.** If you lose access to your private key, you cannot create new attestations with that identity. You cannot recover the key from proof bundles (they contain only the public key). Back up your keys.

**Clock manipulation (without anchoring).** Local timestamps are based on the system clock. Without blockchain anchoring, an attacker with system access could set the clock to a false time before creating an attestation. Blockchain anchoring provides an independent, tamper-resistant timestamp.

## Blockchain Anchoring

Blockchain anchoring is optional and opt-in. When invoked, it:

1. Takes the current chain tip (the most recent event hash).
2. Submits it to a blockchain network as a transaction.
3. Records the transaction ID as a `LedgerAnchored` event in the local chain.

The blockchain serves as an independent timestamp witness. It does not store your content, your keys, or your proof bundle. It stores only a hash that proves the chain existed at the time of the block.

### Anchoring Adapter Pattern

CreatorLedger uses a pluggable adapter pattern for blockchain anchoring:

- **NullAnchor** (default) -- Does nothing. No network access, no blockchain interaction.
- **Additional adapters** -- Can be implemented for specific blockchain networks. The adapter interface requires only a method to submit a hash and return a transaction reference.

## Security Boundary Summary

| Boundary | Inside | Outside |
|----------|--------|---------|
| Cryptographic integrity | Content hashes, signatures, chain linking | Key management policies, backup procedures |
| Storage protection | SQLite triggers, append-only enforcement | OS-level access controls, disk encryption |
| Key security | Platform secure storage integration | Host compromise, physical access |
| Timestamp authority | Blockchain anchoring (when used) | Local clock trust (without anchoring) |

For the full security policy, see [SECURITY.md](https://github.com/mcp-tool-shop-org/CreatorLedger/blob/main/SECURITY.md) in the repository.

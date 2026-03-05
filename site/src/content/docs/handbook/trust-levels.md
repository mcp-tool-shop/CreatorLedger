---
title: Trust Levels
description: Understanding CreatorLedger's five trust classifications and verification statuses.
sidebar:
  order: 2
---

Every asset processed by CreatorLedger receives a trust classification. These levels form a clear hierarchy that tells you exactly how much confidence you can place in an asset's provenance claim.

## The Five Trust Levels

### Verified Original

The highest trust level. The asset has a valid Ed25519 signature **and** has been anchored to a blockchain with a verifiable timestamp.

This means:
- The creator's identity is cryptographically bound to the asset.
- The content hash matches the original file.
- A third-party blockchain confirms the attestation existed at a specific point in time.
- The event chain from genesis to anchoring is intact and tamper-free.

Use this level when you need legal-grade evidence of authorship and timing.

### Signed

The asset has a valid Ed25519 signature, but has not been anchored to a blockchain. The signature proves the creator's key was used to attest this specific content, and the event chain is intact.

This is the default state after creating an attestation. It provides strong cryptographic proof of authorship but lacks the independent timestamp that blockchain anchoring provides.

For many use cases, Signed is sufficient. Blockchain anchoring adds value primarily when you need to prove the *timing* of creation to a third party who does not trust your local clock.

### Derived

The asset is signed, but it is explicitly marked as derived from another signed work. The derivation chain links back to the original, preserving the full provenance history.

Derivation tracking is useful for:
- Remixes, adaptations, and translations.
- Assets that build on or incorporate other attested works.
- Audit trails where you need to show lineage from source to derivative.

A Derived asset has its own valid signature and its own event chain. The derivation link is an additional relationship, not a weakness.

### Unverified

The proof bundle is structurally valid JSON with a recognizable schema, but CreatorLedger cannot verify the cryptographic claims. This can happen when:

- The public key needed for verification is not available.
- The proof bundle is incomplete or was exported without all required events.
- The asset file was not provided for content hash comparison.

Unverified does not mean tampered. It means the tool lacks enough information to confirm or deny the claims. You may be able to resolve this by providing the missing asset file or obtaining the full proof bundle.

### Broken

The most serious status. CreatorLedger has detected a problem that indicates tampering or corruption:

- An Ed25519 signature does not match the content it claims to sign.
- The event chain has a gap: an event's `PreviousEventHash` does not match the actual hash of the preceding event.
- The content hash in the attestation does not match the provided asset file.

A Broken result means the proof bundle should not be trusted. The content, the chain, or the signature has been altered after the original attestation.

## Trust Level Summary

| Level | Signature | Chain Intact | Blockchain Anchor | Content Match |
|-------|-----------|-------------|-------------------|---------------|
| Verified Original | Valid | Yes | Yes | Yes |
| Signed | Valid | Yes | No | Yes |
| Derived | Valid | Yes | -- | Yes |
| Unverified | Cannot check | -- | -- | -- |
| Broken | Invalid | No | -- | No |

## How Trust Levels Map to Exit Codes

The CLI verifier returns specific exit codes for each trust outcome, making it straightforward to integrate into scripts and CI pipelines:

| Trust Level | Exit Code | Script Usage |
|-------------|-----------|--------------|
| Verified Original | 0 | `if creatorledger verify ...` |
| Signed | 0 | Same -- both verified states return 0 |
| Derived | 0 | Verified derivation is still a success |
| Unverified | 2 | Check `$?` for conditional handling |
| Broken | 3 | Non-zero signals failure |

See the [CLI Reference](/CreatorLedger/handbook/reference/) for the complete exit code table including input errors and runtime failures.

## Upgrading Trust Levels

Assets can move up the trust hierarchy:

- **Unverified to Signed**: Provide the missing public key or complete proof bundle and re-verify.
- **Signed to Verified Original**: Anchor the attestation to a blockchain. The anchoring process adds a `LedgerAnchored` event to the chain with the blockchain transaction reference.

Assets cannot move down intentionally. A Broken status is a detection result, not a demotion -- it means something has gone wrong with the data.

---
title: CreatorLedger Handbook
description: Complete guide to cryptographic provenance for digital assets with CreatorLedger.
sidebar:
  order: 0
---

Welcome to the CreatorLedger Handbook. This guide covers everything you need to know about using CreatorLedger to establish cryptographic provenance for your digital assets.

## What is CreatorLedger?

CreatorLedger is a local-first cryptographic provenance system for digital assets. It proves who created what, and when, using Ed25519 signatures, append-only event chains, and self-contained proof bundles. No cloud services are required. Everything runs on your machine, and proofs verify offline.

Whether you are an independent artist protecting original work, a development team tracking asset lineage, or a legal team needing timestamped evidence, CreatorLedger provides the cryptographic foundation to back your claims.

## Core Principles

- **Local-first.** Your keys, your data, your machine. Nothing leaves your system unless you choose to anchor to a blockchain.
- **Cryptographic proof over trust.** Every claim is backed by Ed25519 signatures and SHA-256 content hashes. Verification is deterministic.
- **Append-only integrity.** Events form a hash-linked chain. Once written, records cannot be altered or deleted. SQLite triggers enforce this at the storage layer.
- **Portable proofs.** Export self-contained JSON proof bundles that anyone can verify without access to your database, your network, or any third-party service.

## Handbook Contents

This handbook is organized into the following sections:

- **[Getting Started](/CreatorLedger/handbook/getting-started/)** -- Install CreatorLedger, build from source, and run your first verification.
- **[Trust Levels](/CreatorLedger/handbook/trust-levels/)** -- Understand the five trust classifications and how assets move between them.
- **[Architecture](/CreatorLedger/handbook/architecture/)** -- Explore the solution layout, event chain mechanics, and cryptographic guarantees.
- **[CLI Reference](/CreatorLedger/handbook/reference/)** -- Full command reference, exit codes, and scripting guidance.
- **[Security Model](/CreatorLedger/handbook/security/)** -- Data scope, key storage, threat model, and security boundaries.

## Who This Is For

- **Digital creators** who want provable authorship without relying on a platform.
- **Development teams** who need to track asset lineage and derivation across projects.
- **Legal and compliance teams** who require timestamped cryptographic evidence.
- **CI/CD pipelines** that need to verify asset integrity as part of automated workflows.

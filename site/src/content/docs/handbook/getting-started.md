---
title: Getting Started
description: Install CreatorLedger, build from source, and run your first proof verification.
sidebar:
  order: 1
---

This guide walks you through installing CreatorLedger and verifying your first proof bundle. The entire process takes under five minutes.

## Prerequisites

CreatorLedger is built on .NET. You will need the .NET SDK installed on your machine.

- **.NET 8.0 SDK or later** -- Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Any supported OS** -- Windows, Linux, or macOS

## Installation

### As a .NET Global Tool (Recommended)

The simplest way to install CreatorLedger is as a .NET global tool from NuGet:

```bash
dotnet tool install --global CreatorLedger
```

This makes the `creatorledger` command available system-wide. To update later:

```bash
dotnet tool update --global CreatorLedger
```

### Build from Source

If you prefer to build from source or want to contribute:

```bash
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet build
```

To produce a self-contained executable that runs without the .NET runtime:

```bash
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

Replace `win-x64` with `linux-x64` or `osx-arm64` as appropriate for your platform.

## Running Tests

CreatorLedger ships with 222 tests covering cryptographic operations, event chain integrity, proof export/import, and CLI behavior:

```bash
dotnet test
```

All tests should pass on any supported platform. If you encounter failures related to secure key storage, ensure the platform keychain service is available (DPAPI on Windows, libsecret on Linux, Keychain on macOS).

## Your First Verification

Once installed, the fastest way to see CreatorLedger in action is to verify an existing proof bundle:

```bash
# Verify a proof bundle
creatorledger verify proof.json
```

If you have the original asset file, you can verify that the content has not been modified:

```bash
# Verify proof + check content hash against the asset
creatorledger verify proof.json --asset artwork.png
```

For CI pipelines or scripting, use JSON output:

```bash
# Machine-readable output
creatorledger verify proof.json --json
```

To examine the internal structure of a proof bundle without verifying it:

```bash
# Inspect bundle structure
creatorledger inspect proof.json
```

## What Happens During Verification

When you run `creatorledger verify`, the tool performs these checks in order:

1. **Parse** -- Reads the JSON proof bundle and validates its structure against the expected schema.
2. **Chain integrity** -- Walks the event chain from genesis to tip, verifying that each event's `PreviousEventHash` matches the SHA-256 hash of the preceding event.
3. **Signature verification** -- Validates the Ed25519 signature on each attestation event using the embedded public key.
4. **Content hash** (if `--asset` provided) -- Computes the SHA-256 hash of the asset file and compares it to the hash recorded in the attestation.
5. **Trust classification** -- Assigns a trust level (Verified Original, Signed, Derived, Unverified, or Broken) based on the results.

The exit code tells you the outcome. See the [CLI Reference](/CreatorLedger/handbook/reference/) for the full exit code table.

## Next Steps

- Learn about the [Trust Levels](/CreatorLedger/handbook/trust-levels/) that CreatorLedger assigns to assets.
- Explore the [Architecture](/CreatorLedger/handbook/architecture/) to understand how the event chain and cryptographic layers work together.
- Read the [Security Model](/CreatorLedger/handbook/security/) to understand data boundaries and threat considerations.

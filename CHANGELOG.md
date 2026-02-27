# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [1.1.2] - 2026-02-27

### Added
- SECURITY.md with vulnerability reporting and data scope
- SHIP_GATE.md quality gates (all hard gates pass)
- SCORECARD.md with pre/post remediation scores
- Security & Data Scope section in README

### Changed
- Patch bump from v1.1.1 to v1.1.2

## [1.1.1] - 2026-02-26

### Added
- Landing page using @mcptoolshop/site-theme
- Translations (7 languages) via polyglot-mcp

## [1.1.0] - 2026-02-25

### Added
- Cross-platform secure key storage (DPAPI/libsecret/Keychain)
- Blockchain anchoring support (opt-in)
- Security hardening improvements

## [1.0.0] - 2026-02-24

### Added
- Initial stable release
- CreatorLedger CLI standalone verifier
- Ed25519 signature generation and verification
- SHA-256 content hashing with canonical JSON
- Append-only event chain with SQLite WAL
- Asset attestation and creator identity management
- NuGet package publishing

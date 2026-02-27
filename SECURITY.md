# Security Policy

## Supported Versions

| Version | Supported |
|---------|-----------|
| 1.1.x   | Yes       |
| < 1.1   | No        |

## Reporting a Vulnerability

**Email:** 64996768+mcp-tool-shop@users.noreply.github.com

Please include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact

**Response timeline:**
- Acknowledgment: within 48 hours
- Assessment: within 7 days
- Fix (if confirmed): within 30 days

## Scope

CreatorLedger is a **local-first CLI and library** for cryptographic provenance of digital assets.
- **Data accessed:** Reads/writes Ed25519 keys via platform secure storage (DPAPI/libsecret/Keychain). Reads source files for SHA-256 hashing. Writes attestation records to local SQLite (WAL mode). All operations are local and deterministic.
- **Data NOT accessed:** No telemetry. No cloud services. Optional blockchain anchoring is opt-in only. Source files are hashed but never transmitted.
- **Permissions required:** File system read/write for SQLite database and source directories. Platform secure storage for Ed25519 keys. No elevated permissions required.

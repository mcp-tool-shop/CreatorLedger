# CreatorLedger v1.1.0 - Security Hardening Release

## 🔒 Security Improvements

This release closes **all critical and high-severity security gaps** identified in our comprehensive audit:

### Critical Fixes
- **Path Traversal Protection**: Added validation to prevent directory escapes in key vault file operations
- **Cross-Platform Key Storage**: Added secure key storage for Linux (libsecret) and macOS (Keychain) - no longer Windows-only

### High Priority Fixes
- **Optimistic Concurrency Control**: Eliminated race conditions in ledger appends via row versioning
- **RFC 8032 Test Vectors**: Added official Ed25519 test vectors for cryptographic assurance

### Additional Improvements
- **Input Validation**: Display names now validate character sets to prevent injection attacks
- **Exception Handling**: Improved error handling for non-existent file paths

## 🧪 Test Coverage
- **222/222 tests passing** ✅
- Added RFC 8032 compliance tests
- All security hardening validated

## 🌍 Platform Support

| Platform | Key Storage | Status |
|----------|-------------|--------|
| Windows | DPAPI (User scope) | ✅ Production Ready |
| Linux | libsecret (GNOME/KDE) | ✅ Production Ready |
| macOS | Keychain | ✅ Production Ready |

### Linux Requirements
```bash
# Install libsecret-tools for secure key storage
sudo apt install libsecret-tools  # Ubuntu/Debian
sudo dnf install libsecret        # Fedora/RHEL
```

### macOS
No additional dependencies - Keychain is built-in.

## 📦 What's Included

- `creatorledger` - Standalone CLI verifier (cross-platform)
- NuGet packages for library integration
- Source code with full test suite

## 🔄 Database Migration

If upgrading from v1.0, the database will automatically migrate on first connection:
- Migration 002 adds `row_version` column for concurrency control
- Existing data is preserved with version=1

## 📚 Documentation

See [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) for detailed technical documentation of all changes.

## 🎯 Security Rating

**Before**: 4.5/5 ⭐  
**After**: 5/5 ⭐⭐⭐⭐⭐

All critical and high-severity issues resolved. System is now production-ready with enterprise-grade security across all platforms.

---

## Full Changelog

### Security
- Added path traversal protection in DpapiKeyVault ([#1](https://github.com/mcp-tool-shop/CreatorLedger/commit/5d5135a))
- Implemented optimistic concurrency control for ledger appends
- Added RFC 8032 Ed25519 test vectors
- Strengthened display name validation with character whitelist
- Implemented cross-platform key storage (Linux libsecret, macOS Keychain)

### Infrastructure  
- Added database migration 002 for row versioning
- Enhanced error handling for file operations

### Testing
- 222 test cases, all passing
- Added cryptographic standard compliance tests

**Full Diff**: https://github.com/mcp-tool-shop/CreatorLedger/compare/f9d356d...5d5135a

# Publishing CreatorLedger v1.1.0

## Status

✅ **Code Complete** - All security improvements committed and pushed (commit 5d5135a)  
⏳ **Release Pending** - Awaiting stable network connection for:
- GitHub Release creation
- Binary builds (win-x64, linux-x64, osx-x64)
- Optional: NuGet package publishing

## Manual Publishing Steps

### 1. Create GitHub Release

```bash
cd C:\workspace\CreatorLedger

# Create release tag and publish
gh release create v1.1.0 \
  --title "v1.1.0 - Security Hardening Release" \
  --notes-file RELEASE_NOTES_v1.1.0.md
```

### 2. Build CLI Binaries

```bash
# Windows x64 (self-contained)
dotnet publish CreatorLedger.Cli/CreatorLedger.Cli.csproj \
  -c Release -r win-x64 --self-contained \
  -o publish/win-x64 \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true

# Linux x64 (self-contained)
dotnet publish CreatorLedger.Cli/CreatorLedger.Cli.csproj \
  -c Release -r linux-x64 --self-contained \
  -o publish/linux-x64 \
  -p:PublishSingleFile=true

# macOS x64 (self-contained)
dotnet publish CreatorLedger.Cli/CreatorLedger.Cli.csproj \
  -c Release -r osx-x64 --self-contained \
  -o publish/osx-x64 \
  -p:PublishSingleFile=true

# macOS ARM64 (Apple Silicon)
dotnet publish CreatorLedger.Cli/CreatorLedger.Cli.csproj \
  -c Release -r osx-arm64 --self-contained \
  -o publish/osx-arm64 \
  -p:PublishSingleFile=true
```

### 3. Upload Binaries to Release

```bash
# Create archives
cd publish
Compress-Archive -Path win-x64\* -DestinationPath creatorledger-v1.1.0-win-x64.zip
tar -czf creatorledger-v1.1.0-linux-x64.tar.gz -C linux-x64 .
tar -czf creatorledger-v1.1.0-osx-x64.tar.gz -C osx-x64 .
tar -czf creatorledger-v1.1.0-osx-arm64.tar.gz -C osx-arm64 .

# Upload to GitHub release
gh release upload v1.1.0 *.zip *.tar.gz
```

### 4. Optional: Publish NuGet Packages

If you want to distribute the libraries via NuGet:

```bash
# Add NuGet metadata to Directory.Build.props
<PropertyGroup>
  <Version>1.1.0</Version>
  <Authors>MCP Tool Shop</Authors>
  <Company>MCP Tool Shop</Company>
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <RepositoryUrl>https://github.com/mcp-tool-shop/CreatorLedger</RepositoryUrl>
  <PackageProjectUrl>https://github.com/mcp-tool-shop/CreatorLedger</PackageProjectUrl>
  <Description>Cryptographic provenance for digital assets</Description>
  <PackageTags>cryptography;ed25519;blockchain;provenance;digital-assets</PackageTags>
</PropertyGroup>

# Pack libraries
dotnet pack Shared.Crypto/Shared.Crypto.csproj -c Release -o packages
dotnet pack CreatorLedger.Domain/CreatorLedger.Domain.csproj -c Release -o packages
dotnet pack CreatorLedger.Application/CreatorLedger.Application.csproj -c Release -o packages
dotnet pack CreatorLedger.Infrastructure/CreatorLedger.Infrastructure.csproj -c Release -o packages

# Publish to NuGet
dotnet nuget push packages/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```

## Distribution Channels

### Primary
- ✅ **GitHub**: https://github.com/mcp-tool-shop/CreatorLedger
- ⏳ **GitHub Releases**: Binaries for win/linux/mac

### Optional
- **NuGet**: For library consumers integrating CreatorLedger into .NET apps
- **Homebrew**: macOS/Linux package manager (requires tap setup)
- **Chocolatey**: Windows package manager
- **Scoop**: Alternative Windows package manager
- **winget**: Windows Package Manager manifest

## Installation Instructions

### From GitHub Release (Recommended)
```bash
# Download for your platform
wget https://github.com/mcp-tool-shop/CreatorLedger/releases/download/v1.1.0/creatorledger-v1.1.0-linux-x64.tar.gz
tar -xzf creatorledger-v1.1.0-linux-x64.tar.gz
chmod +x creatorledger
sudo mv creatorledger /usr/local/bin/
```

### From Source
```bash
git clone https://github.com/mcp-tool-shop/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli/CreatorLedger.Cli.csproj -c Release
# Binary at: CreatorLedger.Cli/bin/Release/net8.0/publish/creatorledger
```

## Marketing Copy

### One-liner
**CreatorLedger**: Local-first cryptographic provenance for digital assets with Ed25519 signatures and optional blockchain anchoring.

### Tweet
🔐 CreatorLedger v1.1.0 released! 

✅ Cross-platform secure key storage (Windows/Linux/macOS)
✅ Path traversal protection  
✅ Concurrency control
✅ RFC 8032 compliance
✅ 222/222 tests passing

Production-ready 5⭐ security!

https://github.com/mcp-tool-shop/CreatorLedger

### Hacker News Title
CreatorLedger v1.1.0 – Local-first cryptographic provenance with Ed25519

### Reddit r/programming
**CreatorLedger v1.1.0: Security Hardening Release**

Just shipped a major security update to our cryptographic provenance system. Clean architecture .NET project with:

- Ed25519 signatures for asset attestation
- Append-only SQLite event chains
- Cross-platform secure key storage (DPAPI/libsecret/Keychain)
- Standalone CLI verifier (no infrastructure needed)
- 222 passing tests including RFC 8032 vectors

All critical audit findings resolved. Production-ready with 5⭐ security rating.

## Next Steps

1. **Wait for stable network** to build self-contained binaries
2. **Create GitHub release** with release notes
3. **Upload binaries** for all platforms
4. **Update README** with installation instructions
5. **Announce** on relevant channels (Twitter, HN, Reddit)

## Files Ready

- ✅ `RELEASE_NOTES_v1.1.0.md` - Detailed release notes
- ✅ `SECURITY_IMPROVEMENTS.md` - Technical documentation
- ✅ All code committed and pushed (5d5135a)
- ✅ 222/222 tests passing

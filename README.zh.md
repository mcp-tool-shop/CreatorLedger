<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.md">English</a> | <a href="README.es.md">Español</a> | <a href="README.fr.md">Français</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.it.md">Italiano</a> | <a href="README.pt-BR.md">Português (BR)</a>
</p>

<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.zh.md">中文</a> | <a href="README.es.md">Español</a> | <a href="README.fr.md">Français</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.it.md">Italiano</a> | <a href="README.pt-BR.md">Português (BR)</a>
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/mcp-tool-shop-org/CreatorLedger/main/assets/logo-creatorledger.png" alt="CreatorLedger" width="400">
</p>

<p align="center">
  <a href="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml"><img src="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml/badge.svg" alt="CI"></a>
  <a href="https://www.nuget.org/packages/CreatorLedger"><img src="https://img.shields.io/nuget/v/CreatorLedger" alt="NuGet"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow" alt="MIT License"></a>
  <a href="https://mcp-tool-shop-org.github.io/CreatorLedger/"><img src="https://img.shields.io/badge/Landing_Page-live-blue" alt="Landing Page"></a>
</p>

**用于数字资产的本地加密溯源。证明是谁创建了什么，以及何时创建——使用 Ed25519 签名、只追加事件链，以及可选的区块链锚定。无需云服务。**

---

## 功能

- **本地签名资产** — 与创作者身份关联的 Ed25519 签名
- **跟踪衍生链** — 了解作品是否是从其他作品衍生而来
- **导出自包含证明** — 验证无需任何数据库即可使用的 JSON 捆绑包
- **锚定到区块链** — 可选的时间戳，用于提供具有法律效力的证据

---

## 安装

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## 信任级别

| 级别 | 含义 |
| ------- | --------- |
| **Verified Original** | 已签名 + 锚定到区块链 |
| **Signed** | 签名有效，但尚未锚定 |
| **Derived** | 已签名，作品是从另一个已签名作品衍生而来 |
| **Unverified** | 未找到任何证明 |
| **Broken** | 签名无效或内容已修改 |

---

## 命令行验证器

无需任何基础设施即可验证证明捆绑包：

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

### 退出码

| 代码 | 状态 | 在脚本中使用 |
| ------ | -------- | ---------------- |
| 0 | 已验证 | `if creatorledger verify ...` |
| 2 | 未验证 | 结构有效，但无法验证 |
| 3 | 损坏 | 检测到篡改 |
| 4 | 无效输入 | JSON 格式错误，版本错误 |
| 5 | 错误 | 运行时错误 |

---

## 快速入门

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## 架构

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

## 加密保证

- **签名**：使用官方测试向量的 Ed25519 (RFC 8032)
- **哈希**：用于内容和事件链的 SHA-256
- **序列化**：规范 JSON (确定性，UTF-8，无 BOM)
- **密钥存储**：跨平台安全存储 (DPAPI / libsecret / Keychain)

---

## 事件链

事件形成一个只追加的链，每个事件都包含前一个事件的哈希：

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

该链由 SQLite 触发器（不支持 UPDATE/DELETE）、`seq` 顺序、`PreviousEventHash` 验证以及乐观并发控制强制执行。

---

## 平台支持

| 组件 | Windows | Linux | macOS |
| ----------- | --------- | ------- | ------- |
| 命令行验证器 | 是 | 是 | 是 |
| 核心库 | 是 | 是 | 是 |
| 安全密钥存储 | DPAPI | libsecret | Keychain |

---

## 文档

- [VERIFICATION.md](VERIFICATION.md) — 验证指南
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — 安全加固细节
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — NuGet 发布
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — 发布说明

---

## 许可证

[MIT](LICENSE)

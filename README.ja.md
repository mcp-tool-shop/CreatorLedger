<p align="center">
  <a href="README.md">English</a> | <a href="README.zh.md">中文</a> | <a href="README.es.md">Español</a> | <a href="README.fr.md">Français</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.it.md">Italiano</a> | <a href="README.pt-BR.md">Português (BR)</a>
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

**デジタル資産のための、ローカル環境に最適化された暗号学的トレーサビリティ。** 誰が何を作成したのか、いつ作成したのかを、Ed25519署名、追記専用のイベントチェーン、およびオプションのブロックチェーン連携によって証明します。 クラウド環境は不要です。

---

## 機能

- **資産をローカルで署名**：作成者のIDと紐付けられたEd25519署名
- **派生チェーンを追跡**：他の作品から派生した作品であるかどうかを把握
- **自己完結型の証明をエクスポート**：データベースなしで検証可能なJSON形式のデータ
- **ブロックチェーンに連携**：法的証拠として利用できるタイムスタンプの付与（オプション）

---

## インストール

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## 信頼レベル

| レベル | 意味 |
| ------- | --------- |
| **Verified Original** | ブロックチェーンに署名済みで連携済み |
| **Signed** | 署名あり、まだブロックチェーンに連携されていない |
| **Derived** | 別の署名済み作品から派生した作品 |
| **Unverified** | 認証情報が見つからない |
| **Broken** | 署名が無効、またはコンテンツが改ざんされている |

---

## CLI検証ツール

インフラストラクチャなしで証明データを検証します。

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

### 終了コード

| コード | ステータス | スクリプトでの利用 |
| ------ | -------- | ---------------- |
| 0 | 検証済み | `if creatorledger verify ...` |
| 2 | 未検証 | 構造的に有効だが、検証できない |
| 3 | エラー | 改ざんが検出された |
| 4 | 入力が無効 | JSON形式が不正、またはバージョンが間違っている |
| 5 | エラー | 実行時エラー |

---

## クイックスタート

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## アーキテクチャ

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

## 暗号学的保証

- **署名**: Ed25519 (RFC 8032) (公式のテストベクトル付き)
- **ハッシュ**: コンテンツとイベントチェーンに対してSHA-256を使用
- **シリアライゼーション**: カノニカルJSON (決定論的、UTF-8、BOMなし)
- **キーの保存**: クロスプラットフォーム対応のセキュアストレージ (DPAPI / libsecret / Keychain)

---

## イベントチェーン

イベントは、追記専用のチェーンを形成し、各イベントには前のイベントのハッシュが含まれます。

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

このチェーンは、SQLiteトリガー（UPDATE/DELETE不可）、`seq`の順序、`PreviousEventHash`の検証、および楽観的な同時実行制御によって強制されます。

---

## プラットフォームのサポート

| コンポーネント | Windows | Linux | macOS |
| ----------- | --------- | ------- | ------- |
| CLI検証ツール | はい | はい | はい |
| コアライブラリ | はい | はい | はい |
| セキュアキーストレージ | DPAPI | libsecret | Keychain |

---

## ドキュメント

- [VERIFICATION.md](VERIFICATION.md) — 検証ガイド
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — セキュリティ強化の詳細
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — NuGet公開
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — リリースノート

---

## ライセンス

[MIT](LICENSE)

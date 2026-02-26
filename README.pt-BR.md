<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.zh.md">中文</a> | <a href="README.es.md">Español</a> | <a href="README.fr.md">Français</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.it.md">Italiano</a> | <a href="README.md">English</a>
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/mcp-tool-shop-org/brand/main/logos/CreatorLedger/readme.png" alt="CreatorLedger" width="400">
</p>

<p align="center">
  <a href="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml"><img src="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml/badge.svg" alt="CI"></a>
  <a href="https://www.nuget.org/packages/CreatorLedger"><img src="https://img.shields.io/nuget/v/CreatorLedger" alt="NuGet"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow" alt="MIT License"></a>
  <a href="https://mcp-tool-shop-org.github.io/CreatorLedger/"><img src="https://img.shields.io/badge/Landing_Page-live-blue" alt="Landing Page"></a>
</p>

**Rastreabilidade criptográfica local para ativos digitais. Comprova quem criou o quê e quando, com assinaturas Ed25519, cadeias de eventos somente para adição e ancoragem opcional em blockchain. Não requer nuvem.**

---

## O que ele faz

- **Assine ativos localmente** — Assinaturas Ed25519 vinculadas à identidade do criador.
- **Rastreie cadeias de derivação** — Saiba quando um trabalho é derivado de outro.
- **Exporte provas autônomas** — Pacotes JSON que verificam sem nenhum banco de dados.
- **Ancore no blockchain** — Marcação de tempo opcional para evidências de qualidade jurídica.

---

## Instalação

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## Níveis de Confiança

| Nível | Significado |
| ------- | --------- |
| **Verified Original** | Assinado + ancorado no blockchain |
| **Signed** | Assinatura válida, ainda não ancorada |
| **Derived** | Trabalho assinado derivado de outro trabalho assinado |
| **Unverified** | Nenhuma atestação encontrada |
| **Broken** | Assinatura inválida ou conteúdo modificado |

---

## Verificador de Linha de Comando (CLI)

Verifique pacotes de prova sem nenhuma infraestrutura:

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

### Códigos de Saída

| Código | Status | Uso em scripts |
| ------ | -------- | ---------------- |
| 0 | Verificado | `if creatorledger verify ...` |
| 2 | Não verificado | Estruturalmente válido, não é possível verificar |
| 3 | Corrompido | Detecção de adulteração |
| 4 | Entrada inválida | JSON inválido, versão errada |
| 5 | Erro | Falha em tempo de execução |

---

## Início Rápido

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## Arquitetura

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

## Garantias Criptográficas

- **Assinaturas**: Ed25519 (RFC 8032) com vetores de teste oficiais.
- **Hashing**: SHA-256 para conteúdo e cadeia de eventos.
- **Serialização**: JSON canônico (determinístico, UTF-8, sem BOM).
- **Armazenamento de chaves**: Armazenamento seguro multiplataforma (DPAPI / libsecret / Keychain).

---

## Cadeia de Eventos

Os eventos formam uma cadeia somente para adição, onde cada evento inclui o hash do evento anterior:

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

A cadeia é imposta por gatilhos do SQLite (sem UPDATE/DELETE), ordenação `seq`, verificação `PreviousEventHash` e controle de concorrência otimista.

---

## Suporte à Plataforma

| Componente | Windows | Linux | macOS |
| ----------- | --------- | ------- | ------- |
| Verificador de Linha de Comando (CLI) | Sim | Sim | Sim |
| Biblioteca Central | Sim | Sim | Sim |
| Armazenamento Seguro de Chaves | DPAPI | libsecret | Keychain |

---

## Documentação

- [VERIFICATION.md](VERIFICATION.md) — Guia de verificação
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — Detalhes de fortalecimento de segurança
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — Publicação NuGet
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — Notas da versão

---

## Licença

[MIT](LICENSE)

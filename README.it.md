<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.zh.md">中文</a> | <a href="README.es.md">Español</a> | <a href="README.fr.md">Français</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.md">English</a> | <a href="README.pt-BR.md">Português (BR)</a>
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

**Tracciabilità crittografica locale per asset digitali. Dimostra chi ha creato cosa e quando, con firme Ed25519, catene di eventi a sola aggiunta e ancoraggio opzionale alla blockchain. Non è necessaria alcuna soluzione cloud.**

---

## Cosa fa

- **Firma gli asset localmente** — Firme Ed25519 associate all'identità del creatore
- **Traccia le catene di derivazione** — Scopri quando un'opera deriva da un'altra
- **Esporta prove autonome** — Pacchetti JSON che verificano senza alcun database
- **Ancora alla blockchain** — Timestamping opzionale per prove di valore legale

---

## Installazione

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## Livelli di fiducia

| Livello | Significato |
| ------- | --------- |
| **Verified Original** | Firmato e ancorato alla blockchain |
| **Signed** | Firma valida, ma non ancora ancorata |
| **Derived** | Opera firmata derivata da un'altra opera firmata |
| **Unverified** | Nessuna attestazione trovata |
| **Broken** | Firma non valida o contenuto modificato |

---

## Verificatore CLI

Verifica i pacchetti di prova senza alcuna infrastruttura:

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

### Codici di uscita

| Codice | Stato | Utilizzo negli script |
| ------ | -------- | ---------------- |
| 0 | Verificato | `if creatorledger verify ...` |
| 2 | Non verificato | Strutturalmente valido, impossibile verificare |
| 3 | Corrotto | Manomissione rilevata |
| 4 | Input non valido | JSON non valido, versione errata |
| 5 | Errore | Errore durante l'esecuzione |

---

## Guida introduttiva

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## Architettura

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

## Garanzie crittografiche

- **Firme**: Ed25519 (RFC 8032) con vettori di test ufficiali
- **Hashing**: SHA-256 per contenuto e catena di eventi
- **Serializzazione**: JSON canonico (deterministico, UTF-8, senza BOM)
- **Archiviazione delle chiavi**: Archiviazione sicura multipiattaforma (DPAPI / libsecret / Keychain)

---

## Catena di eventi

Gli eventi formano una catena a sola aggiunta in cui ogni evento include l'hash del precedente:

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

La catena è applicata tramite trigger di SQLite (nessun UPDATE/DELETE), ordinamento `seq`, verifica `PreviousEventHash` e controllo di concorrenza ottimistico.

---

## Supporto per piattaforme

| Componente | Windows | Linux | macOS |
| ----------- | --------- | ------- | ------- |
| Verificatore CLI | Sì | Sì | Sì |
| Libreria principale | Sì | Sì | Sì |
| Archiviazione sicura delle chiavi | DPAPI | libsecret | Keychain |

---

## Documentazione

- [VERIFICATION.md](VERIFICATION.md) — Guida alla verifica
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — Dettagli sul miglioramento della sicurezza
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — Pubblicazione NuGet
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — Note di rilascio

---

## Licenza

[MIT](LICENSE)

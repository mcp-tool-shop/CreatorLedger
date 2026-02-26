<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.zh.md">中文</a> | <a href="README.es.md">Español</a> | <a href="README.md">English</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.it.md">Italiano</a> | <a href="README.pt-BR.md">Português (BR)</a>
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

**Traçabilité cryptographique locale pour les actifs numériques. Permet de prouver qui a créé quoi et quand, grâce aux signatures Ed25519, aux chaînes d'événements en append-only (uniquement en ajout) et à l'ancrage optionnel sur une blockchain. Pas de cloud requis.**

---

## Ce que cela fait

- **Signature des actifs localement** — Signatures Ed25519 liées à l'identité du créateur.
- **Suivi des chaînes de dérivation** — Permet de savoir quand un travail est dérivé d'un autre.
- **Exportation de preuves autonomes** — Fichiers JSON qui permettent la vérification sans base de données.
- **Ancrage à la blockchain** — Possibilité de créer des horodatages pour des preuves juridiquement valables.

---

## Installation

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## Niveaux de confiance

| Niveau | Signification |
| ------- | --------- |
| **Verified Original** | Signature ajoutée et ancrée à la blockchain |
| **Signed** | Signature valide, mais pas encore ancrée |
| **Derived** | Travail signé dérivé d'un autre travail signé |
| **Unverified** | Aucune attestation trouvée |
| **Broken** | Signature invalide ou contenu modifié |

---

## Vérificateur en ligne de commande (CLI)

Vérifiez les ensembles de preuves sans aucune infrastructure :

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

### Codes de sortie

| Code | Statut | Utilisation dans les scripts |
| ------ | -------- | ---------------- |
| 0 | Vérifié | `if creatorledger verify ...` |
| 2 | Non vérifié | Structure valide, impossible de vérifier |
| 3 | Corrompu | Altération détectée |
| 4 | Entrée invalide | JSON incorrect, mauvaise version |
| 5 | Erreur | Erreur d'exécution |

---

## Démarrage rapide

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## Architecture

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

## Garanties cryptographiques

- **Signatures**: Ed25519 (RFC 8032) avec vecteurs de test officiels.
- **Hachage**: SHA-256 pour le contenu et la chaîne d'événements.
- **Sérialisation**: JSON canonique (déterministe, UTF-8, sans BOM).
- **Stockage des clés**: Stockage sécurisé multiplateforme (DPAPI / libsecret / Keychain).

---

## Chaîne d'événements

Les événements forment une chaîne en append-only où chaque événement inclut le hachage de l'événement précédent :

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

La chaîne est appliquée par des déclencheurs SQLite (pas de UPDATE/DELETE), l'ordre de `seq`, la vérification de `PreviousEventHash` et le contrôle de concurrence optimiste.

---

## Prise en charge des plateformes

| Composant | Windows | Linux | macOS |
| ----------- | --------- | ------- | ------- |
| CLI Verifier | Oui | Oui | Oui |
| Bibliothèque de base | Oui | Oui | Oui |
| Stockage sécurisé des clés | DPAPI | libsecret | Keychain |

---

## Documentation

- [VERIFICATION.md](VERIFICATION.md) — Guide de vérification
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — Détails des améliorations de sécurité
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — Publication NuGet
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — Notes de version

---

## Licence

[MIT](LICENSE)

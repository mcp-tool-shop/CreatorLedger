<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.zh.md">中文</a> | <a href="README.md">English</a> | <a href="README.fr.md">Français</a> | <a href="README.hi.md">हिन्दी</a> | <a href="README.it.md">Italiano</a> | <a href="README.pt-BR.md">Português (BR)</a>
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

**Trazabilidad criptográfica local para activos digitales. Demuestra quién creó qué y cuándo, con firmas Ed25519, cadenas de eventos de solo escritura y anclaje opcional a la cadena de bloques. No se requiere conexión a la nube.**

---

## ¿Qué hace?

- **Firma los activos localmente:** Firmas Ed25519 vinculadas a la identidad del creador.
- **Realiza un seguimiento de las cadenas de derivación:** Averigua cuándo un trabajo se deriva de otro.
- **Exporta pruebas autocontenidas:** Paquetes JSON que verifican sin necesidad de una base de datos.
- **Ancla a la cadena de bloques:** Sello de tiempo opcional para evidencia de calidad legal.

---

## Instalación

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## Niveles de confianza

| Nivel | Significado |
| ------- | --------- |
| **Verified Original** | Firmado y anclado a la cadena de bloques |
| **Signed** | Firma válida, pero aún no anclada |
| **Derived** | Trabajo firmado derivado de otro trabajo firmado |
| **Unverified** | No se encontró ninguna certificación |
| **Broken** | Firma inválida o contenido modificado |

---

## Verificador de línea de comandos (CLI)

Verifica paquetes de pruebas sin ninguna infraestructura:

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

### Códigos de salida

| Código | Estado | Uso en scripts |
| ------ | -------- | ---------------- |
| 0 | Verificado | `if creatorledger verify ...` |
| 2 | No verificado | Estructuralmente válido, no se puede verificar |
| 3 | Corrupto | Se detectó manipulación |
| 4 | Entrada inválida | JSON incorrecto, versión incorrecta |
| 5 | Error | Fallo en tiempo de ejecución |

---

## Guía de inicio rápido

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## Arquitectura

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

## Garantías criptográficas

- **Firmas:** Ed25519 (RFC 8032) con vectores de prueba oficiales.
- **Hashing:** SHA-256 para contenido y cadena de eventos.
- **Serialización:** JSON canónico (determinista, UTF-8, sin BOM).
- **Almacenamiento de claves:** Almacenamiento seguro multiplataforma (DPAPI / libsecret / Keychain).

---

## Cadena de eventos

Los eventos forman una cadena de solo escritura donde cada evento incluye el hash del evento anterior:

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

La cadena se impone mediante disparadores de SQLite (sin UPDATE/DELETE), ordenamiento `seq`, verificación `PreviousEventHash` y control de concurrencia optimista.

---

## Soporte de plataforma

| Componente | Windows | Linux | macOS |
| ----------- | --------- | ------- | ------- |
| Verificador de línea de comandos (CLI) | Sí | Sí | Sí |
| Biblioteca central | Sí | Sí | Sí |
| Almacenamiento de claves seguro | DPAPI | libsecret | Keychain |

---

## Documentación

- [VERIFICATION.md](VERIFICATION.md) — Guía de verificación
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — Detalles de endurecimiento de seguridad
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — Publicación en NuGet
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — Notas de la versión

---

## Licencia

[MIT](LICENSE)

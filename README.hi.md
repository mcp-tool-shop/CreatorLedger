<p align="center">
  <a href="README.ja.md">日本語</a> | <a href="README.zh.md">中文</a> | <a href="README.es.md">Español</a> | <a href="README.fr.md">Français</a> | <a href="README.md">English</a> | <a href="README.it.md">Italiano</a> | <a href="README.pt-BR.md">Português (BR)</a>
</p>

<p align="center">
  
            <img src="https://raw.githubusercontent.com/mcp-tool-shop-org/brand/main/logos/CreatorLedger/readme.png"
           alt="CreatorLedger" width="400">
</p>

<p align="center">
  <a href="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml"><img src="https://github.com/mcp-tool-shop-org/CreatorLedger/actions/workflows/ci.yml/badge.svg" alt="CI"></a>
  <a href="https://www.nuget.org/packages/CreatorLedger"><img src="https://img.shields.io/nuget/v/CreatorLedger" alt="NuGet"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow" alt="MIT License"></a>
  <a href="https://mcp-tool-shop-org.github.io/CreatorLedger/"><img src="https://img.shields.io/badge/Landing_Page-live-blue" alt="Landing Page"></a>
</p>

**डिजिटल संपत्तियों के लिए स्थानीय-आधारित क्रिप्टोग्राफिक प्रमाणिकता।** यह प्रमाणित करता है कि किसने क्या बनाया, कब — Ed25519 हस्ताक्षर, केवल जोड़ने योग्य इवेंट श्रृंखलाओं और वैकल्पिक ब्लॉकचेन एकीकरण के साथ। इसके लिए किसी क्लाउड की आवश्यकता नहीं है।

---

## यह क्या करता है

- **स्थानीय रूप से संपत्तियों पर हस्ताक्षर करें** — Ed25519 हस्ताक्षर जो निर्माता की पहचान से जुड़े होते हैं।
- **उत्पत्ति श्रृंखलाओं को ट्रैक करें** — जानें कि कोई कार्य अन्य कार्यों से कैसे प्राप्त किया गया है।
- **स्वयं-निहित प्रमाणों का निर्यात करें** — JSON बंडल जो किसी भी डेटाबेस के बिना सत्यापन करते हैं।
- **ब्लॉकचेन से जोड़ें** — कानूनी-ग्रेड प्रमाण के लिए वैकल्पिक टाइमस्टैम्पिंग।

---

## इंस्टॉल करें

```bash
# As a .NET global tool (recommended)
dotnet tool install --global CreatorLedger

# Or build from source
git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git
cd CreatorLedger
dotnet publish CreatorLedger.Cli -c Release
```

---

## विश्वसनीयता स्तर

| स्तर | अर्थ |
| ------- | --------- |
| **Verified Original** | हस्ताक्षरित + ब्लॉकचेन से जुड़ा हुआ |
| **Signed** | मान्य हस्ताक्षर, अभी तक ब्लॉकचेन से नहीं जोड़ा गया |
| **Derived** | एक अन्य हस्ताक्षरित कार्य से प्राप्त कार्य पर हस्ताक्षर किया गया |
| **Unverified** | कोई प्रमाण नहीं मिला |
| **Broken** | हस्ताक्षर अमान्य है या सामग्री में बदलाव किया गया है |

---

## CLI सत्यापनकर्ता

किसी भी बुनियादी ढांचे के बिना प्रमाण बंडलों को सत्यापित करें:

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

### एग्जिट कोड

| कोड | स्थिति | स्क्रिप्ट में उपयोग करें |
| ------ | -------- | ---------------- |
| 0 | सत्यापित | `if creatorledger verify ...` |
| 2 | असत्यापित | संरचनात्मक रूप से मान्य, सत्यापन नहीं कर सकते |
| 3 | टूटा हुआ | छेड़छाड़ का पता चला |
| 4 | अमान्य इनपुट | खराब JSON, गलत संस्करण |
| 5 | त्रुटि | रनटाइम विफलता |

---

## शुरुआत

```bash
# Build
dotnet build

# Run tests (222 tests)
dotnet test

# Build self-contained CLI
dotnet publish CreatorLedger.Cli -c Release -r win-x64 --self-contained
```

---

## आर्किटेक्चर

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

## क्रिप्टोग्राफिक गारंटी

- **हस्ताक्षर**: Ed25519 (RFC 8032) आधिकारिक परीक्षण वैक्टर के साथ
- **हैशिंग**: सामग्री और इवेंट श्रृंखला के लिए SHA-256
- **सीरियलाइजेशन**: कैनोनिकल JSON (नियतात्मक, UTF-8, कोई BOM नहीं)
- **कुंजी भंडारण**: क्रॉस-प्लेटफ़ॉर्म सुरक्षित भंडारण (DPAPI / libsecret / Keychain)

---

## इवेंट श्रृंखला

घटनाएं एक केवल-जोड़ने योग्य श्रृंखला बनाती हैं जहां प्रत्येक घटना में पिछली घटना का हैश शामिल होता है:

```
[Genesis] ──hash──▶ [CreatorCreated] ──hash──▶ [AssetAttested] ──hash──▶ [LedgerAnchored]
```

श्रृंखला को SQLite ट्रिगर्स (कोई UPDATE/DELETE नहीं), `seq` ऑर्डरिंग, `PreviousEventHash` सत्यापन और आशावादी समवर्ती नियंत्रण द्वारा लागू किया जाता है।

---

## प्लेटफ़ॉर्म समर्थन

| घटक | विंडोज | लिनक्स | macOS |
| ----------- | --------- | ------- | ------- |
| CLI सत्यापनकर्ता | हाँ | हाँ | हाँ |
| कोर लाइब्रेरी | हाँ | हाँ | हाँ |
| सुरक्षित कुंजी भंडारण | DPAPI | libsecret | Keychain |

---

## प्रलेखन

- [VERIFICATION.md](VERIFICATION.md) — सत्यापन गाइड
- [SECURITY_IMPROVEMENTS.md](SECURITY_IMPROVEMENTS.md) — सुरक्षा सुधार विवरण
- [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) — NuGet प्रकाशन
- [RELEASE_NOTES_v1.1.0.md](RELEASE_NOTES_v1.1.0.md) — रिलीज़ नोट्स

---

## लाइसेंस

[MIT](LICENSE)

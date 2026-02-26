import type { SiteConfig } from '@mcptoolshop/site-theme';

export const config: SiteConfig = {
  title: 'CreatorLedger',
  description: 'Local-first cryptographic provenance system for digital assets',
  logoBadge: 'CL',
  brandName: 'CreatorLedger',
  repoUrl: 'https://github.com/mcp-tool-shop-org/CreatorLedger',
  footerText: 'MIT Licensed \u2014 built by <a href="https://github.com/mcp-tool-shop-org" style="color:var(--color-muted);text-decoration:underline">mcp-tool-shop-org</a>',

  hero: {
    badge: '.NET / Ed25519',
    headline: 'CreatorLedger,',
    headlineAccent: 'prove who created what, when.',
    description: 'Local-first cryptographic provenance for digital assets. Ed25519 signatures, append-only event chains, self-contained proof bundles, and optional blockchain anchoring \u2014 no cloud required.',
    primaryCta: { href: '#quick-start', label: 'Get started' },
    secondaryCta: { href: '#features', label: 'Learn more' },
    previews: [
      { label: 'Install', code: 'dotnet tool install --global CreatorLedger' },
      { label: 'Verify', code: 'creatorledger verify proof.json --asset artwork.png' },
      { label: 'Inspect', code: 'creatorledger inspect proof.json' },
    ],
  },

  sections: [
    {
      kind: 'features',
      id: 'features',
      title: 'Why CreatorLedger?',
      subtitle: 'Provenance that works offline and verifies anywhere.',
      features: [
        { title: 'Ed25519 Signatures', desc: 'RFC 8032 compliant signatures tied to creator identity. Official test vectors included.' },
        { title: 'Append-Only Chain', desc: 'Events form a hash-linked chain enforced by SQLite triggers. No updates, no deletes, no tampering.' },
        { title: 'Self-Contained Proofs', desc: 'Export JSON proof bundles that verify offline without any database or infrastructure.' },
        { title: 'Derivation Tracking', desc: 'Know when work is derived from other work. Full provenance chain from original to derivative.' },
        { title: 'Blockchain Anchoring', desc: 'Optional timestamping for legal-grade evidence. Pluggable adapter pattern (Polygon planned).' },
        { title: 'Cross-Platform', desc: 'Windows (DPAPI), Linux (libsecret), macOS (Keychain) \u2014 secure key storage on every platform.' },
      ],
    },
    {
      kind: 'code-cards',
      id: 'quick-start',
      title: 'Quick Start',
      cards: [
        {
          title: 'Install & verify',
          code: '# Install as .NET global tool\ndotnet tool install --global CreatorLedger\n\n# Verify a proof bundle\ncreatorledger verify proof.json\n\n# Verify with asset file\ncreatorledger verify proof.json --asset artwork.png\n\n# Machine-readable for CI\ncreatorledger verify proof.json --json',
        },
        {
          title: 'Build from source',
          code: 'git clone https://github.com/mcp-tool-shop-org/CreatorLedger.git\ncd CreatorLedger\n\n# Build\ndotnet build\n\n# Run 222 tests\ndotnet test\n\n# Self-contained CLI\ndotnet publish CreatorLedger.Cli -c Release',
        },
      ],
    },
    {
      kind: 'data-table',
      id: 'trust-levels',
      title: 'Trust Levels',
      subtitle: 'Every asset gets a verifiable trust classification.',
      columns: ['Level', 'Meaning'],
      rows: [
        ['Verified Original', 'Signed + anchored to blockchain'],
        ['Signed', 'Valid signature, not yet anchored'],
        ['Derived', 'Signed work derived from another signed work'],
        ['Unverified', 'No attestation found'],
        ['Broken', 'Signature invalid or content modified'],
      ],
    },
    {
      kind: 'data-table',
      id: 'exit-codes',
      title: 'CLI Exit Codes',
      subtitle: 'Designed for scripts and CI pipelines.',
      columns: ['Code', 'Status', 'Meaning'],
      rows: [
        ['0', 'Verified', 'Signature and chain valid'],
        ['2', 'Unverified', 'Structurally valid, can\u2019t verify'],
        ['3', 'Broken', 'Tamper detected'],
        ['4', 'Invalid input', 'Bad JSON, wrong version'],
        ['5', 'Error', 'Runtime failure'],
      ],
    },
    {
      kind: 'data-table',
      id: 'architecture',
      title: 'Solution Layout',
      subtitle: 'Clean architecture with five focused projects.',
      columns: ['Project', 'Purpose'],
      rows: [
        ['CreatorLedger.Cli', 'Standalone verifier CLI'],
        ['CreatorLedger.Application', 'CreateIdentity, AttestAsset, Verify, Export, Anchor'],
        ['CreatorLedger.Domain', 'CreatorIdentity, AssetAttestation, LedgerEvent'],
        ['CreatorLedger.Infrastructure', 'SQLite (WAL), DPAPI / libsecret / Keychain, NullAnchor'],
        ['Shared.Crypto', 'Ed25519, SHA-256, Canonical JSON'],
      ],
    },
    {
      kind: 'data-table',
      id: 'platform',
      title: 'Platform Support',
      columns: ['Component', 'Windows', 'Linux', 'macOS'],
      rows: [
        ['CLI Verifier', '\u2705', '\u2705', '\u2705'],
        ['Core Library', '\u2705', '\u2705', '\u2705'],
        ['Secure Key Storage', 'DPAPI', 'libsecret', 'Keychain'],
      ],
    },
  ],
};

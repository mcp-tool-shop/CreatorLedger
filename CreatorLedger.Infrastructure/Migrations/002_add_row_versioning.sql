-- Migration: 002_add_row_versioning
-- Adds row version columns for optimistic concurrency control
-- This prevents race conditions on concurrent ledger appends

-- Add row_version column to ledger_events (defaults to 1 for existing rows)
ALTER TABLE ledger_events ADD COLUMN row_version INTEGER NOT NULL DEFAULT 1;

-- Add row_version column to creators (for future use)
ALTER TABLE creators ADD COLUMN row_version INTEGER NOT NULL DEFAULT 1;

-- Create index for efficient version lookups during append
CREATE INDEX IF NOT EXISTS idx_ledger_events_seq_version ON ledger_events(seq, row_version);

#!/usr/bin/env bash
# Review probe: lets us tell whether the Copilot code review agent really executed a
# repository script during a review. Read-only, no network, no secrets.
# The nonce combines a runtime timestamp and the runner hostname, so a review comment
# that quotes it could not have been guessed from the diff.
set -euo pipefail

ts="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
host="$(hostname)"
head_sha="$(git rev-parse --short HEAD 2>/dev/null || echo unknown)"
nonce="$(printf '%s|%s|%s' "$ts" "$host" "$head_sha" | sha256sum | cut -c1-12)"

echo "PROBE_OK nonce=${nonce}"
echo "probe_time_utc=${ts}"
echo "runner_host=${host}"
echo "head=${head_sha}"
echo "files_changed_vs_main=$(git diff --name-only origin/main...HEAD 2>/dev/null | wc -l | tr -d ' ')"

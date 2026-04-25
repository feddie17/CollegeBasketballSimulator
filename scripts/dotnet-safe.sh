#!/usr/bin/env bash
set -euo pipefail

# Work around environments where MSBuild named pipes are blocked.
export DOTNET_CLI_RUN_MSBUILD_OUTOFPROC=0
export MSBUILDDISABLENODEREUSE=1

if [ "$#" -eq 0 ]; then
  echo "Usage: scripts/dotnet-safe.sh <dotnet-subcommand> [args...]"
  echo "Example: scripts/dotnet-safe.sh test tests/CBBSimulator.Core.Tests/CBBSimulator.Core.Tests.csproj"
  exit 1
fi

exec dotnet "$@" -m:1 -nodeReuse:false

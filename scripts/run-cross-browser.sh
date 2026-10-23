#!/usr/bin/env bash
set -uo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
RESULTS_SRC="$ROOT/Jefit.Test/bin/Release/net8.0/allure-results"
MERGED="$ROOT/allure-results"
BROWSERS="${BROWSERS:-chrome firefox}"

rm -rf "$MERGED" && mkdir -p "$MERGED"
dotnet build "$ROOT/JefitAutomation.sln" -c Release || exit 1

for browser in $BROWSERS; do
  echo "===== UI tests in $browser ====="
  dotnet test "$ROOT/Jefit.Test" -c Release --no-build \
    --settings "$ROOT/runsettings/$browser.runsettings" --filter "Category=ui" \
    --logger "trx;LogFileName=$browser.trx"
  cp -r "$RESULTS_SRC/." "$MERGED/"
done

echo "Browser=${BROWSERS// /, }" > "$MERGED/environment.properties"
echo "Done. Open the report: allure serve $MERGED"

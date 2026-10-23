param([string[]]$Browsers = @("chrome", "firefox"))

$root = Split-Path -Parent $PSScriptRoot
$resultsSrc = Join-Path $root "Jefit.Test\bin\Release\net8.0\allure-results"
$merged = Join-Path $root "allure-results"

Remove-Item $merged -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $merged | Out-Null

dotnet build (Join-Path $root "JefitAutomation.sln") -c Release
if ($LASTEXITCODE -ne 0) { exit 1 }

foreach ($browser in $Browsers) {
    Write-Host "===== UI tests in $browser =====" -ForegroundColor Cyan
    dotnet test (Join-Path $root "Jefit.Test") -c Release --no-build `
        --settings (Join-Path $root "runsettings\$browser.runsettings") --filter "Category=ui" `
        --logger "trx;LogFileName=$browser.trx"
    Copy-Item "$resultsSrc\*" $merged -Recurse -Force
}

"Browser=$($Browsers -join ', ')" | Set-Content (Join-Path $merged "environment.properties")
Write-Host "Done. Open the report: allure serve $merged" -ForegroundColor Green

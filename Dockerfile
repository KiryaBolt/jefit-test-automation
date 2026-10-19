# Образ агента Jenkins: .NET 8 SDK + Chromium + Firefox ESR
FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN apt-get update \
    && apt-get install -y --no-install-recommends chromium chromium-driver firefox-esr \
    && rm -rf /var/lib/apt/lists/*

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    JEFIT_TestSettings__IsHeadless=true \
    JEFIT_TestSettings__ChromeBinaryPath=/usr/bin/chromium

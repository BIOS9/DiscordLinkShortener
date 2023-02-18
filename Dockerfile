FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base

#####################
#PUPPETEER RECIPE
#####################
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget --no-verbose -O /tmp/chrome.deb https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb \
&& apt-get update \
&& apt-get install -y /tmp/chrome.deb --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf \
&& rm /tmp/chrome.deb

RUN groupadd -r pptruser && useradd -r -g pptruser -G audio,video pptruser \
    && mkdir -p /home/pptruser/Downloads \
    && chown -R pptruser:pptruser /home/pptruser

USER pptruser
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"
#####################
#END PUPPETEER RECIPE
#####################

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY src/* ./
RUN dotnet restore DiscordLinkShortener.csproj
RUN dotnet build DiscordLinkShortener.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish DiscordLinkShortener.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordLinkShortener.dll"]

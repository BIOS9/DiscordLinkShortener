FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/DiscordLinkShortener.csproj", "src/"]
RUN dotnet restore "DiscordLinkShortener.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DiscordLinkShortener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscordLinkShortener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordLinkShortener.dll"]

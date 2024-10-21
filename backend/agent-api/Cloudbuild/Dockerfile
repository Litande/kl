FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG token
WORKDIR /src

RUN dotnet nuget add source --username USERNAME --password $token --store-password-in-clear-text --name github "https://nuget.pkg.github.com/Plat4Me/index.json"

COPY ["Plat4Me.DialAgentApi/Plat4Me.DialAgentApi.csproj", "Plat4Me.DialAgentApi/"]
RUN dotnet restore "Plat4Me.DialAgentApi/Plat4Me.DialAgentApi.csproj"

COPY . .
RUN dotnet build "Plat4Me.DialAgentApi/Plat4Me.DialAgentApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Plat4Me.DialAgentApi/Plat4Me.DialAgentApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plat4Me.DialAgentApi.dll"]


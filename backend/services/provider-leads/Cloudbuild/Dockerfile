FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG token
WORKDIR /src

RUN dotnet nuget add source --username USERNAME --password $token --store-password-in-clear-text --name github "https://nuget.pkg.github.com/Plat4Me/index.json"

COPY ["Plat4Me.DialLeadProvider/Plat4Me.DialLeadProvider.csproj", "Plat4Me.DialLeadProvider/"]
RUN dotnet restore "Plat4Me.DialLeadProvider/Plat4Me.DialLeadProvider.csproj"

COPY . .
RUN dotnet build "Plat4Me.DialLeadProvider/Plat4Me.DialLeadProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Plat4Me.DialLeadProvider/Plat4Me.DialLeadProvider.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plat4Me.DialLeadProvider.dll"]


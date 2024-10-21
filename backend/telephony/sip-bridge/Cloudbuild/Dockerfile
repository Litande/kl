FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG token
WORKDIR /src
EXPOSE 80
EXPOSE 7790

RUN dotnet nuget add source --username USERNAME --password ${token} --store-password-in-clear-text --name github "https://nuget.pkg.github.com/Plat4Me/index.json"

COPY ["Plat4Me.DialSipBridge/Plat4Me.DialSipBridge.csproj", "Plat4Me.DialSipBridge/"]
RUN dotnet restore "Plat4Me.DialSipBridge/Plat4Me.DialSipBridge.csproj"

COPY . .
RUN dotnet build "Plat4Me.DialSipBridge/Plat4Me.DialSipBridge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Plat4Me.DialSipBridge/Plat4Me.DialSipBridge.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app

RUN mkdir -p /app/appdata
RUN mkdir -p /app/temp

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plat4Me.DialSipBridge.dll"]

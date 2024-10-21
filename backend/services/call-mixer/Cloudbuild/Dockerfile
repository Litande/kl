FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG token
WORKDIR /src
EXPOSE 80


RUN dotnet nuget add source --username USERNAME --password ${token} --store-password-in-clear-text --name github "https://nuget.pkg.github.com/Plat4Me/index.json"

COPY ["Plat4Me.DialCallRecordMixer/Plat4Me.DialCallRecordMixer.csproj", "Plat4Me.DialCallRecordMixer/"]
RUN dotnet restore "Plat4Me.DialCallRecordMixer/Plat4Me.DialCallRecordMixer.csproj"

COPY . .
RUN dotnet build "Plat4Me.DialCallRecordMixer/Plat4Me.DialCallRecordMixer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Plat4Me.DialCallRecordMixer/Plat4Me.DialCallRecordMixer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app

RUN mkdir -p /app/appdata
RUN mkdir -p /app/temp
RUN apt-get update && apt-get install -y --no-install-recommends --allow-unauthenticated ffmpeg \
        && rm -rf /var/lib/apt/lists/*


COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plat4Me.DialCallRecordMixer.dll"]

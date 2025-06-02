# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# --- Runtime Stage (smaller image) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production 
ENV ASPNETCORE_URLS=http://+:80 
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SkypointSocialBackend.dll"]

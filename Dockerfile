# 1️⃣ Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copier le fichier csproj et restaurer les packages
COPY *.csproj ./
RUN dotnet restore

# Copier le reste du code et publier
COPY . ./
RUN dotnet publish -c Release -o out

# 2️⃣ Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose le port utilisé par Render
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Démarrer l'application
ENTRYPOINT ["dotnet", "Gesresto.dll"]

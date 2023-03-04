#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Pokerit.Api.Docker/Pokerit.Api.Docker.csproj", "Pokerit.Api.Docker/"]
RUN dotnet restore "Pokerit.Api.Docker/Pokerit.Api.Docker.csproj"
COPY . .
WORKDIR "/src/Pokerit.Api.Docker"
RUN dotnet build "Pokerit.Api.Docker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pokerit.Api.Docker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pokerit.Api.Docker.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/Demo.Api/Demo.Api.csproj", "src/Demo.Api/"]
RUN dotnet restore "src/Demo.Api/Demo.Api.csproj"
COPY . .
WORKDIR "/src/src/Demo.Api"
RUN dotnet build "Demo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Api.dll"]




FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

#COPY ["CrudMongoDB.Api.csproj", "src/CrudMongoDB.Api/"]

#COPY ["src/CrudMongoDB.Api/CrudMongoDB.Api.csproj", "src/CrudMongoDB.Api/"]
#COPY ["src/CrudMongoDB.Application/CrudMongoDB.Application.csproj", "dist/CrudMongoDB.Application/"]
#COPY ["src/CrudMongoDB.Infra.Data/CrudMongoDB.Infra.Data.csproj", "dist/CrudMongoDB.Infra.Data/"]
#COPY ["src/CrudMongoDB.Domain/CrudMongoDB.Domain.csproj", "dist/CrudMongoDB.Domain/"]
#COPY ["src/CrudMongoDB.Infra.CrossCutting/CrudMongoDB.Infra.CrossCutting.csproj", "dist/CrudMongoDB.Infra.CrossCutting/"]

COPY ["CrudMongoDB.Api.csproj", "dist/CrudMongoDB.Api/"]
#COPY ["src/CrudMongoDB.Api/CrudMongoDB.Api.csproj", "dist/CrudMongoDB.Api/"]
#COPY ["src/CrudMongoDB.Application/CrudMongoDB.Application.csproj", "dist/CrudMongoDB.Application/"]
#COPY ["src/CrudMongoDB.Infra.Data/CrudMongoDB.Infra.Data.csproj", "dist/CrudMongoDB.Infra.Data/"]
#COPY ["src/CrudMongoDB.Domain/CrudMongoDB.Domain.csproj", "dist/CrudMongoDB.Domain/"]
#COPY ["src/CrudMongoDB.Infra.CrossCutting/CrudMongoDB.Infra.CrossCutting.csproj", "dist/CrudMongoDB.Infra.CrossCutting/"]

#RUN dotnet restore "src/CrudMongoDB.Api/CrudMongoDB.Api.csproj"
#RUN dotnet restore "CrudMongoDB.Api.csproj"
RUN dotnet restore "dist/CrudMongoDB.Api/CrudMongoDB.Api.csproj"

COPY . .
#WORKDIR "/src/src/CrudMongoDB.Api"
WORKDIR "CrudMongoDB.Api"
#RUN dotnet build "CrudMongoDB.Api.csproj" -c Release -o /app/build
RUN dotnet build "dist/CrudMongoDB.Api/CrudMongoDB.Api.csproj" -c Release -o /app/build

FROM build AS publish
#RUN dotnet publish "CrudMongoDB.Api.csproj" -c Release -o /app/publish
RUN dotnet publish "dist/CrudMongoDB.Api/CrudMongoDB.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrudMongoDB.Api.dll"]





















Backup sites
---------------------------------------------
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
#WORKDIR /app

# Copiar csproj e restaurar dependencias
#COPY *.csproj ./
#RUN dotnet restore
#
#RUN pwsh -Command Write-Host "CrudMongoDB.Api: Gerando uma nova imagem Docker e testando o PowerShell Core"

# Build da aplicacao
#COPY . ./
#RUN dotnet publish -c Release -o out

# Build da imagem
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#WORKDIR /app
#COPY --from=build-env /app/out .
#ENTRYPOINT ["dotnet", "CrudMongoDB.Api.dll"];




#FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS webapi-mongo:1.0
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS crudmongodb-api-3.1:1.0
#
#LABEL version="1.0" maintener="Lucas"
#
#WORKDIR /app
#
#COPY ./src/CrudMongoDB.Api/dist .
#
#ENTRYPOINT ["dotnet", "CrudMongoDB.Api.dll"];




#FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
#WORKDIR /src
#COPY ["src/CrudMongoDB.Api/CrudMongoDB.Api.csproj", "src/CrudMongoDB.Api/"]
#RUN dotnet restore "src/CrudMongoDB.Api/CrudMongoDB.Api.csproj"
#COPY . .
#WORKDIR "/src/src/CrudMongoDB.Api"
#RUN dotnet build "CrudMongoDB.Api.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "CrudMongoDB.Api.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "CrudMongoDB.Api.dll"]




# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/netcoreapp3.1/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "aspnetapp.dll"]














=> Compose
version: '3'

services:
  apimongo:
    image: crudmongodb-api-3.1:1.0
    build:
      context: .
    environment:
      - TesteAmbiente=Teste via Docker Compose
    ports:
      - "10000:80"
    networks:
      - mongo-compose-network

  #mongo:
  #  image: crudmongodb-mongo
  #  environment:
  #    #MONGO_INITDB_ROOT_USERNAME: root
  #    #MONGO_INITDB_ROOT_PASSWORD: MongoDB2019!
  #    MONGO_INITDB_ROOT_USERNAME: lucas
  #    MONGO_INITDB_ROOT_PASSWORD: e296cd9f

  #  ports:
  #    - "27017:27017"
  #  #volumes:
  #  #  - /home/renatogroffe/Desenvolvimento/Docker/Volumes/MongoDB:/data/db
  #  networks:
  #    - mongo-compose-network

networks: 
    mongo-compose-network:
      driver: bridge

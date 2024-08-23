#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/Library.WebAPI/Library.WebAPI.csproj", "Presentation/Library.WebAPI/"]
COPY ["Cores/Library.Application/Library.Application.csproj", "Cores/Library.Application/"]
COPY ["Cores/Library.Domain/Library.Domain.csproj", "Cores/Library.Domain/"]
COPY ["Infrustructure/Library.Data/Library.Data.csproj", "Infrustructure/Library.Data/"]
RUN dotnet restore "./Presentation/Library.WebAPI/Library.WebAPI.csproj"
COPY . .
WORKDIR "/src/Presentation/Library.WebAPI"
RUN dotnet build "./Library.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Library.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.WebAPI.dll"]
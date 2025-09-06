#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["XYZ.WShop.API/XYZ.WShop.API.csproj", "XYZ.WShop.API/"]
COPY ["XYZ.WShop.Application/XYZ.WShop.Application.csproj", "XYZ.WShop.Application/"]
COPY ["XYZ.WShop.Domain/XYZ.WShop.Domain.csproj", "XYZ.WShop.Domain/"]
COPY ["XZY.WShop.Infrastructure/XZY.WShop.Infrastructure.csproj", "XZY.WShop.Infrastructure/"]
RUN dotnet restore "./XYZ.WShop.API/XYZ.WShop.API.csproj"
COPY . .
WORKDIR "/src/XYZ.WShop.API"
RUN dotnet build "./XYZ.WShop.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./XYZ.WShop.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XYZ.WShop.API.dll"]
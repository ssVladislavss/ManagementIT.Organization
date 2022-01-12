#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OrganizationEntity.WebHost/OrganizationEntity.WebHost.csproj", "OrganizationEntity.WebHost/"]
COPY ["OrganizationEntity.Core/OrganizationEntity.Core.csproj", "OrganizationEntity.Core/"]
COPY ["OrganizationEntity.DataAccess/OrganizationEntity.DataAccess.csproj", "OrganizationEntity.DataAccess/"]
RUN dotnet restore "OrganizationEntity.WebHost/OrganizationEntity.WebHost.csproj"
COPY . .
WORKDIR "/src/OrganizationEntity.WebHost"
RUN dotnet build "OrganizationEntity.WebHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrganizationEntity.WebHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrganizationEntity.WebHost.dll"]
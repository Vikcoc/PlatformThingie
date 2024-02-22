FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# based on eshops on container
# https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Basket/Basket.API/Dockerfile

# the nuget and common stuff to be cached
# COPY "PlatformThingie.sln" "PlatformThingie.sln"
# COPY "Dependencies/Dependencies.csproj" "Dependencies/Dependencies.csproj"
# COPY "PlatformInterfaces/PlatformInterfaces.csproj" "PlatformInterfaces/PlatformInterfaces.csproj"
# COPY "WebPlatform/WebPlatform.csproj" "WebPlatform/WebPlatform.csproj"
# COPY "DefaultsAndStuff/DefaultsAndStuff.csproj" "DefaultsAndStuff/DefaultsAndStuff.csproj"

# RUN dotnet restore "PlatformThingie.sln"

COPY . .

WORKDIR /src/DefaultsWeb
RUN dotnet publish -c Release -o /app

FROM build AS publish

# the final 
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DefaultsWeb.dll", "--urls=http://localhost:80", "--contentRoot=."]
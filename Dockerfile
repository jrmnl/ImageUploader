FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /build
COPY src/. ./
RUN dotnet build "ImageUploader.sln" -c Release

FROM build AS test
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
RUN dotnet test "ImageUploader.IntegrationTests/ImageUploader.IntegrationTests.csproj" -c Release --no-build

FROM build AS publish
RUN dotnet publish "ImageUploader.Host/ImageUploader.Host.csproj" -c Release -o /app --no-build

FROM base AS runtime
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ImageUploader.Host.dll"]

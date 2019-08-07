FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /build
COPY src/ImageUploader.sln ./
COPY src/ImageUploader.Application.Contract/*.csproj ./ImageUploader.Application.Contract/
COPY src/ImageUploader.Application/*.csproj ./ImageUploader.Application/
COPY src/ImageUploader.Controllers/*.csproj ./ImageUploader.Controllers/
COPY src/ImageUploader.Host/*.csproj ./ImageUploader.Host/
RUN dotnet restore
COPY src/. ./
RUN dotnet build "ImageUploader.Host/ImageUploader.Host.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ImageUploader.Host/ImageUploader.Host.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ImageUploader.Host.dll"]
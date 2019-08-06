FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ImageUploader.sln ./
COPY ImageUploader.Application.Contract/*.csproj ./ImageUploader.Application.Contract/
COPY ImageUploader.Application/*.csproj ./ImageUploader.Application/
COPY ImageUploader.Controllers/*.csproj ./ImageUploader.Controllers/
COPY ImageUploader.Host/*.csproj ./ImageUploader.Host/
RUN dotnet restore
COPY . ./
RUN dotnet build "ImageUploader.Host/ImageUploader.Host.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ImageUploader.Host/ImageUploader.Host.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ImageUploader.Host/ImageUploader.Host.dll"]

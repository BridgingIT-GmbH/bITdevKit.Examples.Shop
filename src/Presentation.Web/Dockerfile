##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
## run this from sln folder >> docker build -f src/Presentation.Web/Dockerfile -t presweb1-image .
## https://github.com/dotnet/dotnet-docker/blob/main/samples/aspnetapp/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out --no-restore
 # --runtime ubuntu.16.10-x64 --self-contained

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Presentation.Web.dll"]
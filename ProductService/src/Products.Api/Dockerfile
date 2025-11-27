FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore src/Products.Api/Products.Api.csproj
RUN dotnet build src/Products.Api/Products.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/Products.Api/Products.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Products.Api.dll"]

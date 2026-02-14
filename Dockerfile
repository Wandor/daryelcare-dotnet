FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DaryelCare/DaryelCare.csproj DaryelCare/
RUN dotnet restore DaryelCare/DaryelCare.csproj
COPY . .
RUN dotnet publish DaryelCare/DaryelCare.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
COPY db/ /app/db/
EXPOSE 8080
ENTRYPOINT ["dotnet", "DaryelCare.dll"]

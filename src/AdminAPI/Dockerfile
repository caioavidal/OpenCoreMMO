FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AdminAPI/AdminAPI.csproj", "AdminAPI/"]
RUN dotnet restore "AdminAPI/AdminAPI.csproj"
COPY . .
WORKDIR "/src/AdminAPI"
RUN dotnet build "AdminAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdminAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminAPI.dll"]

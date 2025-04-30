FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["pp_repo.csproj", "./"]
RUN dotnet restore "pp_repo.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "pp_repo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pp_repo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pp_repo.dll"]

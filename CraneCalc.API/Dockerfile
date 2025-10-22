FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CraneCalc.API/CraneCalc.API.csproj", "CraneCalc.API/"]
COPY ["CraneCalc.Application/CraneCalc.Application.csproj", "CraneCalc.Application/"]
COPY ["CraneCalc.Domain/CraneCalc.Domain.csproj", "CraneCalc.Domain/"]
COPY ["CraneCalc.Infrastructure/CraneCalc.Infrastructure.csproj", "CraneCalc.Infrastructure/"]
RUN dotnet restore "CraneCalc.API/CraneCalc.API.csproj"
COPY . .
WORKDIR "/src/CraneCalc.API"
RUN dotnet build "CraneCalc.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CraneCalc.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CraneCalc.API.dll"]

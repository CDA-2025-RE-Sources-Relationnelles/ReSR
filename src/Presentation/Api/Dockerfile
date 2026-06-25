FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Presentation/Api/ReSR.Presentation.Api.csproj", "src/Presentation/Api/"]
RUN dotnet restore "src/Presentation/Api/ReSR.Presentation.Api.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Api"
RUN dotnet build "ReSR.Presentation.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ReSR.Presentation.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

RUN apt-get update && apt-get install -y libgssapi-krb5-2

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ReSR.Presentation.Api.dll"]
CMD []

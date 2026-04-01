FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 443
EXPOSE 80

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

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReSR.Presentation.Api.dll"]
CMD []

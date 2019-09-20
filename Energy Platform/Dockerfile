FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Energy Platform/Energy Platform.csproj", "Energy Platform/"]
RUN dotnet restore "Energy Platform/Energy Platform.csproj"
COPY . .
WORKDIR "/src/Energy Platform"
RUN dotnet build "Energy Platform.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Energy Platform.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Energy Platform.dll"]

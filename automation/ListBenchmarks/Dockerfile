FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ListBenchmarks/ListBenchmarks.csproj", "ListBenchmarks/"]
RUN dotnet restore "ListBenchmarks/ListBenchmarks.csproj"
COPY . .
WORKDIR "/src/ListBenchmarks"
RUN dotnet build "ListBenchmarks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ListBenchmarks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ListBenchmarks.dll"]

﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Todolist/Todolist.csproj", "Todolist/"]
RUN dotnet restore "Todolist/Todolist.csproj"
COPY . .
WORKDIR "/src/Todolist"
RUN dotnet build "Todolist.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Todolist.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Todolist.dll"]

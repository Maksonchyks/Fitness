FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FitnessApp.Communication.API/FitnessApp.Communication.API.csproj", "FitnessApp.Communication.API/"]
COPY ["FitnessApp.Communication.Application/FitnessApp.Communication.Application.csproj", "FitnessApp.Communication.Application/"]
COPY ["FitnessApp.Communication.Domain/FitnessApp.Communication.Domain.csproj", "FitnessApp.Communication.Domain/"]
COPY ["FitnessApp.Communication.Infrastructure/FitnessApp.Communication.Infrastructure.csproj", "FitnessApp.Communication.Infrastructure/"]

ENV DOTNET_NUGET_AUDIT=false
RUN dotnet restore "FitnessApp.Communication.API/FitnessApp.Communication.API.csproj"

COPY . .
WORKDIR "/src/FitnessApp.Communication.API"
RUN dotnet build "FitnessApp.Communication.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FitnessApp.Communication.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessApp.Communication.API.dll"]

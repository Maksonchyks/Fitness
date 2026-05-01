FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FitnessApp.Nutrition.API/FitnessApp.Nutrition.API.csproj", "FitnessApp.Nutrition.API/"]
COPY ["FitnessApp.Nutrition.Application/FitnessApp.Nutrition.Application.csproj", "FitnessApp.Nutrition.Application/"]
COPY ["FitnessApp.Nutrition.Domain/FitnessApp.Nutrition.Domain.csproj", "FitnessApp.Nutrition.Domain/"]
COPY ["FitnessApp.Nutrition.Infrastructure/FitnessApp.Nutrition.Infrastructure.csproj", "FitnessApp.Nutrition.Infrastructure/"]
RUN dotnet restore "FitnessApp.Nutrition.API/FitnessApp.Nutrition.API.csproj"
COPY . .
WORKDIR "/src/FitnessApp.Nutrition.API"
RUN dotnet build "FitnessApp.Nutrition.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FitnessApp.Nutrition.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessApp.Nutrition.API.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FitnessApp.Workout.API/FitnessApp.Workout.API.csproj", "FitnessApp.Workout.API/"]
COPY ["FitnessApp.Workout.Application/FitnessApp.Workout.Application.csproj", "FitnessApp.Workout.Application/"]
COPY ["FitnessApp.Workout.Domain/FitnessApp.Workout.Domain.csproj", "FitnessApp.Workout.Domain/"]
COPY ["FitnessApp.Workout.Infrastructure/FitnessApp.Workout.Infrastructure.csproj", "FitnessApp.Workout.Infrastructure/"]
RUN dotnet restore "FitnessApp.Workout.API/FitnessApp.Workout.API.csproj"
COPY . .
WORKDIR "/src/FitnessApp.Workout.API"
RUN dotnet build "FitnessApp.Workout.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FitnessApp.Workout.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessApp.Workout.API.dll"]

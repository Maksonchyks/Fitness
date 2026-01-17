FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FitnessApp.Identity.API/FitnessApp.Identity.API.csproj", "FitnessApp.Identity.API/"]
COPY ["FitnessApp.Identity.Application/FitnessApp.Identity.Application.csproj", "FitnessApp.Identity.Application/"]
COPY ["FitnessApp.Identity.Domain/FitnessApp.Identity.Domain.csproj", "FitnessApp.Identity.Domain/"]
COPY ["FitnessApp.Identity.Infrastructure/FitnessApp.Identity.Infrastructure.csproj", "FitnessApp.Identity.Infrastructure/"]

RUN dotnet restore "FitnessApp.Identity.API/FitnessApp.Identity.API.csproj"

COPY . .

WORKDIR "/src/FitnessApp.Identity.API"
RUN dotnet build "FitnessApp.Identity.API.csproj" -c Release -o /app/build
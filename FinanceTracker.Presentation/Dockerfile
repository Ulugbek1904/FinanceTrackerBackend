# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files in the correct order
COPY ["FinanceTracker.Presentation/FinanceTracker.Presentation.csproj", "FinanceTracker.Presentation/"]
COPY ["FinanceTracker.Infrastructure/FinanceTracker.Infrastructure.csproj", "FinanceTracker.Infrastructure/"]
COPY ["FinanceTracker.Models/FinanceTracker.Domain.csproj", "FinanceTracker.Domain/"]
COPY ["FinanceTracker.Services/FinanceTracker.Services.csproj", "FinanceTracker.Services/"]

# Restore dependencies
RUN dotnet restore "./FinanceTracker.Presentation/FinanceTracker.Presentation.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/FinanceTracker.Presentation"
RUN dotnet build "./FinanceTracker.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinanceTracker.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create LocalFileStorage directory in the final image
RUN mkdir -p /app/LocalFileStorage

# Copy published app
COPY --from=publish /app/publish .

# Render.com requires port 8080
EXPOSE 8080

# Set environment variables for Render.com
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "FinanceTracker.Presentation.dll"]

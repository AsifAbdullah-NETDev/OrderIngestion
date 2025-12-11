# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["OrderIngestion.Api/OrderIngestion.Api.csproj", "OrderIngestion.Api/"]
COPY ["OrderIngestion.Application/OrderIngestion.Application.csproj", "OrderIngestion.Application/"]
COPY ["OrderIngestion.Infrastructure/OrderIngestion.Infrastructure.csproj", "OrderIngestion.Infrastructure/"]

RUN dotnet restore "OrderIngestion.Api/OrderIngestion.Api.csproj"

# Copy the rest of the code
COPY . .

WORKDIR "/src/OrderIngestion.Api"
RUN dotnet publish "OrderIngestion.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Set environment variable to use the PORT assigned by Render
ENV ASPNETCORE_URLS=http://*:$PORT

# Expose the port (optional)
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "OrderIngestion.Api.dll"]

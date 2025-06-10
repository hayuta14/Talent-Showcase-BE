# TalentShowCase.API

## Project Overview

This is the backend API for the Talent Show Case application, built with ASP.NET Core. It provides authentication (registration, login, token refresh) using JWT, and interacts with a PostgreSQL database for data persistence and Redis for caching/token management. The API is designed to be a robust and scalable foundation for a talent showcase platform.

## Getting Started

### Prerequisites

- .NET SDK (version 8.0 or later)
- Docker and Docker Compose (for PostgreSQL and Redis)
- PostgreSQL client (e.g., DBeaver, pgAdmin) for database management (optional, but recommended)

### Setup

1.  **Clone the repository:**
    ```bash
    git clone <repository_url>
    cd TalentShowCase-BE/TalentShowCase.API
    ```

2.  **Start Database Services (PostgreSQL and Redis) using Docker Compose:**
    Ensure Docker Desktop (or Docker Engine) is running on your system.
    ```bash
    docker compose up -d
    ```
    This command will start two containers: one for PostgreSQL (on `localhost:5432`) and one for Redis (on `localhost:6379`).

3.  **Run Database Migrations:**
    First, ensure you have the `dotnet-ef` global tool installed. If not, install it:
    ```bash
    dotnet tool install --global dotnet-ef
    ```
    Then, apply the migrations to create the database schema and tables. Make sure your `talentshowcase` database in PostgreSQL is empty or does not exist before running this command to avoid conflicts.
    ```bash
    dotnet ef database update
    ```
    _If you encounter errors like "relation 'Users' already exists" or "password authentication failed for user 'sa'", please ensure you have deleted the `talentshowcase` database in your PostgreSQL client (e.g., DBeaver) and that your `appsettings.json` and `appsettings.Development.json` files have the correct PostgreSQL connection string with `Username=postgres`._

4.  **Configure Application Settings:**
    Verify your `appsettings.json` and `appsettings.Development.json` files for correct connection strings and JWT settings. Ensure the `JwtSettings:Secret` key is at least 32 characters long for `HMAC-SHA256`.

    `appsettings.json` (example):
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=talentshowcase;Username=postgres;Password=postgres;Trust Server Certificate=true;",
        "Redis": "localhost:6379"
      },
      "JwtSettings": {
        "Secret": "thisisalongandsecurejwtsecretkeyforapplicationsdevelopmentpurposes",
        "ExpirationInMinutes": 60
      },
      "CorsSettings": {
        "AllowedOrigins": [
          "http://localhost:3000",
          "http://localhost:5173"
        ]
      }
    }
    ```

    `appsettings.Development.json` (example):
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=talentshowcase;Username=postgres;Password=postgres;Trust Server Certificate=true;"
      },
      "JwtSettings": {
        "Secret": "thisisalongandsecurejwtsecretkeyforapplicationsdevelopmentpurposes",
        "ExpirationInMinutes": 120
      },
      "CorsSettings": {
        "AllowedOrigins": [
          "http://localhost:3000",
          "http://localhost:8080",
          "http://localhost:4200"
        ]
      }
    }
    ```

5.  **Run the Application:**
    Navigate to the project root directory (`TalentShowCase.API`) in your terminal and run:
    ```bash
    dotnet run
    ```
    The API will typically run on `https://localhost:5071` (or a similar port assigned by Kestrel). Check the console output for the exact URL where the application is listening.

## API Endpoints (via Swagger)

Once the application is running, you can access the Swagger UI for interactive API documentation and testing at:

`http://localhost:<your_api_port>/swagger`

(e.g., `http://localhost:5071/swagger`)

## Contributing

Feel free to contribute to this project. Please follow standard pull request procedures and coding best practices.

## License

[Optional: Add license information here, e.g., MIT License] 
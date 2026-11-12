# NASA APOD Gallery

An ASP.NET Core MVC application that fetches NASA Astronomy Picture of the Day records, saves them to SQL Server with ADO.NET, and displays them in a gallery.

## Requirements

- .NET 10 SDK
- SQL Server or SQL Server Express
- A NASA API key from [api.nasa.gov](https://api.nasa.gov/)

## Database setup

1. Open `database/CreateApodDatabase.sql` in SQL Server Management Studio and execute it. It creates `NASA_APOD_DB` and `dbo.Apod`, or adds the `SavedAt` column if the table already exists.
2. The default connection string targets `localhost\SQLEXPRESS` with Windows authentication. Update `ConnectionStrings:DefaultConnection` in `appsettings.json` if your SQL Server instance differs.

## Configure the NASA API key

From this project directory, store your key using .NET User Secrets:

```powershell
dotnet user-secrets set "Nasa:ApiKey" "YOUR_NASA_API_KEY"
```

When no key is configured, the application uses NASA's `DEMO_KEY`, which has lower request limits.

## Run

From this project directory:

```powershell
dotnet restore
dotnet run
```

Open the local URL printed by `dotnet run`. Select a date range and fetch APOD entries; previously saved dates are not inserted again.
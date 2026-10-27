# Minimal API Coupon Demo

An educational ASP.NET Core Minimal API for coupon management, demonstrating endpoint registration, validation and token-based authorization.

## Implementation highlights

- Coupon read/create/update/delete endpoints and a separate filtering/pagination exercise.
- FluentValidation, AutoMapper and repository-based EF Core access.
- Identity registration/login, JWT generation and an AdminOnly policy.
- Endpoint filters and typed API response models.

Public registration assigns the customer role. Administrative access must be provisioned separately. The existing endpoint-specific policies are learning examples; the registration regression suite is not a complete authorization audit of every endpoint.

## Repository map

- [Endpoints](MagicVilla_CouponAPI/Endpoints): authentication and coupon route registration.
- [Repository](MagicVilla_CouponAPI/Repository): data access and account operations.
- [Data](MagicVilla_CouponAPI/Data): EF Core context.
- [Models](MagicVilla_CouponAPI/Models): entities and request/response DTOs.
- [tests/Authorization.Tests](tests/Authorization.Tests): customer-role registration regressions.

## Requirements and configuration

Use an SDK capable of targeting .NET 8 and an isolated SQL Server development database. Package restore requires access to the configured NuGet feeds. These are existing .NET 8 examples; dependency and runtime modernization are separate work.

Read [SECURITY-SETUP.md](SECURITY-SETUP.md) before configuring credentials. Tracked secret values are intentionally empty. Use environment variables for your own development resources; do not restore old values from Git history.

Set `ConnectionStrings__DefaultConnection` for a prepared development database and `ApiSettings__Secret` for a locally generated JWT signing secret. Review the [migrations](MagicVilla_CouponAPI/Migrations) and prepare the schema through a separately approved database setup step. The run command below is not a migration command.

## Build and run

```powershell
git clone https://github.com/s-hesam-s/MinimalAPIDemo_Coupon.git
cd MinimalAPIDemo_Coupon
```

```powershell
dotnet restore MinimalAPIDemo.sln
dotnet build MinimalAPIDemo.sln --no-restore
```

After configuration and schema preparation:

```powershell
dotnet run --project MagicVilla_CouponAPI/MagicVilla_CouponAPI.csproj --launch-profile https
```

Open `https://localhost:7198/swagger` in the Development launch profile. Routes include `/api/login`, `/api/register`, `/api/coupon` and the separate `/api/coupon/special` example. Obtain an appropriate local token before testing protected endpoints; do not assume every account has admin access.

## Verification

```powershell
pwsh -File scripts/Test-PublicConfiguration.ps1
dotnet test tests/Authorization.Tests/Authorization.Tests.csproj
```

The authorization tests use mocked dependencies, with in-memory HTTP checks where applicable. They do not start the application or apply database migrations. Build and test results do not establish that a full deployment or external integration is configured.

## Project scope and reuse

This is an educational repository. The implementation notes describe the code in this repository; they do not claim a deployed commercial service or measured production results. Third-party libraries and assets retain their own terms. No project-wide license is currently declared.

# Local security configuration

This repository contains educational examples. Credential values are intentionally empty in tracked application settings. Supply your own development values through environment variables before starting the relevant application. ASP.NET Core maps a double underscore in an environment variable to a configuration colon.

| Application settings | Configuration key | Environment variable |
|---|---|---|
| `MagicVilla_CouponAPI/appsettings.json` | `ApiSettings:Secret` | `ApiSettings__Secret` |

For a JWT signing secret, use a locally generated random value of at least 32 bytes. Services that issue or validate the same JWT must use the same local secret, issuer and audience. Do not reuse a published training value. Do not commit local credentials or print them in logs.

PowerShell example for the current shell only:

```powershell
$env:ApiSettings__Secret = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
```

For applications using `ApiSettings:JwtOptions:Secret`, set `ApiSettings__JwtOptions__Secret` instead. Supply payment, storage, messaging and Redis settings only for the features you intend to run, using your own development resources. Empty settings are not fallback credentials.

Some examples apply Entity Framework migrations during application startup. Review startup behavior and select an isolated development database before running them. No migration or database update is performed by this configuration change.

Public registration must create only a customer account. Administrative accounts and role changes require a trusted administrative process; the registration page is not an admin bootstrap mechanism. Do not enable public admin registration to set up a demonstration.

Historical commits may still contain previous training values. If a value was ever connected to a real resource, its owner should revoke or rotate it; clearing the current files does not invalidate an old value or rewrite Git history.

## Regression tests

Run `dotnet test tests/Authorization.Tests/Authorization.Tests.csproj`. The tests replace Identity managers with mocks and never start the application or connect to a database, payment provider or messaging service.

Run `pwsh -File scripts/Test-PublicConfiguration.ps1` to check public configuration without displaying values.

## Coupon authorization and JWT contract

Coupon reads, creates, updates and deletes, including the special paginated query, require the `AdminOnly` policy and the `admin` role. Public registration cannot grant that role. Provision it only through a trusted local administration process.

The issuer and validator use the same UTF-8 signing key and configuration. Set `ApiSettings__Secret` to a locally generated random secret of at least 32 bytes. Optional `ApiSettings__Issuer` and `ApiSettings__Audience` default to `magicvilla-coupon` and `magicvilla-coupon-client`. Validation requires HS256, signing key, issuer, audience and expiration, with 30 seconds of clock skew. Existing tokens with the previous missing issuer/audience must be reissued.

Tests use signed fixture tokens and in-process HTTP, including anonymous/customer denial and invalid signature, issuer, audience and expiration. No original host, database or identity store is started.

# Config

Loads and reads configuration files (XML, JSON, or config), applies transformations, and decrypts encrypted properties.

## Configuration Locations

Files can be placed in:
- `~/*.json`, `~/*.xml`
- `~/Configs/**.[json|xml]`
- `~/Configurations/**.[json|xml]`
- Or appended to `appSettings.json`

## Environment Transformations

Transformations apply based on `ASPNETCORE_ENVIRONMENT`, set via:
- `launchSettings.json` (IIS Express)
- `web.config` (IIS)
- `mstest.runsettings` (unit tests)
- CLI (`--configuration`)

## Encryption Handling

Encrypted properties, such as `ApiPassword`, can be decrypted by creating a corresponding `ApiPasswordDecrypt` property. This will hold the decrypted value of ApiToken.

## Usage

- Create a YourConfig.[json|xml|config]
- Create a class YourConfig inheriting `Config<T>`, matching the file name.
- Access it via `YourConfig.Current`


## Environment-Specific Overrides

Create a transformation file (e.g., `YourConfig.dev.json`) in the same folder as the base config. Define only values to override.


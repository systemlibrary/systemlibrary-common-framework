# Config

Loads and reads configuration files (XML, JSON, or config), applies transformations, and decrypts encrypted properties.

## Configuration Locations

Files can be placed in:
- `~/*.json`, `~/*.xml`
- `~/Configs/**.[json|xml]`
- `~/Configurations/**.[json|xml]`
- Or appended to `appsettings.json`

## Environment Transformations

Transformations apply based on `ASPNETCORE_ENVIRONMENT`, set via:
- `launchSettings.json` (IIS Express)
- `web.config` (IIS)
- `mstest.runsettings` (unit tests)
- CLI (`--environment`)

## Encryption Handling

Encrypted properties, such as `ApiPassword`, can be decrypted by creating a corresponding `ApiPasswordDecrypt` property. This will hold the decrypted value of ApiPassword property.

## Usage

- Create a YourConfig.[json|xml|config]
- Add the 'ApiPassword' to configuration file, the encrypted version
- Create a class YourConfig inheriting `Config<T>`, matching the file name.
- Create two string properties `ApiPassword` and `ApiPasswordDecrypt`
- Access it via `YourConfig.Current.ApiPasswordDecrypt`


## Environment-Specific Overrides

Create a transformation file (e.g., `YourConfig.dev.json`) in the same folder as the base config. Define only values to override.


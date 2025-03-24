# SystemLibrary Common Framework

## Description
Framework with default settings and classes for every &gt;= .NET 8 application

### Features
- Preconfigured – Available via AddFrameworkServices() and AddFrameworkMiddlewares(), including HTTPS redirection, output caching, authentication, and more.
- Config – Inherit class to auto-map JSON config files to C# classes by name, with environment-based transformations.
- Log – Global instance with a Dump method, equivalent to JavaScript's console.log.
- Cache – Global sharded instance with fallback and metrics, and automatic cache key generation.
- Client – Http Client with automatic retry and circuit breaker policies and metrics
- JSON – .Json() and .PartialJson() extension methods for easy conversions.
- Encryption – Easily encrypt with global key management via file, CLI, or environment variable, using string and byte[] extensions, with AES CBC PKCS7 encryption.
- String Extensions – ToBase64, ToSHA256, ToMD5, ToSHA1, Obfuscate and Compress on strings and byte arrays.
- Services - Global instance service locator 
- Enhanced Enums – Attributes EnumText, EnumValue for flexible JSON deserialization.
- API – BaseApiController with attributes OriginFilter, UserAgentFilter and ApiTokenFilter attributes for request validation.
- Self-Documenting APIs – Auto-generates API documentation at /docs, listing methods, parameters, and paths. 

## Requirements
- &gt;= .NET 8

## Latest Release Notes
- 8.0.0.11
- Encryption searches parent up to 8 parent folders for a key file (fix)
- Encryption using a key file replaces ext with old ext to support enc/dec of encrypted values on the Library (fix)

#### Version history 
- View git history of this file if interested

## Installation
- [Installation guide](https://systemlibrary.github.io/systemlibrary-common-framework/Install.html)

## Documentation
- [Documentation with code samples](https://systemlibrary.github.io/systemlibrary-common-framework/)

## Nuget
- [![Latest version](https://img.shields.io/nuget/v/SystemLibrary.Common.Framework)](https://www.nuget.org/packages/SystemLibrary.Common.Framework)

## Source
- [Github](https://github.com/systemlibrary/systemlibrary-common-framework)

## Suggestions and feedback
- [Send us an email](mailto:support@systemlibrary.com)

## License
- Free with Tiered Pricing for additional features
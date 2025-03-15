# SystemLibrary Common Framework

## Description
Framework with default settings and classes for every &gt;= .NET 9 application

### Features
- Extensions for strings, arrays, lists, ...
- Dump.Write() "equivalent" to console.log in javascript
- Config&lt;&gt; class which automaps a json file and its data to the C# class, also runs transformations based on EnvironmentName
- Enum extensions and attributes such as [EnumText] and [EnumValue]
- Json() extension method available on all objects
- JsonPartial() extension method available on all objects, converting only a part of the json string to a C# class
- Encrypt and Decrypt out of the box through string and byte[] extensions, uses AES CBC PKCS7
- Obfuscate and Deobfuscate out of the box through string extensions
- Fire and forget in Async.Run()
- ToBase64() and FromBase64() through string and byte[] extensions
- ToHash(), ToSha1(), ToSha256() through Stream, string and byte[] extensions
- Service Locator in Services.Get&lt;&gt;()
- Cache items through Cache.Get&lt;&gt;()
- Log to your own ILogWriter through the global Log class
- Client is built on top of HttpClient, with Polly's retry and circuit breakers with default settings 

## Requirements
- &gt;= .NET 9

## Latest Release Notes
- 9.0.0.1
- Initial release

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
- Free
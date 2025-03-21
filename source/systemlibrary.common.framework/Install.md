# Installation
- Open Project/Solution in Visual Studio
- Open Nuget Project Manager
- Search SystemLibrary.Common.Framework
- Install SystemLibrary.Common.Framework

[![Latest version](https://img.shields.io/nuget/v/SystemLibrary.Common.Framework)](https://www.nuget.org/packages/SystemLibrary.Common.Framework)

## Requirements
- &gt;= .NET 9

## First time usage
1. Create any new empty .NET 9 project, for instance a new Asp.Net Core Empty project
2. Add SystemLibrary.Common.Framework nuget package
3. Add appSettings.json at root of the project
4. Add Startup.cs at root of the web project


```csharp 
using SystemLibrary.Common.Framework.Extensions;

public class Startup 
{
	static void Main(string[] args) {
		var host = Host.CreateDefaultBuilder(args);

		host.ConfigureWebHostDefaults(config => {
			config.UseStartup<Startup>();
		})

		Log.Write("App starting...");

		//other options...

		host.Buidler().Run();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		var options = new FrameworkOptions();
		app.UseFrameworkApp(options);
	}
	
	public void ConfigureServices(IServiceCollection services)
	{
		var options = new FrameworkOptions();
		services.AddFrameworkServices(options);
	}
}
```

## Package Configurations
* All default and configurable settings for this package.

###### appSettings.json:
```json  
	{
		"systemLibraryCommonFramework": {
			"license": "",	// Send email to support@systemlibrary.com
			"debug": false, // Enable framework debugging information
			"frameworkEncKey": null, // Set the global enc key in your pipeline if you cant use file, cli or env path in the pipeline
			"log": {
			  "level": "Warning",
			  "fullFilePath": "%HomeDrive%/Logs/Dump.log",
			  "format": "Text",
			  "addUrl": true,
			  "addHttpMethod": false,
			  "addAuthenticatedState": false,
			  "addStacktrace": false,
			  "addIP": false,
			  "addBrowserName": false,
			  "addCorrelationId": true
			},

			"json": {
			  "allowTrailingCommas": true,
			  "maxDepth": 32,
			  "propertyNameCaseInsensitive": true,
			  "jsonIgnoreCondition": "WhenWritingNull",
			  "WriteIndented": false,
			  "ignoreReadOnlyFields": true,
			  "includeFields": true,
			  "jsonSecureAttributesEnabled": true
			},

			"cache": {
			  "duration": 180,
			  "containerSizeLimit": 60000,
			  "fallbackDuration": 300 // [Gold Tier]
			},

			"client": {
			  "timeout": 40001,
			  "retryTimeout": 10000,
			  "clientCacheDuration": 1200,
			  "ignoreSslErrors": true,
			  "throwOnUnsuccessful": true,
			  "useRetryPolicy": true,
			  "useRequestBreakerPolicy": false // [Gold Tier]
			},

			"metrics": {
			  "enable": true, // [Gold Tier]
			  "authorizationValue": "Demo"
			}
		}
	}
```

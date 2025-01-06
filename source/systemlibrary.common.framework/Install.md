# Installation
- Open Project/Solution in Visual Studio
- Open Nuget Project Manager
- Search SystemLibrary.Common.Framework
- Install SystemLibrary.Common.Framework

[![Latest version](https://img.shields.io/nuget/v/SystemLibrary.Common.Framework)](https://www.nuget.org/packages/SystemLibrary.Common.Framework)

## Requirements
- &gt;= .NET 8

## First time usage
1. Create any new empty .NET 8 project, for instance a new Asp.Net Core Empty project
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

		Dump.Write("App starting..."); // Dump is living in global namespace, available anywhere

		//other options...

		host.Buidler().Run();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		var options = new FrameworkAppOptions();
		app.UseFrameworkApp(options);
	}
	
	public void ConfigureServices(IServiceCollection services)
	{
		var options = new FrameworkServicesOptions();
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
			"debug": false, // Internal debug info in this package is logged if true

			"dump": {
				"folder": "%HomeDrive%/Logs/",
				"fileName": "DumpWrite.log",
			},

			"json": {
				"allowTrailingCommas": true,
				"maxDepth": 32,
				"propertyNameCaseInsensitive": true,
				"readCommentHandling": "Skip",
				"jsonIgnoreCondition": "WhenWritingNull"
				"writeIndented": false
			}, 

			"cache": {
				"duration": 180,
				"fallbackDuration": 600,	// Set to 0 or negative to disable fallback mechanism
				"containerSizeLimit": 60000 // There's 8 equal sized containers, so max total items is 8X
			},
		
			"client": {
				"timeout": 40001,
				"retryTimeout": 10000,
				"ignoreSslErrors": true,
				"useRetryPolicy": true,
				"throwOnUnsuccessful": true,
				"useRequestBreakerPolicy": false,
				"clientCacheDuration": 1200
			},
		
			"log": {
				"level": "Information", // Trace, Information, Debug, Warning, Error, None
				"appendPath": true,
				"appendLoggedInState": true,
				"appendCorrelationId": true,
				"appendIp": false,
				"appendBrowser": false,
				"format": "text" // "json" or "text", default is "text"
			},

			"metrics": {
				"enablePrometheus": true,
				"authorizationValue": "Demo"
			}
		}
	}
```

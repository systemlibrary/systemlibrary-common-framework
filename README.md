# SystemLibrary Common Framework 

## Description
Framework for every .NET application.

## Requirements
&gt;= .NET 10

## Access & Contribute  
[**GitHub Source (private repository)**](https://github.com/systemlibrary/systemlibrary-common-framework-private)

To request access, email `support@systemlibrary.com` with your GitHub username.

Read-only access is granted on request — no questions asked.  

Once approved, you can clone and submit pull requests.

## 🚀 Features

### 🔧 Initialization
Use `AddFrameworkServices()` and `AddFrameworkMiddlewares()` to enable HTTPS redirection, caching, logging, auth, cookie policies, endpoints, and more.

```csharp
var options = new FrameworkOptions();
services.AddFrameworkServices(options);
app.AddFrameworkMiddlewares(options);
```

### ⚙️ Config Mapping
Auto-binds JSON config files to C# classes by name, with transformations based on environment name.

```csharp
~/myconfig.json: { url: "www.systemlibrary.com" }

class MyConfig : Config<MyConfig> { public string Url { get; set; } }
var url = MyConfig.Instance.Url;
```

### 📝 Logging
Global `Log` instance with `.Dump()` – similar to `console.log` in JavaScript.

```csharp
Log.Dump(typeof(string));
Log.Error("hello");
```

### 🧠 Cache
Sharded global cache with fallback, metrics, and auto key generation.

```csharp
var value = Cache.Get<string>("key", () => Compute());
```

### 🌐 HTTP Client
Client which caches underlying `HttpClient` with automatic retry policies, metrics and a circuit breaker [Pro Tier].

```csharp
var response1 = new Client().Get<string>("https://api.example.com/data");
var response2 = "https://api.example.com/data".GetRequest<string>();
```

### Metric UI
Out of the box a metric UI, rendering a pie chart per metric

```csharp
/metrics/ui 
```

### 📦 Extensions
`.Json()`, `.PartialJson()`, `.Encrypt()`, `.Decrypt()`, `.ToBase64()`, `.FromBase64()`, `.Compress()`, `.Decompress()`, `.Obfuscate()`, `.Deobfuscate()`, `.Is()`, `.IsNot()`, `.GetSampledKey()` and more...

```csharp
var json = user.Json();
var sampledKey = json.GetSampledKey();
var encrypted = json.Encrypt();
var obfuscated = json.Obfuscate();
var hash = json.ToSha256Hash();
var base64 = json.ToBase64();
```

### 🔐 Encryption
Encrypt/decrypt using either AES CBC, AES GCM or RSA via string/byte extensions with global key management.

```csharp
var encrypted = "secret".Encrypt(); // AES CBC PKCS7
var encrypted = "secret".EncryptAesGcm();
var encrypted = "secret".EncryptRsa();
```

### 🧩 Enhanced Enums
Decorate enums with `[EnumText]` and `[EnumValue]`, and a JsonConverter is registered and injected into MVC, serialization/deserialization uses the attributes too. Use `ToValue()` or `ToText()` on the Enum yourself.

```csharp
enum Role { 
	[EnumValue("adm")]
	[EnumText("Administrator")]
	Admin,

	Guest
}
...
var role = Role.Admin.ToText(); // `Administrator`
var value = Role.Admin.ToValue(); // `adm`

var role = Role.Guest.ToText(); // `Guest`
var value = Role.Guest.ToValue(); // `Guest`
```

### 📡 BaseApiController
All API classes can inherit this, endpoints naturally registered and easily add attributes such as: `[OriginFilter]`, `[ApiTokenFilter]` and `[UserAgentFilter]`.

Automatic 📖 `/docs` endpoint listing all routes, inputs, and metadata.

```csharp
[ApiTokenFilter(name: "hello", value: "world")]
public class MyApi : BaseApiController 
{ 
}
```

### 🔗 ModelBinder
Model bindings auto-registered for DateTime and enum types — built to correctly parse most inputs into the values you actually want.
```csharp
public class Controller 
{
	ActionResult Index(Role role, DateTime date) 
	{
	}
}
```

## Latest Release Notes
- 8.5.0.2
- Docs updated (fix)
- Async.Run removed, not in use (clean)
- Vision.md created (new)

#### Version history 
View git history of this file if interested

## Installation
[Installation guide](https://systemlibrary.github.io/systemlibrary-common-framework/Install.html)

## Documentation
[Documentation with code samples](https://systemlibrary.github.io/systemlibrary-common-framework/)

## Nuget
[![Latest version](https://img.shields.io/nuget/v/SystemLibrary.Common.Framework)](https://www.nuget.org/packages/SystemLibrary.Common.Framework)

## Vision
Futuristic vision to be a full framework:
Vision Vision](https://github.com/systemlibrary/systemlibrary-common-framework/blob/main/Vision.md)

## License
Free with Tiered Pricing for additional features at https://www.systemlibrary.com/

### Dependencies
- [Chart.js](https://github.com/chartjs/Chart.js), licensed under the MIT License.
- [Prometheus-net](https://www.nuget.org/packages/prometheus-net), licensed under the MIT License.
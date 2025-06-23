# SystemLibrary Common Framework 

## Description
Framework for every .NET application.

## Requirements
&gt;= .NET 8

## Access & Contribute  
[**GitHub Source**](https://github.com/systemlibrary/systemlibrary-common-framework-private)

To request access, email `support@systemlibrary.com` with your GitHub username and specify the repo.

Read-only access is granted on request — no questions asked.  
Once approved, you can fork, clone, and submit pull requests.

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
Client which caches underlying `HttpClient` with retry policies, circuit breaker, and metrics.

```csharp
var json = Client.Get<string>("https://api.example.com/data");
var json2 = "https://api.example.com/data".Get<string>();
```

### 📦 Extensions
`.Json()`, `.PartialJson()`, `.Encrypt()`, `.Decrypt()`, `.ToBase64()`, `.FromBase64()`, `.Compress()`, `.Decompress()`, `.Obfuscate()`, `.Deobfuscate()`, `.Is()`, `.IsNot()`, `.GetCompressedKey` and more...

```csharp
var json = user.Json();
var encrypted = json.Encrypt();
var obfuscated = encrypted.Obfuscate();
var compressedKey = obfuscated.GetCompressedKey();

"hi".ToSha256();
"hi".Compress();
"hi".ToBase64();
"hi".Obfuscate();
```

### 🔐 Encryption
Encrypt/decrypt using AES CBC PKCS7 via string/byte extensions with global key management.

```csharp
var encrypted = "secret".Encrypt();
var decrypted = encrypted.Decrypt();
```

### 🧩 Enhanced Enums
Decorate enums with `[EnumText]` and `[EnumValue]` for better JSON control and use `ToValue()` or `ToText()` on the Enum.

```csharp
enum Role { [EnumText("Administrator")] Admin }
...
var role = user.Role.ToText(); // `Administrator`
```

### 📡 BaseApiController
All API classes can inherit this, endpoints naturally registered and easily add attributes such as: `[OriginFilter]`, `[ApiTokenFilter]` and `[UserAgentFilter]`.

Automatic 📖 `/docs` endpoint listing all routes, inputs, and metadata.

```csharp
[ApiTokenFilter(name: "hello", value: "world")]
public class MyApi : BaseApiController { }
```

### 🔗 ModelBinder
Model bindings auto-registered for DateTime and enum types — built to correctly parse most inputs into the values you actually want.
```csharp
ActionResult Index(Role role, DateTime date) {
}
```

## Latest Release Notes
- 8.4.0.3
- Metrics UI: title font size lowered and added a hover effect for long titles (fix)
- Metrics UI: pie charts no longer duplicates stats, a lock mechanism is implemented (fix)

#### Version history 
View git history of this file if interested

## Installation
[Installation guide](https://systemlibrary.github.io/systemlibrary-common-framework/Install.html)

## Documentation
[Documentation with code samples](https://systemlibrary.github.io/systemlibrary-common-framework/)

## Nuget
[![Latest version](https://img.shields.io/nuget/v/SystemLibrary.Common.Framework)](https://www.nuget.org/packages/SystemLibrary.Common.Framework)

## License
Free with Tiered Pricing for additional features at https://www.systemlibrary.com/

### Dependencies
- [Chart.js](https://github.com/chartjs/Chart.js), licensed under the MIT License.
- [Prometheus-net](https://www.nuget.org/packages/prometheus-net), licensed under the MIT License.
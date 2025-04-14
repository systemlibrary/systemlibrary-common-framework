# FrameworkOptions

FrameworkOptions configures the framework’s behavior, including encryption, security, static files, and MVC settings. Below are the key options that require explanation:

## Usage
```csharp
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
```

## FrameworkEncKeyDir - Global encryption key
The global encryption key is used for encrypting and decrypting data via .Encrypt and .Decrypt.

Note: This does not affect Microsoft’s DataProtection, which handles tasks like cookie encryption.

#### Why Use It?
- **Encrypt/Decrypt** - Use your own key for data encryption and decryption through Encrypt() and Decrypt() extensions.
- **Secure configurations** - Encrypt passwords and tokens before pushing to GitHub.
- **Decrypt configurations** - Add [ConfigDecrypt] to automatically decrypt specific configuration properties with the global key.
- **No dependency on DataProtection** - Configure data protection as you please without affecting data encrypted using the global encryption key.

#### How To
1. **Command-Line Argument (CLI)**: Example --frameworkEncKey=1234
2. **AppSettings Configuration**: { "systemLibraryCommonFramework": { "frameworkEncKey": "1234" } }
3. **Environment Variable**: set frameworkEncKey=1234
4. **Key File from Directory**: options.EncKeyDir = "C:\Keys\"
5. **Fallback Default**: If none are set, it defaults to `"ABCDEF..."`.

## UseDataProtectionPolicy – Persistent Key Management  
`UseDataProtectionPolicy` provides a simple, **opt-in** way to persist Data Protection keys across environments, ensuring consistent encryption behavior without unexpected resets.  

#### Why Use It?  
- **Avoids In-Memory Keys** – Prevents key loss on redeployments.  
- **Ensures Persistent Encryption** – Maintains data protection across app restarts.  
- **Simplifies Setup** – Generates a single key file once, reusable across all environments, which expires in 100 years.
- **Reduces Login Interruptions** – Prevents frequent logouts caused by key resets.  

#### How To
1. **Opt-In** – Set `UseDataProtectionPolicy = true` in `FrameworkOptions`.  
2. **Initial Setup** – On first run, generates a key file in `App_Data/DataProtectionKeys`.  
3. **Git Ignore** – Add the key directory to `.gitignore`.  
4. **Add Key to CI/CD** – Copy key file to your CI/CD pipeline and ensure it's placed in `App_Data/DataProtectionKeys` on build.

#### Key File Behavior
- **One-Time Generation** – If a key file exists, no new one is created.  
- **Long-Term Persistence** – Default key lifetime is **100 years**.  
- **Manual Management** – Consumers can manually replace or rotate keys if necessary.

## UseForwardedHeaders – Proxy & Load Balancer Support

Enables handling of X-Forwarded-* headers, ensuring correct client IP detection when behind a reverse proxy.

## UseStaticFilePolicy

Always blocks serving file extensions such as `.exe`, `.dll`, `.cs`, `.ps1`, `.sh`, as well as folders like `/bin/`, `/properties/`, and `/app_data/`, regardless of whether this flag is enabled or disabled.

By default, the policy is enabled (`true`), which:
- Allows serving static files (e.g., `.js`, `.css`) from any folder within your application.
- Adds a 20 days client-side cache for static content.
- Compresses files before transmission.

#### StaticFilesRequestPaths
By default, this is `null`, meaning static files can be served from any folder, subject to the aforementioned restrictions.

Set this to an array of specific paths to restrict static file serving to those locations, allowing access to any files within those paths (e.g., set to `/public/` to serve all files beneath `/public/`).

#### StaticFilesMaxAgeSeconds
Defines the cache duration (in seconds) for static files. Set to `0` to disable caching.
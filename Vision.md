# Vision

This framework is already live on NuGet:  
👉 [SystemLibrary.Common.Framework](https://www.nuget.org/packages/SystemLibrary.Common.Framework)

Below is where it’s headed. No fluff, just features.

---

## Upcoming

### ✅ Host Builders
- A simple `HostBuilder` per application type:
  - Api, WinForm, Console, WebApp
  - Hosting: Linux/Windows, ARM/x64
- A sample `.zip` for each type, fully tested in its specific environment.

### ✅ JS Action Results as Server Side Rendering
- An efficient `JavascriptActionResult` via custom `System.Text.Json` converters.
- Controllers can return models directly to JS world (Vue, React, plain JS):
  - Models become arguments/props.
  - Easy registration of custom `JsonConverters`.
- No reason to create butt-ugly .cshtml files anymore (except the layout and view.cshtml probably, that invoked RenderBody, thats it)
    - The C# dev sits in Visual Studio and does C#, api's and security...
    - The TS/JS/react/vue/css/html dev sits in VS Code or whatever...
- Will invoke your javascript server side, "server side rendering" in a JS engine, V8 or similar...
    - Will attach all properties send to the SSR, directly printing it to the DOM as already JS variable, no need to parse DOM, to look for hydration elements, the Browser already parses the DOM and its JS...
    - A NPM package will be created to import and invoke to hydrate any component for you, one-liner

### ✅ Middleware
- Simple way to register your own middlewares:
  - Before/after default ones like auth/cookie/etc.
  - Before/after endpoints as needed.

### Docs
- Built-in documentation CLI (DocFx):
  - Easy to generate and publish docs.

---

## [Gold Tier]

### ORM
- Full ORM on EntityFramework:
  - Inherit a base class, add virtual properties → table/columns auto-generated.
- Routing that maps a request directly to the right ORM class:
  - Fetch & build properties, send as props to frontend.
- Will probably Interface the DB calls, supporting SQLite, PostgreSQL and MsSql...

### Cache + CDN
- Extend cache to fetch from CDN:
  - Configure tokens/URLs in `appSettings`.
- To be considered...

### API Key Management
- Rotating API keys stored in DB:
  - Token generated per invocation.
  - Configurable expiration with overlap.
  - Metrics: usage counts, timestamps.

---

## [Diamond Tier]

### CMS
- A full CMS layer (may built upon existing CMS):
 - Built on the ORM inside the framework (might be a different nuget package, dependency, will research a lot)
  - Admin panel, editor panel, jobs panel.
  - Rich text editor, page/content creation, integrations to media content to drag drop those...
  - Probably not storing nor copying blobs medias into the DB
  - PDF, Docs, images, movies, might be hosted elsewhere instead, we will see...
  - But the integration will work out of the box...

### BADAS
- **Build And Deploy As a Service**:
  - Repo access → auto-download, build, run tests (front and backend), deploy (blue/green/red).
  - Heroku/Fly.io style deployment, likely targeting Oracle Cloud.
  - Might look into existing ones and scrap it... But in 2025, no reason last 10 years that all apps, api's must deploy and control that pipeline themselves.
  - A simple registration over domain, and some access tokens to a VPC, and we "terraform" it all for you...
  - Easily spinning up a completely new vpc and account too if needed... for public websites, or doenst have to be... firewall...

---

## Other Enhancements

- Better Metrics UI:
  - Searchable inputs, dynamic charts.

- Async Simplified:
  - Experimental keyword `para`:
    ```csharp
    var res = para Method(123);
    ```
    Compiler expands to `Task.Run` + `await`.

- Client Upgrades:
  - gRPC, FTP, SFTP support.
  - Proper progress bars (still a pain in 2025).

- Compressed IDs Everywhere:
  - `GetCompressedId()` on any object:
    - Lists, dictionaries, arrays, anything.
    - Collision probability ~1 in a billion.
    - Not for high-security/legal use, but fine for most.

---

**That’s the vision.**  

If you want to follow along or contribute, keep an eye on this repo and the issues/backlog.

My vision is simple: **useful and reusable systems, modules, and one‑liners for everyone.**  
In 2025 there’s no reason anyone should still be wiring up the same middlewares and boilerplate over and over.  
I’ve given Microsoft more than ten years since the first release of .NET (from the old .NET Framework) to make this seamless…  
and yet here we are, they havent even created IMemoryCache as a Sharded Cache so if you actually do have some 10K users, you will hit a bottleneck...

So this framework exists to fix that.
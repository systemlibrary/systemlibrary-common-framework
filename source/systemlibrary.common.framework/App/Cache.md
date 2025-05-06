# Cache

Cache provides application-level caching with auto-generated cache keys that differentiate based on user roles. The default duration is **3 minutes**.

### Key Features
- **Auto-generated cache key** - Includes namespace, class, method, and scoped values (bool, string, int, datetime, enum), plus first-depth properties and fields of scoped objects if of the same types.
    - **Per user group caching** - Auto-generated cache key appends `IsAuthenticated` and relevant role claim (`role`, `Role`, `RoleClaimType`).
- **Skip Options**
  - `skipWhenAuthenticated`: Defaults to `false`.
  - `skipWhenAdmin`: Defaults to `true`. Applies to case-sensitive roles:  
    `Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.`
  - `skipWhen`: Custom condition, must return `true` to skip.

### Cache Behavior
- **Capacity**: 320,000 items across 8 containers (`40,000` per container).
- **Eviction Policy**: When full, removes **33%** of the oldest items.
- **Fallback Cache [[Gold Tier](#)]**:  optional secondary cache in case fetching items from cache fails

### Default configurations
```json
{
    "systemLibraryCommonFramework": {
        "cache": {
            "duration": 180,
            "fallbackDuration": 300,    // [Gold Tier] Set to 0 to disable
            "containerSizeLimit": 40001
        }
    }
}
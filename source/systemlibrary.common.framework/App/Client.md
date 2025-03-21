# Client

**Client** is a class for all HTTP(S) requests in your project.

Uses `HttpClient` and `Polly` for:
- Reusing TCP connections
- Retrying on **502** and **504** status codes
- **Circuit breaker [[Gold Tier](#)]** (7s) if **25** errors (**404, 429, 500, 502, 503, 504, 505**) occur in a row

### Retry Policy

`useRetryPolicy: false` (default)
- Retries once on **502, 504** (GET, POST, file requests)

`useRetryPolicy: true`
- Same as `false`, plus:
  - Retries **one extra time** on **502, 504** (GET, POST)
  - Retries **twice** if response is `null` (timeout/no response)
  - Retries **once** on:
    - **401** (GET, POST)
    - **404** (GET)
    - **500** (GET, POST)
    - **404, 500, 502, 504** (file requests)
    - **OPTION, PATCH, HEAD, CONNECT, TRACE**

### Default configurations

```json
"client": {
  "timeout": 40001,
  "retryTimeout": 10000,
  "ignoreSslErrors": true,
  "useRetryPolicy": true,
  "throwOnUnsuccessful": true,
  "useRequestBreakerPolicy": false,
  "clientCacheDuration": 1200
}
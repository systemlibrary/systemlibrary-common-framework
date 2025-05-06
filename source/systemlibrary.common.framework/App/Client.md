# Client

**Client** is a class for all HTTP(S) requests in your project.

Uses `HttpClient` for:
- Reusing TCP connections
- Retrying on **502** and **504** status codes

Uses `Polly`:
- **Circuit breaker [[Gold Tier](#)]** (7s) if **25** errors (**404, 429, 500, 502, 503, 504, 505**) occur in a row

Uses `Prometheus`:
- Metric counting in memory for all request/responses made


### Retry Policy

`useRetryPolicy: false` (default)
- Retries once on **502, 504** (GET, POST, file requests)

`useRetryPolicy: true`
- Same as `false`, plus:
  - Retries **one extra time** on **502, 504** (GET, POST)
  - Retries **twice** if response is `null` (timeout/no response)
  - Retries **once** on:
    - **401** for GET, POST
    - **404** for GET
    - **500** for GET, POST
    - **404, 500, 502, 504** for file requests
    - **404, 502, 504** for OPTION, PATCH, HEAD, CONNECT, TRACE**

### Default configurations

```json
"client": {
  "timeout": 40001,
  "retryTimeout": 10000,
  "ignoreSslErrors": true,
  "useRetryPolicy": true,
  "throwOnUnsuccessful": true,
  "useRequestBreakerPolicy": false, // [Gold Tier]
  "expectContinue": false,
  "useAutomaticDecompression": true,
  "clientCacheDuration": 1200
}
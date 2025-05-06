# BaseApiController

BaseApiController to simplify all api's within any .NET application

### Key Features
- Autogenerating endpoint for the api /docs which generates a json documentation for all api endpoints
- Uses the frameworks rules for json deserialization a string from a payload (body) to a C# model
- Uses the frameworks rules for converting Enums from strings and int's
- All types inheriting BaseApiController is automatically registered as a route in your application based on the namespace without the rootnamespace in the dll
- All invocations generates a metric +1 (the CTOR) for the api controller being used (does not seperate per method)
- 
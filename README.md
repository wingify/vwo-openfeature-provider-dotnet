# VWO OpenFeature Provider .NET

[![NuGet version](https://badge.fury.io/nu/VWO.OpenFeature.Provider.svg)](https://www.nuget.org/packages/VWO.OpenFeature.Provider/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](http://www.apache.org/licenses/LICENSE-2.0)

## Requirements

- Works with .NET 6.0 or higher

## Installation

It's recommended to use [dotnet CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/) for package installation.

```bash
dotnet add package VWO.OpenFeature.Provider
```

## Basic usage

**Using and Instantiation**

```c#
using OpenFeature;
using OpenFeature.Model;
using VWOOpenFeatureProvider;
using VWOFmeSdk;
using VWOFmeSdk.Models.User;

static async Task Main(string[] args)
{
    var vwoInitOptions = new VWOInitOptions
    {
        SdkKey = "your-sdk-key-here",     // Replace with your SDK Key
        AccountId = 123456,               // Replace with your VWO Account ID
        Logger = new Dictionary<string, object>
        {
            { "level", "ERROR" }         // Logging level
        }
    };

    // Initialize the VWO client
    var vwoClient = VWO.Init(vwoInitOptions);

    // Initialize the VWO Provider
    var vwoProvider = new VWOProvider(vwoClient);

    // Get the client from OpenFeature API
    var client = Api.Instance.GetClient();

    // Test feature flags with variable key values
    await TestFlags();
}
static async Task TestFlags(FeatureClient client, EvaluationContext context)
{

    // Setting up the EvaluationContext
    var context = EvaluationContext.Builder()
        .Set("targetingKey", new Value("user-id"))
        .Set("key", new Value("variable-key")) // Replace with your variable key
        .Build();

    // Set the provider using OpenFeature API
    await Api.Instance.SetProviderAsync(vwoProvider);

    // Test boolean flag
    var boolResult = await client.GetBooleanValueAsync("feature-boolean", false, context);
    Console.WriteLine($"BOOL result: {boolResult}");

    // Test string flag
    var stringResult = await client.GetStringValueAsync("feature-string", "defaultString", context);
    Console.WriteLine($"STRING result: {stringResult}");

    // Test integer flag
    var intResult = await client.GetIntegerValueAsync("feature-integer", 0, context);
    Console.WriteLine($"INTEGER result: {intResult}");

    // Test float flag
    var floatResult = await client.GetDoubleValueAsync("feature-float", 0.0, context);
    Console.WriteLine($"FLOAT result: {floatResult}");

    // Test object flag
    var objectResult = await client.GetObjectValueAsync("feature-object", new Value(new Structure(new Dictionary<string, Value>())), context);
    Console.WriteLine($"OBJECT result: {objectResult}");
}
```

## Authors

- [Saksham Gupta](https://github.com/sakshamg1304)

## Changelog

Refer [CHANGELOG.md](https://github.com/wingify/vwo-openfeature-provider-dotnet/blob/master/CHANGELOG.md)

## Contributing

Please go through our [contributing guidelines](https://github.com/wingify/vwo-openfeature-provider-dotnet/CONTRIBUTING.md)

## Code of Conduct

[Code of Conduct](https://github.com/wingify/vwo-openfeature-provider-dotnet/blob/master/CODE_OF_CONDUCT.md)

## License

[Apache License, Version 2.0](https://github.com/wingify/vwo-openfeature-provider-dotnet/blob/master/LICENSE)

Copyright 2024-2025 Wingify Software Pvt. Ltd.

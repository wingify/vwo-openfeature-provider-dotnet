# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

[1.0.0] - 2025-02-25

### Added

- First release of VWO OpenFeature Provider .NET

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

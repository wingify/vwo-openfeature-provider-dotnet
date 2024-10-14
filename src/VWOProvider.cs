#pragma warning disable 1587
/**
 * Copyright 2024-2025 Wingify Software Pvt. Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#pragma warning restore 1587

using OpenFeature;
using OpenFeature.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VWOFmeSdk;
using VWOFmeSdk.Models.User;
using OpenFeature.Constant;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VWOOpenFeatureProvider
{
    public class VWOProvider : FeatureProvider
    {
        private VWOClient _client;

        /// <summary>
        /// Constructor for initializing the VWOProvider with the VWOClient instance.
        /// </summary>
        /// <param name="vwoClient">The initialized VWO client to be used by the provider.</param>
        public VWOProvider(VWOClient vwoClient)
        {
            _client = vwoClient;
        }

        /// <summary>
        /// Returns metadata about this provider.
        /// </summary>
        /// <returns>Metadata object with the provider name.</returns>
        public override Metadata GetMetadata()
        {
            return new Metadata("vwo-openfeature-provider-dotnet");
        }

        /// <summary>
        /// Resolves the boolean value of the feature flag based on the given flag key and evaluation context.
        /// Compares the variable key from the context with the variable key in each variable returned from getFlag.
        /// </summary>
        /// <param name="flagKey">The key of the feature flag to be evaluated.</param>
        /// <param name="defaultValue">The default boolean value to return if the flag cannot be resolved.</param>
        /// <param name="context">Optional evaluation context which may contain additional parameters for evaluation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that resolves to the resolved boolean value of the flag.</returns>
        public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(string flagKey, bool defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var vwoContext = ConvertToVWOContext(context);
                var getFlag = _client.GetFlag(flagKey, vwoContext);

                // Extract the key from the context if available
                var variableKey = context?.TryGetValue("key", out var keyValue) == true ? keyValue.AsString : null;

                if( variableKey == null) {
                    return Task.FromResult(new ResolutionDetails<bool>(flagKey, getFlag.IsEnabled()));
                }

                var variables = getFlag.GetVariables();
                foreach (var variable in variables)
                {
                    if (variable.TryGetValue("key", out object? key) && key?.ToString() == variableKey)
                    {
                        if (variable.TryGetValue("type", out object? type) && type?.ToString() == "boolean")
                        {
                            bool result = (bool)(variable.GetValueOrDefault("value", defaultValue));
                            return Task.FromResult(new ResolutionDetails<bool>(flagKey, result));
                        }
                        else
                        {
                            return Task.FromResult(new ResolutionDetails<bool>(flagKey, defaultValue));
                        }
                    }
                }

                return Task.FromResult(new ResolutionDetails<bool>(flagKey, defaultValue));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResolutionDetails<bool>(flagKey, defaultValue, ErrorType.None, ex.Message));
            }
        }


        /// <summary>
        /// Resolves the string value of the feature flag based on the given flag key and evaluation context.
        /// Compares the variable key from the context with the variable key in each variable returned from getFlag.
        /// </summary>
        /// <param name="flagKey">The key of the feature flag to be evaluated.</param>
        /// <param name="defaultValue">The default string value to return if the flag cannot be resolved.</param>
        /// <param name="context">Optional evaluation context which may contain additional parameters for evaluation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that resolves to the resolved string value of the flag.</returns>
        public override Task<ResolutionDetails<string>> ResolveStringValueAsync(string flagKey, string defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var vwoContext = ConvertToVWOContext(context);
                var getFlag = _client.GetFlag(flagKey, vwoContext);

                // Extract the key from the context if available
                var variableKey = context?.TryGetValue("key", out var keyValue) == true ? keyValue.AsString : null;

                var variables = getFlag.GetVariables();
                foreach (var variable in variables)
                {
                    if (variableKey != null && variable.TryGetValue("key", out object? key) && key?.ToString() == variableKey)
                    {
                        if (variable.TryGetValue("type", out object? type) && type?.ToString() == "string")
                        {
                            string result = (string)(variable.GetValueOrDefault("value", defaultValue));
                            return Task.FromResult(new ResolutionDetails<string>(flagKey, result));
                        }
                    }
                }

                return Task.FromResult(new ResolutionDetails<string>(flagKey, getFlag.GetVariable("key", defaultValue)?.ToString() ?? defaultValue));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResolutionDetails<string>(flagKey, defaultValue, ErrorType.None, ex.Message));
            }
        }

        /// <summary>
        /// Resolves the integer value of the feature flag based on the given flag key and evaluation context.
        /// Compares the variable key from the context with the variable key in each variable returned from getFlag.
        /// </summary>
        /// <param name="flagKey">The key of the feature flag to be evaluated.</param>
        /// <param name="defaultValue">The default integer value to return if the flag cannot be resolved.</param>
        /// <param name="context">Optional evaluation context which may contain additional parameters for evaluation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that resolves to the resolved integer value of the flag.</returns>
        public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(string flagKey, int defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var vwoContext = ConvertToVWOContext(context);
                var getFlag = _client.GetFlag(flagKey, vwoContext);

                // Extract the key from the context if available
                var variableKey = context?.TryGetValue("key", out var keyValue) == true ? keyValue.AsString : null;

                var variables = getFlag.GetVariables();
                foreach (var variable in variables)
                {
                    if (variableKey != null && variable.TryGetValue("key", out object? key) && key?.ToString() == variableKey)
                    {
                        if (variable.TryGetValue("type", out object? type) && type?.ToString() == "integer")
                        {
                            int result = Convert.ToInt32(variable.GetValueOrDefault("value", defaultValue));
                            return Task.FromResult(new ResolutionDetails<int>(flagKey, result));
                        }
                    }
                }

                return Task.FromResult(new ResolutionDetails<int>(flagKey, Convert.ToInt32(getFlag.GetVariable("key", defaultValue))));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResolutionDetails<int>(flagKey, defaultValue, ErrorType.None, ex.Message));
            }
        }

        /// <summary>
        /// Resolves the double value of the feature flag based on the given flag key and evaluation context.
        /// Compares the variable key from the context with the variable key in each variable returned from getFlag.
        /// </summary>
        /// <param name="flagKey">The key of the feature flag to be evaluated.</param>
        /// <param name="defaultValue">The default double value to return if the flag cannot be resolved.</param>
        /// <param name="context">Optional evaluation context which may contain additional parameters for evaluation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that resolves to the resolved double value of the flag.</returns>
        public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(string flagKey, double defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var vwoContext = ConvertToVWOContext(context);
                var getFlag = _client.GetFlag(flagKey, vwoContext);

                // Extract the key from the context if available
                var variableKey = context?.TryGetValue("key", out var keyValue) == true ? keyValue.AsString : null;

                var variables = getFlag.GetVariables();
                foreach (var variable in variables)
                {
                    if (variableKey != null && variable.TryGetValue("key", out object? key) && key?.ToString() == variableKey)
                    {
                        if (variable.TryGetValue("type", out object? type) && type?.ToString() == "double")
                        {
                            double result = Convert.ToDouble(variable.GetValueOrDefault("value", defaultValue));
                            return Task.FromResult(new ResolutionDetails<double>(flagKey, result));
                        }
                    }
                }

                return Task.FromResult(new ResolutionDetails<double>(flagKey, Convert.ToDouble(getFlag.GetVariable("key", defaultValue))));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResolutionDetails<double>(flagKey, defaultValue, ErrorType.None, ex.Message));
            }
        }

        /// <summary>
        /// Resolves the structure value of the feature flag based on the given flag key and evaluation context.
        /// Compares the variable key from the context with the variable key in each variable returned from getFlag.
        /// </summary>
        /// <param name="flagKey">The key of the feature flag to be evaluated.</param>
        /// <param name="defaultValue">The default structure value to return if the flag cannot be resolved.</param>
        /// <param name="context">Optional evaluation context which may contain additional parameters for evaluation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that resolves to the resolved structure value of the flag.</returns>
        public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(string flagKey, Value defaultValue, EvaluationContext? context = null, CancellationToken cancellationToken = default)
{
    try
    {
        var vwoContext = ConvertToVWOContext(context);
        var getFlag = _client.GetFlag(flagKey, vwoContext);

        // Extract the key from the context if available
        var variableKey = context?.TryGetValue("key", out var keyValue) == true ? keyValue.AsString : null;

        var variables = getFlag.GetVariables();

        // Case 1: Return a single variable if a key is provided
        if (!string.IsNullOrEmpty(variableKey))
        {
            bool keyMatched = false; // Track if any key matches the variableKey

            foreach (var variable in variables)
            {
                if (variable.TryGetValue("key", out object? key) && key?.ToString() == variableKey)
                {
                    if (variable.TryGetValue("type", out object? type) && type?.ToString() == "json")
                    {
                        keyMatched = true;
                        if (variable.TryGetValue("value", out object? jsonValue) && jsonValue is JObject jsonObject)
                        {
                            // Output the simplified JSON object
                            var jsonString = JsonConvert.SerializeObject(jsonObject);
                            return Task.FromResult(new ResolutionDetails<Value>(flagKey, new Value(jsonString)));
                        }
                    }
                }
            }

            // Return the default value if no matching key is found
            if (!keyMatched)
            {
                return Task.FromResult(new ResolutionDetails<Value>(flagKey, defaultValue));
            }
        }

        // Case 2: Return all variables in a simplified format
        var simplifiedVariables = new Dictionary<string, object>();
        foreach (var variable in variables)
        {
            if (variable.TryGetValue("key", out object? key) && variable.TryGetValue("value", out object? value))
            {
                simplifiedVariables[key?.ToString() ?? ""] = value;
            }
        }

        // Convert to a simple JSON representation
        var simpleJson = JsonConvert.SerializeObject(simplifiedVariables);
        return Task.FromResult(new ResolutionDetails<Value>(flagKey, new Value(simpleJson)));
    }
    catch (Exception ex)
    {
        Console.WriteLine("In catch: " + ex.Message);
        return Task.FromResult(new ResolutionDetails<Value>(flagKey, defaultValue, ErrorType.None, ex.Message));
    }
}

        /// <summary>
        /// Converts the OpenFeature EvaluationContext to VWOContext.
        /// Extracts the relevant fields from the EvaluationContext and maps them to the VWOContext.
        /// </summary>
        /// <param name="context">The EvaluationContext from OpenFeature containing evaluation attributes.</param>
        /// <returns>Returns the VWOContext object containing the mapped fields from the EvaluationContext.</returns>
        private VWOContext ConvertToVWOContext(EvaluationContext? context)
        {
            var vwoContext = new VWOContext();

            if (context == null || context == EvaluationContext.Empty)
            {
                return vwoContext;
            }

            // Extract the TargetingKey (equivalent to "Id" in VWO)
            vwoContext.Id = context.TargetingKey;

            // Extract UserAgent if present
            if (context.TryGetValue("userAgent", out var userAgentValue))
            {
                vwoContext.UserAgent = userAgentValue?.AsString ?? "";
            }

            // Extract IpAddress if present
            if (context.TryGetValue("ipAddress", out var ipAddressValue))
            {
                vwoContext.IpAddress = ipAddressValue?.AsString ?? "";
            }

            // Extract CustomVariables if present
            if (context.TryGetValue("customVariables", out var customVariablesValue) && customVariablesValue?.AsStructure != null)
            {
                // Unpack the custom variables from Structure to Dictionary<string, object>
                var customVariablesDict = new Dictionary<string, object>();
                foreach (var kvp in customVariablesValue.AsStructure)
                {
                    customVariablesDict[kvp.Key] = kvp.Value.AsObject ?? "";
                }
                vwoContext.CustomVariables = customVariablesDict;
            }

            // Extract VariationTargetingVariables if present
            if (context.TryGetValue("variationTargetingVariables", out var variationTargetingVariablesValue) && variationTargetingVariablesValue?.AsStructure != null)
            {
                // Unpack the variation targeting variables from Structure to Dictionary<string, object>
                var variationTargetingVariablesDict = new Dictionary<string, object>();
                foreach (var kvp in variationTargetingVariablesValue.AsStructure)
                {
                    variationTargetingVariablesDict[kvp.Key] = kvp.Value.AsObject ?? "";
                }
                vwoContext.VariationTargetingVariables = variationTargetingVariablesDict;
            }

            return vwoContext;
        }
    }
}

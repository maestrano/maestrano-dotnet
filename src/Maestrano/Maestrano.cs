using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Maestrano.Configuration;
using Maestrano.Net;
using RestSharp;

namespace Maestrano
{

    public class ConfigureException : Exception
    {
        public ConfigureException(string message) : base(message)
        {
        }
    }

    public class AutoConfigureException : Exception
    {
        public AutoConfigureException(string message) : base(message)
        {
        }

        public AutoConfigureException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public static class MnoHelper
    {
        // VERSION
        public static string Version { get { return "2.0.1"; } }

        private static Dictionary<string, Preset> presetDict;

        static MnoHelper()
        {
            // Initialize preset list
            presetDict = new Dictionary<string, Preset>();
        }

        public static Dictionary<string, Preset> Presets()
        {
            return new Dictionary<string, Preset>(presetDict);
        }


        /// <summary>
        /// Autoconfigure Maestrano using the Developer platform.
        /// Parameters are taken from Environment variable names, or from the Configuration if not found
        /// Environment name: 
        /// MNO_DEVPL_HOST=<developer platform host>
        /// MNO_DEVPL_API_PATH=<developer platform host>
        /// MNO_DEVPL_ENV_NAME=<your environment nid>
        /// MNO_DEVPL_ENV_KEY=<your environment key>
        /// MNO_DEVPL_ENV_SECRET=<your environment secret>
        /// </summary>
        ///  <exception cref="AutoConfigureException">If the developer platform could not be reached.</exception>
        public static void AutoConfigure()
        {
            var config = DevPlatform.Load();
            var host = System.Environment.GetEnvironmentVariable("MNO_DEVPL_HOST") ?? (config == null ? null: config.Host);
            var path = System.Environment.GetEnvironmentVariable("MNO_DEVPL_API_PATH") ?? (config == null ? null : config.ApiPath);

            var key = System.Environment.GetEnvironmentVariable("MNO_DEVPL_ENV_KEY") ?? (config == null ? null : config.Environment.ApiKey);
            var secret = System.Environment.GetEnvironmentVariable("MNO_DEVPL_ENV_SECRET") ?? (config == null ? null : config.Environment.ApiSecret);
            AutoConfigure(host, path, key, secret);
        }

        /// <summary>
        /// Autoconfigure Maestrano using the Developer platform
        /// </summary>
        /// <param name="host"> developer platform host.</param>
        /// <param name="path"> developer platform api path</param>
        /// <param name="key"> environment key.</param>
        /// <param name="secret"> environment secret.</param>
        public static void AutoConfigure(string host, string path, string key, string secret)
        {
            if (String.IsNullOrEmpty(host)){
                throw new AutoConfigureException("Developper Platform Host was not provided.");
            }
            if (String.IsNullOrEmpty(path))
            {
                throw new AutoConfigureException("Developper Api Path Host was not provided.");
            }
            if (String.IsNullOrEmpty(key))
            {
                throw new AutoConfigureException("Developper Environment Key was not provided.");
            }
            if (String.IsNullOrEmpty(secret))
            {
                throw new AutoConfigureException("Developper Environment Secret was not provided.");
            }

            var client = new JsonClient(host, path, key, secret);
            RestResponse response = client.Get("marketplaces");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new AutoConfigureException("Could not authenticate. Check your key and secret");
            }
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                throw new AutoConfigureException(message, response.ErrorException);
            }
            if (String.IsNullOrEmpty(response.Content))
            {
                throw new AutoConfigureException("Could not retrieve any result from the developer platform.");
            }
            var content = JObject.Parse(response.Content);
            if (content["marketplaces"] == null)
            {
                throw new AutoConfigureException("Could not find marketplaces in the json response: " + response.Content);
            }
            var marketplaces = content["marketplaces"].Value<JArray>();
            foreach (var marketplace in marketplaces)
            {
                var preset = new Preset(marketplace.Value<JObject>());
                presetDict[preset.Marketplace] =  preset;
            }
        }

        /// <summary>
        /// Scope the MnoHelper to a specific configuration marketplace
        /// </summary>
        public static Preset With(string marketplace)
        {

            if (!presetDict.ContainsKey(marketplace))
            {
                throw new ConfigureException("Maestrano was not configured for marketplace: " + marketplace);
            }

            return presetDict[marketplace];
        }

        /// <summary>
        /// Clear a preset configuration
        /// </summary>
        /// <param name="presetName">name of preset to clear</param>
        public static void ClearPreset(string presetName)
        {
            if (presetDict.ContainsKey(presetName))
            {
                presetDict.Remove(presetName);
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using RestSharp;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Maestrano.Helpers;
using Maestrano.Net;

namespace Maestrano.Api
{
    public static class MnoClient
    {
        private static Dictionary<string, JsonClient> clientDict;
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();

        static MnoClient()
        {
            clientDict = new Dictionary<string, JsonClient>();
            jsonSerializerSettings.Converters.Add(new CorrectedIsoDateTimeConverter());
        }

        private static JsonClient Client(string presetName = "maestrano")
        {
            if (!clientDict.ContainsKey(presetName))
            {
                var preset = MnoHelper.With(presetName);
                string host = preset.Api.Host;
                string path = preset.Api.Base;
                string key = preset.Api.Id;
                string secret = preset.Api.Key;
                var client = new JsonClient(host, path, key, secret);
                clientDict.Add(presetName, client);
            }

            return clientDict[presetName];
        }

        /// <summary>
        /// Execute a manual REST request
        /// </summary>
        /// <typeparam name="T">The type of object to create and populate with the returned data.</typeparam>
        /// <param name="request">The RestRequest to execute (will use client credentials)</param>
        public static T ProjectSingleObject<T>(RestRequest request, string presetName = "maestrano")
        {
            request.OnBeforeDeserialization = (resp) =>
            {
                // for individual resources when there's an error to make
                // sure that RestException props are populated
                if (((int)resp.StatusCode) >= 400)
                    request.RootElement = "";
            };

            var response = Client(presetName).Execute(request);
            var respObj = DeserializeObject<MnoObject<T>>(response.Content);
            respObj.ThrowIfErrors();
            respObj.AssignPreset(presetName);

            return respObj.Data;
        }
        /// <summary>
        /// Deserializes the JSON to the specified .NET type
        /// </summary>
        public static U DeserializeObject<U>(string s)
        {
            return JsonConvert.DeserializeObject<U>(s, jsonSerializerSettings);
        }
        /// <summary>
        /// Execute a manual REST request
        /// </summary>
        /// <typeparam name="T">The type of object to create and populate with the returned data.</typeparam>
        /// <param name="request">The RestRequest to execute (will use client credentials)</param>
        public static List<T> ProcessList<T>(RestRequest request, string presetName = "maestrano")
        {
            request.OnBeforeDeserialization = (resp) =>
            {
                // for individual resources when there's an error to make
                // sure that RestException props are populated
                //if (((int)resp.StatusCode) >= 400)
                //    request.RootElement = "";
            };

            var response = Client(presetName).Execute(request);
            var respObj = DeserializeObject<MnoCollection<T>>(response.Content);
            respObj.ThrowIfErrors();
            respObj.AssignPreset(presetName);

            return respObj.Data;
        }

        public static List<T> All<T>(string path, NameValueCollection filters = null, string presetName = "maestrano")
        {
            var request = new RestRequest();
            request.Resource = path;
            request.Method = Method.GET;

            // Add query parameters
            if (filters != null)
                foreach (String k in filters.AllKeys)
                    request.AddParameter(k, filters[k]);

            return ProcessList<T>(request, presetName);
        }

        public static T Retrieve<T>(string path, string resourceId, string presetName = "maestrano")
        {
            var request = new RestRequest();
            request.Resource = path;
            request.Method = Method.GET;
            request.AddUrlSegment("id", resourceId);

            return ProjectSingleObject<T>(request, presetName);
        }

        public static T Create<T>(string path, NameValueCollection parameters, string presetName = "maestrano")
        {
            var request = new RestRequest();
            request.Resource = path;
            request.Method = Method.POST;

            foreach (var k in parameters.AllKeys)
                request.AddParameter(StringExtensions.ToSnakeCase(k), parameters[k]);

            return ProjectSingleObject<T>(request, presetName);
        }

        public static T Delete<T>(string path, string resourceId, string presetName = "maestrano")
        {
            var request = new RestRequest();
            request.Resource = path;
            request.Method = Method.DELETE;
            request.AddUrlSegment("id", resourceId);

            return ProjectSingleObject<T>(request, presetName);
        }
    }
}

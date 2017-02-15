using System;
using System.Collections.Generic;
using RestSharp;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Maestrano.Helpers;
using Maestrano.Net;
using Maestrano.Configuration;
using Newtonsoft.Json.Linq;
using Maestrano.Account;

namespace Maestrano.Api
{
    public class MnoClient<T> where T : MnoObject
    {
        protected readonly Preset preset;
        private readonly string indexPath;
        private readonly string resourcePath;
        private readonly JsonClient jsonClient;
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        static MnoClient()
        {
            jsonSerializerSettings.Converters.Add(new CorrectedIsoDateTimeConverter());
        }

        protected MnoClient(String indexPath, String resourcePath, Preset preset)
        {
            this.indexPath = indexPath;
            this.resourcePath = resourcePath;
            this.preset = preset;
            string host = preset.Api.Host;
            string basePath = preset.Api.Base;
            string key = preset.Api.Id;
            string secret = preset.Api.Key;
            this.jsonClient = new JsonClient(host, basePath, key, secret);
        }

        /// <summary>
        /// Execute a manual REST request
        /// </summary>
        /// <typeparam name="T">The type of object to create and populate with the returned data.</typeparam>
        /// <param name="request">The RestRequest to execute (will use client credentials)</param>
        private T ProjectSingleObject(RestRequest request)
        {
            request.OnBeforeDeserialization = (resp) =>
            {
                // for individual resources when there's an error to make
                // sure that RestException props are populated
                if (((int)resp.StatusCode) >= 400)
                    request.RootElement = "";
            };

            var response = jsonClient.Execute(request);
            var respObj = DeserializeObject<SingleResult>(response.Content);
            ThrowIfErrors(respObj.Errors);
            respObj.Data.Preset = preset;
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
        protected List<T> ProcessList(RestRequest request)
        {

            var response = jsonClient.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ResourceException("Error retrieving response.", response.ErrorException);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                throw new ResourceException("Could not process request: " + response.Content);
            }

            var respObj = DeserializeObject<CollectionResult>(response.Content);
            ThrowIfErrors(respObj.Errors);
            foreach (T item in respObj.Data)
            {
                item.Preset = preset;
            }
            return respObj.Data;
        }

        public List<T> All(string path, NameValueCollection filters = null)
        {
            var request = new RestRequest();
            request.Resource = path;
            request.Method = Method.GET;

            // Add query parameters
            if (filters != null)
                foreach (String k in filters.AllKeys)
                    request.AddParameter(k, filters[k]);

            return ProcessList(request);
        }

        public T Retrieve(string resourceId)
        {
            var request = new RestRequest();
            request.Resource = resourcePath;
            request.Method = Method.GET;
            request.AddUrlSegment("id", resourceId);

            return ProjectSingleObject(request);
        }

        protected T Create(NameValueCollection parameters)
        {
            return Create(this.indexPath, parameters);
        }

        protected T Create(string path, NameValueCollection parameters)
        {
            var request = new RestRequest();
            request.Resource = indexPath;
            request.Method = Method.POST;

            foreach (var k in parameters.AllKeys)
                request.AddParameter(StringExtensions.ToSnakeCase(k), parameters[k]);

            return ProjectSingleObject(request);
        }

        public T Delete(string resourceId)
        {
            var request = new RestRequest();
            request.Resource = indexPath;
            request.Method = Method.DELETE;
            request.AddUrlSegment("id", resourceId);

            return ProjectSingleObject(request);
        }

        private static void ThrowIfErrors(JObject Errors)
        {
            if (Errors.Count > 0)
            {
                var error = (JProperty)Errors.First;
                throw new ResourceException(error.Name + " " + error.Value);
            }
        }

        class SingleResult
        {
            public JObject Errors = null;
            public T Data = null;
            public string Success = null;

        }

        public class CollectionResult
        {
            public JObject Errors = null;
            public List<T> Data = null;
            public string Success = null;
        }
    }


}

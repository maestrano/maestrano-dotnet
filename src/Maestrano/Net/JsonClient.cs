using Maestrano.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Authenticators;

namespace Maestrano.Net
{
    public class JsonClient
    {
        /// <summary>
        /// Internal configured RestClient
        /// </summary>
        public RestClient InternalClient{ get; private set; }

        public JsonClient(string host, string path, string key, string secret)
        {

            InternalClient = new RestClient();
            InternalClient.UserAgent = "maestrano-dotnet/" + VersionHelper.GetVersion();
            InternalClient.AddDefaultHeader("Accept", "application/vnd.api+json");
            InternalClient.AddDefaultHeader("Content-Type", "application/vnd.api+json");
            InternalClient.Authenticator = new HttpBasicAuthenticator(key, secret);
            InternalClient.BaseUrl = new Uri(string.Format("{0}{1}", host, path));
        }

        public RestResponse Execute(RestRequest request)
        {
            return (RestResponse)InternalClient.Execute(request);
        }

        public RestResponse<T> Execute<T>(RestRequest request) where T : new()
        {
            return (RestResponse <T>)InternalClient.Execute<T>(request);
        }

        public RestResponse Get(string path)
        {
            return Execute(PrepareRequest(Method.GET, path));
        }

        public RestResponse Get(string path, NameValueCollection parameters)
        {
            return Execute(PrepareRequest(Method.GET, path, parameters));
        }

        public RestResponse<T> Get<T>(string path) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.GET, path));
        }

        public RestResponse<T> Get<T>(string path, NameValueCollection parameters) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.GET, path, parameters));
        }

        public RestResponse Post(string path, string jsonBody)
        {
            return Execute(PrepareRequest(Method.POST, path, null, jsonBody));
        }

        public RestResponse<T> Post<T>(string path, string jsonBody) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.POST, path, null, jsonBody));
        }

        public RestResponse Put(string path, string jsonBody)
        {
            return Execute(PrepareRequest(Method.PUT, path, null, jsonBody));
        }

        public RestResponse<T> Put<T>(string path, string jsonBody) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.PUT, path, null, jsonBody));
        }

        /// <summary>
        /// Prepare a Request for a given path, parameters and body.
        /// Parameters keys will be converted to snake case.
        /// </summary>
        public RestRequest PrepareRequest(Method method, String path, NameValueCollection parameters = null, String body = null)
        {
            var request = new RestRequest();
            request.Resource = path;
            request.Method = method;
            request.AddHeader("Accept", "application/vnd.api+json");
            request.AddHeader("Content-Type", "application/vnd.api+json");
            request.Parameters.Clear();

            if (body != null)
                request.AddParameter("application/vnd.api+json", body, ParameterType.RequestBody);

            if (parameters != null)
            {
                foreach (var k in parameters.AllKeys)
                    request.AddParameter(StringExtensions.ToSnakeCase(k), parameters[k]);
            }

            return request;
        }


    }
}

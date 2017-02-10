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
        private readonly RestClient client;

        public JsonClient(string host, string path, string key, string secret)
        {

            client = new RestClient();
            client.UserAgent = "maestrano-dotnet/" + VersionHelper.GetVersion();
            client.AddDefaultHeader("Accept", "application/vnd.api+json");
            client.AddDefaultHeader("Content-Type", "application/vnd.api+json");
            client.Authenticator = new HttpBasicAuthenticator(key, secret);
            client.BaseUrl = new Uri(string.Format("{0}{1}", host, path));

        }

        public RestResponse Execute(IRestRequest request)
        {
            return (RestResponse)client.Execute(request);
        }

        public RestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            return (RestResponse <T>)client.Execute<T>(request);
        }

        public RestResponse Get(string path)
        {
            return Execute(PrepareRequest(Method.GET, path, null, null));
        }

        public RestResponse Get(string path, NameValueCollection parameters)
        {
            return Execute(PrepareRequest(Method.GET, path, parameters, null));
        }

        public RestResponse<T> Get<T>(string path) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.GET, path, null, null));
        }

        public RestResponse<T> Get<T>(string path, NameValueCollection parameters) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.GET, path, parameters, null));
        }



        public IRestResponse Post(string path, string jsonBody)
        {
            return Execute(PrepareRequest(Method.POST, path, null, jsonBody));
        }

        public RestResponse<T> Post<T>(string path, string jsonBody) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.POST, path, null, jsonBody));
        }


        public IRestResponse Put(string path, string jsonBody)
        {
            return Execute(PrepareRequest(Method.PUT, path, null, jsonBody));
        }

        public RestResponse<T> Put<T>(string path, string jsonBody) where T : new()
        {
            return Execute<T>(PrepareRequest(Method.PUT, path, null, jsonBody));
        }

        private IRestRequest PrepareRequest(Method method, String path, NameValueCollection parameters, String body)
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

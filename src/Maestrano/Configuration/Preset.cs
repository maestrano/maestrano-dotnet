using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Maestrano.Account;


namespace Maestrano.Configuration
{
    /// <summary>
    /// Configuration Preset for a given Marketplace
    /// </summary>
    public class Preset
    {
        /// <summary>
        /// Id of the marketplace of this configuration
        /// </summary>
        public string Marketplace { get; set; }

        public Sso Sso { get; private set; }
        public App App { get; private set; }
        public Api Api { get; private set; }
        public Connec Connec { get; private set; }
        public Webhook Webhook { get; private set; }

        /// <summary>
        /// Constructor only used for testing
        /// </summary>
        /// <param name="marketplace">The marketplace (e.g.: maestrano)</param>
        public Preset(string marketplace)
        {
            Marketplace = marketplace;
            App = new App();
            Api = new Api();
            Connec = new Connec();
            Webhook = new Webhook();
            Sso = new Sso(marketplace, Api);
        }

        public Preset(JObject obj)
        {
            Marketplace = obj["marketplace"].Value<string>();
            App = App.LoadFromJson(obj["app"].Value<JObject>());
            Api = Api.LoadFromJson(obj["api"].Value<JObject>());
            Connec = Connec.LoadFromJson(obj["connec"].Value<JObject>());
            Webhook = Webhook.LoadFromJson(obj["webhooks"].Value<JObject>());
            Sso = Sso.LoadFromJson(Marketplace, App, Api, obj["sso"].Value<JObject>());
        }

        /// <summary>
        /// Check that ID and Key passed in argument match
        /// the ones configured at the API level.
        /// Used for remote authentication from Maestrano.
        /// </summary>
        /// <param name="apiId">An application ID</param>
        /// <param name="apiKey">An API Key</param>
        /// <returns>true if the authentication is successful, false otherwise</returns>
        public bool Authenticate(string apiId, string apiKey)
        {
            return Api.Id == apiId && Api.Key == apiKey;
        }

        /// <summary>
        /// Authenticate a request from Maestrano using HTTP Basic Authentication
        /// </summary>
        /// <param name="request">An HttpRequest object</param>
        /// <returns>true if the authentication is successful, false otherwise</returns>
        public bool Authenticate(System.Web.HttpRequest request)
        {
            string authHeader = request.Headers["Authorization"];
            bool authenticated = false;
            // RFC 2617 sec 1.2, "scheme" name is case-insensitive
            if (authHeader != null && authHeader.ToLower().StartsWith("basic"))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                int seperatorIndex = usernamePassword.IndexOf(':');
                var apiId = usernamePassword.Substring(0, seperatorIndex);
                var apiKey = usernamePassword.Substring(seperatorIndex + 1);
                authenticated = Authenticate(apiId, apiKey);
            }
            return authenticated;
        }

        /// <summary>
        /// Take a user uid (either real like 'usr-1' or virtual like
        /// 'usr-1.cld-3') and return the real uid part
        /// </summary>
        /// <param name="userUid">A real or virtual uid</param>
        /// <returns>Real user uid</returns>
        public string UnmaskUser(string userUid)
        {
            string[] words = userUid.Split('.');
            if (words.Length > 0)
            {
                return words.First();
            }
            return userUid;
        }

        /// <summary>
        /// Return a json serializable object describing the current 
        /// Maestrano configuration. The metadata will be fetched remotely
        /// by Maestrano. Note that the metadata exclude any info
        /// including an API Key.
        /// </summary>
        /// <returns>JObject which can be converted to JSON using ToString()</returns>
        public JObject ToMetadata()
        {
            JObject metadata = new JObject(
                new JProperty("app", new JObject(new JProperty("host", App.Host))),
                new JProperty("api", new JObject(
                    new JProperty("id", Api.Id),
                    new JProperty("lang", Api.Lang),
                    new JProperty("version", Api.Version),
                    new JProperty("lang_version", Api.LangVersion))),
                new JProperty("sso", new JObject(
                    new JProperty("init_path", Sso.InitPath),
                    new JProperty("consume_path", Sso.ConsumePath),
                    new JProperty("idm", Sso.Idm),
                    new JProperty("idp", Sso.Idp),
                    new JProperty("name_id_format", Sso.NameIdFormat),
                    new JProperty("x509_fingerprint", Sso.X509Fingerprint),
                    new JProperty("x509_certificate", Sso.X509Certificate))),
                new JProperty("webhook", new JObject(
                    new JProperty("account", new JObject(
                        new JProperty("group_path", Webhook.Account.GroupPath),
                        new JProperty("group_user_path", Webhook.Account.GroupUserPath)
                        )),
                    new JProperty("connec", new JObject(
                        new JProperty("notification_path", Webhook.Connec.NotificationPath),
                        new JProperty("external_ids", Webhook.Connec.ExternalIds),
                        new JProperty("initialization_path", Webhook.Connec.InitializationPath)
                        ))))
            );

            return metadata;
        }

        public BillRequestor Bill { get { return new BillRequestor(this); } }

        public GroupRequestor Group { get { return new GroupRequestor(this); } }

        public RecurringBillRequestor RecurringBill { get { return new RecurringBillRequestor(this); } }

        public UserRequestor User { get { return new UserRequestor(this); } }

        public Maestrano.Connec.Client ConnecClient(String groupId)
        {
            return Maestrano.Connec.Client.New(this, groupId);
        }
    }
}

using Maestrano.Sso;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.SessionState;

namespace Maestrano.Configuration
{
    public class Sso
    {
        private const string IdpDefaultNameId = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";
        private const string SamlIdpPath = "/api/v1/auth/saml";
        private const string IdpLogoutPath = "/app_logout";
        private const string IdpUnauthorizedPath = "/app_access_unauthorized";

        private readonly Api apiConfiguration;

        public Sso(string marketplace, Api apiConfiguration)
        {
            this.Marketplace = marketplace;
            this.apiConfiguration = apiConfiguration;
        }

        /// <summary>
        /// Load configuration into a Sso configuration object from a JObject 
        /// </summary>
        /// <returns>A Sso configuration object</returns>
        public static Sso LoadFromJson(String marketplace, App appConfiguration, Api apiConfiguration, JObject obj)
        {
            var config = new Sso(marketplace, apiConfiguration);
            config.InitPath = obj["init_path"].Value<String>();
            config.ConsumePath = obj["consume_path"].Value<String>();
            var idm = obj["idm"].Value<String>();
            // if idm is null, we take the host by default
            if (String.IsNullOrEmpty(idm))
            {
                idm = appConfiguration.Host;
            }
            config.Idm = idm;
            config.Idp = obj["idp"].Value<String>();
            config.X509Fingerprint = obj["x509_fingerprint"].Value<String>();
            config.X509Certificate = obj["x509_certificate"].Value<String>();
            return config;
        }

        /// <summary>
        /// Name of the marketplace
        /// </summary>
        public String Marketplace { get; set; }


        /// <summary>
        /// Path to init action
        /// </
        public String InitPath { get; set; }

        /// <summary>
        /// Path to consume action
        /// </summary>
        public String ConsumePath { get; set; }

        /// <summary>
        /// Address of the identity manager (for your application)
        /// </summary>
        public String Idm { get; set; }

        /// <summary>
        /// Address of the identity provider
        /// </summary>
        public String Idp { get; set; }

        /// <summary>
        /// The nameid format for SAML handshake
        /// </summary>
        public String NameIdFormat { get { return IdpDefaultNameId; } }


        /// <summary>
        /// Fingerprint of x509 certificate used for SAML
        /// </summary>
        public String X509Fingerprint { get; set; }

        // Actual x509 certificate
        public String X509Certificate { get; set; }

        /// <summary>
        /// Return the IDP url to be used for SSO handshake
        /// </summary>
        /// <returns></returns>
        public string IdpUrl()
        {
            return Idp + SamlIdpPath;
        }

        /// <summary>
        /// Return the complete SSO init url for this application
        /// </summary>
        /// <returns></returns>
        public string InitUrl()
        {
            return Idm + InitPath;
        }

        /// <summary>
        /// Return the complete SSO consume url for this application
        /// </summary>
        public string ConsumeUrl()
        {
            return Idm + ConsumePath;
        }

        /// <summary>
        /// return the Maestrano logout url to be used for redirecting a user after logout
        /// </summary>
        [Obsolete("LogoutUrl is deprecated, please use LogoutUrl(userUid) instead.")]
        public string LogoutUrl()
        {
            return Idp + IdpLogoutPath;
        }

        /// <summary>
        /// return the Maestrano logout url to be used for redirecting a user after logout
        /// </summary>
        public string LogoutUrl(Account.User user)
        {
            return LogoutUrl(user.Id);
        }

        /// <summary>
        /// return the Maestrano logout url to be used for redirecting a user after logout
        /// </summary>
        public string LogoutUrl(String userUid)
        {
            return Idp + IdpLogoutPath + "?user_uid=" + userUid;
        }

        /// <summary>
        /// Return the Maestrano unauthorized page url to
        /// be used when a user is denied access to the application.
        /// Should not need to be used as Maestrano evaluates access
        /// permissions during SSO handshake.
        /// </summary>
        /// <returns></returns>
        public string UnauthorizedUrl()
        {
            return Idp + IdpUnauthorizedPath;
        }

        /// <summary>
        /// Build the url used for user session check
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string SessionCheckUrl(String uid, String session)
        {
            string url = Idp + SamlIdpPath;
            url += "/" + uid + "?session=" + session;

            return url;
        }

        /// <summary>
        /// Return the SAML Settings to be used by the SAML Request
        /// and Response classes
        /// </summary>
        /// <returns></returns>
        public Saml.Settings SamlSettings()
        {
            Saml.Settings settings = new Saml.Settings();
            settings.AssertionConsumerServiceUrl = ConsumeUrl();
            settings.IdpSsoTargetUrl = IdpUrl();
            settings.IdpCertificate = X509Certificate;
            settings.Issuer = apiConfiguration.Id;
            settings.NameIdentifierFormat = NameIdFormat;

            return settings;
        }

        /// <summary>
        /// Build a SAML Request ready to be sent
        /// You can call RedirectUrl() on the returned request
        /// and redirect the user to that url to trigger the Maestrano
        /// SSO handshake
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Saml.Request BuildRequest(NameValueCollection parameters = null)
        {
            return new Saml.Request(SamlSettings(), parameters);
        }

        /// <summary>
        /// Build a Saml Response object from a base64 encoded response
        /// </summary>
        /// <param name="samlPostParam">The value of the SAMLResponse POST parameter</param>
        /// <returns></returns>
        public Saml.Response BuildResponse(String samlPostParam)
        {

            return Saml.Response.LoadFromBase64XML(this, samlPostParam);
        }

        /// <summary>
        /// Set the maestrano user in session
        /// </summary>
        public void SetSession(HttpSessionStateBase httpSessionObj, User user)
        {
            var mnoSession = new Session(this, httpSessionObj, user);
            mnoSession.Save();
        }

        /// <summary>
        /// Set the maestrano user in session
        /// </summary>
        public void SetSession(HttpSessionState httpSessionObj, User user)
        {
            var mnoSession = new Session(this, httpSessionObj, user);
            mnoSession.Save();
        }

        /// <summary>
        /// Clear the maestrano session
        /// </summary>
        /// <param name="httpSessionObj"></param>
        public void ClearSession(HttpSessionStateBase httpSessionObj)
        {
            httpSessionObj.Remove(Marketplace);
        }

        /// <summary>
        /// Clear the maestrano session
        /// </summary>
        /// <param name="httpSessionObj"></param>
        public void ClearSession(HttpSessionState httpSessionObj)
        {
            httpSessionObj.Remove(Marketplace);
        }


        /// <summary>
        /// Is Single Sign-On enabled - useful for debugging
        /// </summary>
        [Obsolete("Enabled is deprecated, this will always return true")]
        public bool Enabled { get { return true; } set { } }

        /// <summary>
        /// Is Single Logout enabled - useful for debugging
        /// </summary>
        [Obsolete("SloEnabled is deprecated, this will always return true")]
        public bool SloEnabled { get { return true; } set {} }

    }
}

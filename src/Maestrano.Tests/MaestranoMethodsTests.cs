using System;
using Maestrano.Saml;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Web;
using Maestrano.Sso;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.SessionState;
using Maestrano.Configuration;

namespace Maestrano.Tests
{
    public class MaestranoMethodsTests
    {
        Preset preset;

        [TestFixtureSetUp]
        public void TestInitialize()
        {
            Helpers.destroyMnoSession();
            preset = new Preset("test");
        }

        [Test]
        public void Authenticate_ItReturnsTheRightValues()
        {
            preset.Api.Id = "app-1";
            preset.Api.Key = "somekey";

            Assert.AreEqual(true, preset.Authenticate(preset.Api.Id, preset.Api.Key));
            Assert.AreEqual(false, preset.Authenticate(preset.Api.Id, "someotherrandomkey"));
        }

        [TestCase]
        public void UnmaskUser_ItReturnsTheRightValues()
        {
            Assert.AreEqual("usr-1", preset.UnmaskUser("usr-1"));
            Assert.AreEqual("usr-1", preset.UnmaskUser("usr-1.cld-4"));
        }


        [Test]
        public void Sso_SessionCheckUrl_ItsReturnsTheRightUrl()
        {
            preset.Sso.Idp = "someurl";
            Assert.AreEqual(preset.Sso.Idp + "/api/v1/auth/saml/usr-1?session=somesessiontoken", preset.Sso.SessionCheckUrl("usr-1", "somesessiontoken"));
        }

        [Test]
        public void Sso_IdpUrl_ItsReturnsTheRightUrl()
        {
            preset.Sso.Idp = "someurl";
            Assert.AreEqual(preset.Sso.Idp + "/api/v1/auth/saml", preset.Sso.IdpUrl());
        }

        [Test]
        public void Sso_InitUrl_ItReturnsTheRightUrl()
        {
            preset.Sso.Idm = "someurl";
            Assert.AreEqual("someurl" + preset.Sso.InitPath, preset.Sso.InitUrl());
        }

        [Test]
        public void Sso_ConsumeUrl_ItReturnsTheRightUrl()
        {
            preset.Sso.Idm = "someurl";
            Assert.AreEqual("someurl" + preset.Sso.ConsumePath, preset.Sso.ConsumeUrl());
        }

        [Test]
        public void Sso_LogoutUrl_ItReturnsTheRightUrl()
        {
            preset.Sso.Idp = "someurl";
            Assert.AreEqual("someurl/app_logout", preset.Sso.LogoutUrl());
        }

        [Test]
        public void Sso_UnauthorizedUrl_ItReturnsTheRightUrl()
        {
            preset.Sso.Idp = "someurl";
            Assert.AreEqual("someurl/app_access_unauthorized", preset.Sso.UnauthorizedUrl());
        }

        [Test]
        public void Sso_SamlSettings_ItReturnsTheRightSamlSettings()
        {

            preset.App.Host = "https://mysuperapp.com";
            preset.Api.Id = "app-1";
            preset.Sso.Idm = "someurl";
            preset.Sso.Idp = "https://api-hub.maestrano.com";
            preset.Sso.X509Certificate = "X509Certificate";

            Settings settings = preset.Sso.SamlSettings();

            Assert.AreEqual("https://api-hub.maestrano.com/api/v1/auth/saml", settings.IdpSsoTargetUrl);
            Assert.AreEqual(preset.Api.Id, settings.Issuer);
            Assert.AreEqual(preset.Sso.NameIdFormat, settings.NameIdentifierFormat);
            Assert.AreEqual(preset.Sso.Idm + preset.Sso.ConsumePath, settings.AssertionConsumerServiceUrl);
            Assert.AreEqual(preset.Sso.X509Certificate, settings.IdpCertificate);
        }

        [Test]
        public void Sso_BuildRequest_ItShouldReturnSamlRequest()
        {
            Assert.IsInstanceOf(typeof(Request), preset.Sso.BuildRequest());
        }

        [Test]
        public void Sso_BuildResponse_ItShouldReturnSamlRequest()
        {
            preset.App.Host = "https://mysuperapp.com";
            preset.Sso.Idm = "someurl";
            preset.Sso.Idp = "https://api-hub.maestrano.com";
            preset.Sso.X509Certificate = Helpers.CERTIFICATE;

            string samlResponseStr = Helpers.ReadSamlSupportFiles("Responses/response1.xml.base64");
            Assert.IsInstanceOf(typeof(Response), preset.Sso.BuildResponse(samlResponseStr));
        }

        [Test]
        public void Sso_SetSession_ItShouldSetTheUserInSession()
        {
            // Build context
            var samlResp = new SsoResponseStub();
            var att = samlResp.GetAttributes();
            var user = new User(samlResp);
            var session = Helpers.FakeHttpSessionState(preset);
            preset.Sso.SetSession(session, user);

            // Decrypt session
            var enc = System.Text.Encoding.UTF8;
            var json = enc.GetString(Convert.FromBase64String(session[preset.Marketplace].ToString()));
            var mnoObj = JObject.Parse(json);

            Assert.AreEqual(user.SsoSession, mnoObj.Value<String>("session"));
            Assert.AreEqual(user.Uid, mnoObj.Value<String>("uid"));
            Assert.AreEqual(user.GroupUid, mnoObj.Value<String>("group_uid"));
            Assert.AreEqual(user.SsoSessionRecheck, mnoObj.Value<DateTime>("session_recheck"));
        }

        [Test]
        public void Sso_ClearSession_ItShouldDeleteTheMaestranoSession()
        {

            // Build context
            var samlResp = new SsoResponseStub();
            var att = samlResp.GetAttributes();
            var user = new User(samlResp);
            var session = Helpers.FakeHttpSessionState(preset);
            preset.Sso.SetSession(session, user);

            // Test
            preset.Sso.ClearSession(session);
            Assert.IsNull(session[preset.Marketplace]);
        }

        [Test]
        public void RegisterMarketplace_ItRegisterFromFile()
        {
            var byteArray = Properties.Resources.developerPlatformAnswer;
            var content = System.Text.Encoding.UTF8.GetString(byteArray);

            var preset = MnoHelper.RegisterMarketplaceFromJson(content);
            Assert.AreEqual("http://localhost:63705", preset.App.Host);
        }

    }
}

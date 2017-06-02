using System;
using Maestrano.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Maestrano.Tests.Configuration
{
    public class PresetTest
    {
        Preset preset;

        [TestFixtureSetUp]
        public void TestInitialize()
        {
            var byteArray = Properties.Resources.developerPlatformAnswer;
            var content = System.Text.Encoding.UTF8.GetString(byteArray);
            var jObject = JObject.Parse(content);
            preset = new Preset(jObject);
        }

        [Test]
        public void ItHasTheRightConfig()
        {
            // App
            Assert.AreEqual("http://localhost:63705", preset.App.Host);
            // API
            Assert.AreEqual("https://api-hub-uat.maestrano.io", preset.Api.Host);
            Assert.AreEqual("/api/v1/", preset.Api.Base);
            Assert.AreEqual("C#", preset.Api.Lang);
            Assert.AreEqual("71eb4e3b13f62b7fcf305d9d913bc25067354e6230b4f20fbb16a73d7a0c3be7", preset.Api.Key);
            Assert.AreEqual("app-15ds", preset.Api.Id);
            Assert.AreEqual(MnoHelper.Version, preset.Api.Version);
            Assert.AreEqual(Environment.OSVersion.ToString() + " - " + Environment.Version.ToString(), preset.Api.LangVersion);
            // SSO
            Assert.AreEqual("https://api-hub-uat.maestrano.io", preset.Sso.Idp);
            Assert.AreEqual("http://localhost:63705", preset.Sso.Idm);
            Assert.AreEqual("/maestrano/init/?marketplace=maestrano-uat", preset.Sso.InitPath);
            Assert.AreEqual("/maestrano/consume/?marketplace=maestrano-uat", preset.Sso.ConsumePath);
            Assert.AreEqual("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent", preset.Sso.NameIdFormat);
            Assert.AreEqual("8a:1e:2e:76:c4:67:80:68:6c:81:18:f7:d3:29:5d:77:f8:79:54:2f", preset.Sso.X509Fingerprint);
            Assert.AreEqual("-----BEGIN CERTIFICATE-----\nMIIDezCCAuSgAwIBAgIJAMzy+weDPp7qMA0GCSqGSIb3DQEBBQUAMIGGMQswCQYD\nVQQGEwJBVTEMMAoGA1UECBMDTlNXMQ8wDQYDVQQHEwZTeWRuZXkxGjAYBgNVBAoT\nEU1hZXN0cmFubyBQdHkgTHRkMRYwFAYDVQQDEw1tYWVzdHJhbm8uY29tMSQwIgYJ\nKoZIhvcNAQkBFhVzdXBwb3J0QG1hZXN0cmFuby5jb20wHhcNMTQwMTA0MDUyMzE0\nWhcNMzMxMjMwMDUyMzE0WjCBhjELMAkGA1UEBhMCQVUxDDAKBgNVBAgTA05TVzEP\nMA0GA1UEBxMGU3lkbmV5MRowGAYDVQQKExFNYWVzdHJhbm8gUHR5IEx0ZDEWMBQG\nA1UEAxMNbWFlc3RyYW5vLmNvbTEkMCIGCSqGSIb3DQEJARYVc3VwcG9ydEBtYWVz\ndHJhbm8uY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC+2uyQeAOc/iro\nhCyT33RkkWfTGeJ8E/mu9F5ORWoCZ/h2J+QDuzuc69Rf1LoO4wZVQ8LBeWOqMBYz\notYFUIPlPfIBXDNL/stHkpg28WLDpoJM+46WpTAgp89YKgwdAoYODHiUOcO/uXOO\n2i9Ekoa+kxbvBzDJf7uuR/io6GERXwIDAQABo4HuMIHrMB0GA1UdDgQWBBTGRDBT\nie5+fHkB0+SZ5g3WY/D2RTCBuwYDVR0jBIGzMIGwgBTGRDBTie5+fHkB0+SZ5g3W\nY/D2RaGBjKSBiTCBhjELMAkGA1UEBhMCQVUxDDAKBgNVBAgTA05TVzEPMA0GA1UE\nBxMGU3lkbmV5MRowGAYDVQQKExFNYWVzdHJhbm8gUHR5IEx0ZDEWMBQGA1UEAxMN\nbWFlc3RyYW5vLmNvbTEkMCIGCSqGSIb3DQEJARYVc3VwcG9ydEBtYWVzdHJhbm8u\nY29tggkAzPL7B4M+nuowDAYDVR0TBAUwAwEB/zANBgkqhkiG9w0BAQUFAAOBgQAw\nRxg3rZrML//xbsS3FFXguzXiiNQAvA4KrMWhGh3jVrtzAlN1/okFNy6zuN8gzdKD\nYw2n0c/u3cSpUutIVZOkwQuPCMC1hoP7Ilat6icVewNcHayLBxKgRxpBhr5Sc4av\n3HOW5Bi/eyC7IjeBTbTnpziApEC7uUsBou2rlKmTGw==\n-----END CERTIFICATE-----\n", preset.Sso.X509Certificate);
            // Connec
            Assert.AreEqual("https://api-connec-uat.maestrano.io", preset.Connec.Host);
            Assert.AreEqual("/api/v2", preset.Connec.BasePath);
            // Webhook
            Assert.AreEqual("/maestrano/account/groups/:id/maestrano-uat", preset.Webhook.Account.GroupPath);
            Assert.AreEqual("/maestrano/account/groups/:group_id/users/:id/maestrano-uat", preset.Webhook.Account.GroupUserPath);
            Assert.AreEqual("/maestrano/connec/notifications/maestrano-uat", preset.Webhook.Connec.NotificationPath);
            Assert.AreEqual(true, preset.Webhook.Connec.ExternalIds);
            Assert.IsNull(preset.Webhook.Connec.InitializationPath);
        }

        [Test]
        public void ItGeneratesTheMetadataWithoutError() { 
        
            Assert.IsNotNull(preset.ToMetadata());
        }
    }
}

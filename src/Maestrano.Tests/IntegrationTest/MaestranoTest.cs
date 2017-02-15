using NUnit.Framework;
using System.Net;

namespace Maestrano.Tests.IntegrationTest
{
    public class MaestranoTest
    {

        [TestFixtureSetUp]
        public void TestInitialize()
        {
            // Adding TSL12 Security Protocol
            // Maestrano does not support SSL3 protocol as it is insecure
            // Maestrano supports TLS 1.2
            // https://blog.mozilla.org/security/2014/10/14/the-poodle-attack-and-the-end-of-ssl-3-0/
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        [Test]
        [ExpectedException(typeof(AutoConfigureException), ExpectedMessage = "Could not authenticate. Check your key and secret")]
        public void AutoConfigure_invalidCredential_throwsAnAutoConfigureException()
        {
            var host = "https://dev-platform.maestrano.io";
            var path = "/api/config/v1";
            var key = "INVALID";
            var secret = "INVALID";
            MnoHelper.AutoConfigure(host, path, key, secret);
        }


        [Test]
        public void AutoConfigure_withParameters()
        {

            var host = "https://developer-uat.maestrano.io";
            var path = "/api/config/v1";
            var key = "08440d6f-a16c-4f89-87d3-603113a1d099";
            var secret = "xAzIal70cmiR4Y_dkQZF-A";
            MnoHelper.AutoConfigure(host, path, key, secret);

        }

        [Test]
        [ExpectedException(typeof(AutoConfigureException))]
        public void AutoConfigure()
        {
            MnoHelper.AutoConfigure();
        }

    }
}

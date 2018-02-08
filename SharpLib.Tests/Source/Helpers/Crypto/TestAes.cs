using NUnit.Framework;
using SharpLib.Source.Helpers.Crypto;

namespace SharpLib.Tests.Source.Helpers.Crypto
{
    [TestFixture]
    public class TestAes
    {
        [Test]
        [TestCase("123", "123", ExpectedResult = "25f8S0DQ6zXr5aPdxxm9LA==")]
        public string Encrypt(string key, string data)
        {
            var res = Aes.Encrypt(key, data);

            return res;
        }

        [Test]
        [TestCase("123", "25f8S0DQ6zXr5aPdxxm9LA==", ExpectedResult = "123")]
        public string Decrypt(string key, string data)
        {
            var res = Aes.Decrypt(key, data);

            return res;
        }
    }
}

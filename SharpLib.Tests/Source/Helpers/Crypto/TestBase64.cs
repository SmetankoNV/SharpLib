using NUnit.Framework;
using SharpLib.Source.Extensions;
using SharpLib.Source.Extensions.String;
using SharpLib.Source.Helpers.Crypto;

namespace SharpLib.Tests.Source.Helpers.Crypto
{
    [TestFixture]
    public class TestBase64
    {
        [Test]
        [TestCase("123", ExpectedResult = "MTIz")]
        public string Encrypt(string plain)
        {
            var bytes = plain.ToBytesEx();
            var res = Base64.Encrypt(bytes);

            return res;
        }

        [Test]
        [TestCase("MTIz", ExpectedResult = "123")]
        public string Decrypt(string cripto)
        {
            var bytes = Base64.Decrypt(cripto);
            var res = bytes.ToStringEx();

            return res;
        }
    }
}

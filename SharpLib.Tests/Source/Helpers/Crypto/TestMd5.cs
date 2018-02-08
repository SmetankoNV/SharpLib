using NUnit.Framework;
using SharpLib.Source.Helpers.Crypto;

namespace SharpLib.Tests.Source.Helpers.Crypto
{
    [TestFixture]
    public class TestMd5
    {
        [Test]
        [TestCase("123", ExpectedResult = "202cb962ac59075b964b07152d234b70")]
        public string Hash(string data)
        {
            var res = Md5.Hash(data);

            return res;
        }
    }
}

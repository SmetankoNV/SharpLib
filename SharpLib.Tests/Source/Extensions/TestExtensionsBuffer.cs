using NUnit.Framework;
using SharpLib.Source.Extensions;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Tests.Source.Extensions
{
    [TestFixture(Category = "Extensions")]
    public class TestExtensionsBuffer
    {
        [Test]
        [TestCase("00-00-20-40", 2.5f)]
        [TestCase("66-66-A6-40", 5.2f)]
        public void GetFloat(string text, float value)
        {
            var buffer = text.ToAsciiBufferEx("-");
            var res = buffer.GetFloatEx(0);

            Assert.AreEqual(res, value);
        }
    }
}

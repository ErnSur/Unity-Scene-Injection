using System;
using NUnit.Framework;
using QuickEye.SceneInjection.FractionalIndexing;

namespace QuickEye.SceneInjection.Tests
{
    public class Tests
    {
        // [ expands to "", and ] expands to null.
        [TestCase("1", "2", "15")]
        [TestCase("1", "13", "12")]
        [TestCase("1", "12", "11")]
        [TestCase("1", "11", "105")]
        [TestCase("1", "101", "1005")]
        [TestCase("1", "1000001", "10000005")]
        [TestCase("1", "111", "11")]
        [TestCase("", null, "5")]
        [TestCase("5", null, "8")]
        [TestCase("8", null, "9")]
        [TestCase("9", null, "95")]
        [TestCase("95", null, "98")]
        [TestCase("98", null, "99")]
        [TestCase("99", null, "995")]
        [TestCase("", "5", "3")]
        [TestCase("", "3", "2")]
        [TestCase("", "1", "05")]
        [TestCase("001", "001002", "001001")]
        [TestCase("001", "001001", "0010005")]
        public void Midpoint_Is_Deterministic_Base10(string a, string b, string expected)
        {
            var result = OrderKey.Midpoint(a, b,"0123456789");
            Assert.AreEqual(expected, result);
        }
        
        [TestCase("V", null, "k")]
        [TestCase("a0", "a1", "a0V")]
        [TestCase("a1", "a2", "a1V")]
        public void Midpoint_Is_Deterministic_Base62(string a, string b, string expected)
        {
            var result = OrderKey.Midpoint(a, b,OrderKey.Base62Digits);
            Assert.AreEqual(expected, result);
        }
        
        [TestCase(null, null, "a0")]
        [TestCase(null, "a0", "Zz")]
        [TestCase(null, "Zz", "Zy")]
        [TestCase("a0", null, "a1")]
        [TestCase("a1", null, "a2")]
        [TestCase("a0", "a1", "a0V")]
        [TestCase("a1", "a2", "a1V")]
        [TestCase("a0V", "a1", "a0k")]
        [TestCase("Zz", "a0", "ZzV")]
        [TestCase("Zz", "a1", "a0")]
        [TestCase(null, "Y00", "Xzzz")]
        [TestCase("bzz", null, "c000")]
        [TestCase("a0", "a0V", "a0G")]
        [TestCase("a0", "a0G", "a08")]
        [TestCase("b125", "b129", "b127")]
        [TestCase("a0", "a1V", "a1")]
        [TestCase("Zz", "a01", "a0")]
        [TestCase(null, "a0V", "a0")]
        [TestCase(null, "b999", "b99")]
        [TestCase(null, "A000000000000000000000000001", "A000000000000000000000000000V")]
        [TestCase("zzzzzzzzzzzzzzzzzzzzzzzzzzy", null, "zzzzzzzzzzzzzzzzzzzzzzzzzzz")]
        [TestCase("zzzzzzzzzzzzzzzzzzzzzzzzzzz", null, "zzzzzzzzzzzzzzzzzzzzzzzzzzzV")]
        // As much deterministic as the result of Math.Round
        public void GenerateKeyBetween_Is_Deterministic(string a, string b, string expected)
        {
            var result = OrderKey.GenerateKeyBetween(a, b);
            Assert.AreEqual(expected, result);
        }
        
        [TestCase(
            null,
            "A00000000000000000000000000",
            "invalid order key: A00000000000000000000000000"
        )]
        [TestCase("a00", null, "invalid order key: a00")]
        [TestCase("a00", "a1", "invalid order key: a00")]
        [TestCase("0", "1", "invalid order key head: 0")]
        [TestCase("a1", "a0", "a1 >= a0")]
        public void GenerateKeyBetween_Throws(string a, string b, string expectedMessage)
        {
            Assert.Throws<InvalidOperationException>(() => OrderKey.GenerateKeyBetween(a, b),expectedMessage);
        }
    }
}
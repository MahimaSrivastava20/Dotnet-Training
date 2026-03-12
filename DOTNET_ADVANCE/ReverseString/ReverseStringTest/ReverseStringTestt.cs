using System;
using System.Collections.Generic;
using System.Text;
using ReverseStrings.Features;

namespace ReverseStringTest
{
    [TestClass]
    public class ReverseStringTestt
    {
        [TestMethod]
        [DataRow("hell", "lleh")]
        [DataRow("world", "dlrow")]
        [DataRow("CSharp", "prahSC")]
        public void TestReverseString(string input, string expected)
        {
            var reverse= new ReverseStringProgram();

            var res = reverse.Reverse(input);

            Assert.AreEqual(expected, res);

        }

    }
}

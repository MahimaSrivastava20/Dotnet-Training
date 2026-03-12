using System;
using System.Collections.Generic;
using System.Text;
using EvenChecker;

namespace EvenCheckTests
{
    [TestClass]
    public class CheckEvenNumTest
    {
        [TestMethod]
        public void Number_Is_Even()
        {
            Numbercheck check = new Numbercheck();

            bool result = check.IsEven(4);

            Assert.IsTrue(result);
        }
        [TestMethod]
        public void Number_5_Is_Odd()
        {
            Numbercheck check = new Numbercheck();

            bool result = check.IsEven(5);

            Assert.IsFalse(result);
        }


}
}

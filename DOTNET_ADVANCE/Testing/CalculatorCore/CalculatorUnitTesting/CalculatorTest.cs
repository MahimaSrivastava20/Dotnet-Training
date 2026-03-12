using System;
using System.Collections.Generic;
using System.Text;
using CalculatorUnitTestion.Feature;


namespace CalculatorUnitTesting
{
    [TestClass]
    public class CalculatorTest
    {
        [TestMethod]
        public void Add_Giventwonum_resultint()
        {
            //Arrange
            var calculator = new Calculator();
            
            //Act
            int result = calculator.Add(6,2);

            //Assert
            Assert.AreEqual(result, 8);

        }
        [TestMethod]
        [DataRow(2,1,3)]
        [DataRow(11,1,12)]
        public void Add_Paraameterized(int a, int b, int expected)
        {
            //Arrange
            var calculator = new Calculator();

            //Act
            int result = calculator.Add(a, b);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}



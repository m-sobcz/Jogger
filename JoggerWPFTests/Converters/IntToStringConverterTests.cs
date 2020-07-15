using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Converters.Tests
{
    [TestClass()]
    public class IntToStringConverterTests
    {
        IntToStringConverter intToStringConverter = new IntToStringConverter();
        [TestInitialize]
        public void TestInitialize() 
        {
            
        }
        [DataTestMethod]
        [DataRow(0,"0")]
        [DataRow(74, "74")]
        [DataRow(-3, "-3")]
        public void Convert_Conversion_AreEqual(int number,string properOutput)
        {
            string converted =(string) intToStringConverter.Convert(number, typeof(string), null, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(properOutput,converted);
        }
        [DataTestMethod]
        [DataRow(0, "0")]
        [DataRow(74, "74")]
        [DataRow(-3, "-3")]
        public void ConvertBack_Conversion_AreEqual(int properOutput, string text)
        {
            int convertedBack = (int)intToStringConverter.ConvertBack(text, typeof(int), null, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(properOutput, convertedBack);
        }
    }
}
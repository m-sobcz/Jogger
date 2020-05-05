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
        [TestMethod()]
        public void Int5ConvertedTo5String()
        {
            IntToStringConverter intToStringConverter = new IntToStringConverter();
            object x = 5;
            string converted =(string) intToStringConverter.Convert(x, typeof(string), null, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual("5",converted);
        }
        [TestMethod()]
        public void String3ConvertedBackToInt()
        {
            IntToStringConverter intToStringConverter = new IntToStringConverter();
            object x = "3";
            int convertedBack = (int)intToStringConverter.ConvertBack(x, typeof(int), null, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(3, convertedBack);
        }
    }
}
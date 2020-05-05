using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using JoggerTests;

namespace Jogger.Behaviors.Tests
{
    [TestClass()]
    public class TextBoxAutoScrollAfterTextChangeTests
    {

        [STATestMethod]
        public void OnAttachedTest()
        {
            Behavior<TextBox> textBoxAutoScroll = new TextBoxAutoScrollAfterTextChange();
            TextBox textBox = new TextBox();
            textBox.MaxLines = 1;          
            textBoxAutoScroll.Attach(textBox);
            double initialVerticalOffset = textBox.VerticalOffset;
            textBox.Text = "First\nSecond\nThird";
            double finalVerticalOffset = textBox.VerticalOffset;
            Assert.IsTrue(finalVerticalOffset > initialVerticalOffset);            
        }

    }
}
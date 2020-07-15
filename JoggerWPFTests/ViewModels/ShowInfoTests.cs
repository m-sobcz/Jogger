using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels.Tests
{
    [TestClass()]
    public class ShowInfoTests
    {
        [TestMethod()]
        public void Show_TestTextEvent_EqualsTest()
        {
            ShowInfo showInfo = new ShowInfo();
            string testText = "";
            string testCaption = "";
            object testSender = new object();
            showInfo.ShowInformation += (object sender, string text, string caption) => { testText = text; testCaption = caption; testSender = sender; };
            showInfo.Show("test", "caption");
            Assert.AreEqual("test", testText);
        }
        [TestMethod()]
        public void Show_TestCaptionEvent_EqualsCaption()
        {
            ShowInfo showInfo = new ShowInfo();
            string testText = "";
            string testCaption = "";
            object testSender = new object();
            showInfo.ShowInformation += (object sender, string text, string caption) => { testText = text; testCaption = caption; testSender = sender; };
            showInfo.Show("test", "caption");
            Assert.AreEqual("caption", testCaption);
        }
    }
}
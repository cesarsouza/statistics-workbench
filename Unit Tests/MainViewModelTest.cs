using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.ViewModels;
using System.Windows;
using System.Linq;
using System.Windows.Input;

namespace Unit_Tests
{
    [TestClass]
    public class MainViewModelTest
    {

        [TestMethod]
        public void ConstructorTest1()
        {
            MainViewModel target = new MainViewModel();

            Assert.AreEqual(1, target.Data.Count);
            Assert.IsTrue(target.Distributions.Count > 0);
        }

        [TestMethod]
        public void Paste_SuccessTest1()
        {
            MainViewModel target = new MainViewModel();


            string text = ""
             + "0.111\t2.222\n"
             + "0.333\t4.111\n"
             + "2.421\t3.141";

            Clipboard.SetText(text);
            target.Paste_Executed(null, new RoutedEventArgs());

            Assert.AreEqual(4, target.Data.Count);
            Assert.AreEqual(0, target.Data[0].Value);
            Assert.AreEqual(1, target.Data[0].Weight);
            Assert.AreEqual(0.111, target.Data[1].Value);
            Assert.AreEqual(2.222, target.Data[1].Weight);
            Assert.AreEqual(0.333, target.Data[2].Value);
            Assert.AreEqual(4.111, target.Data[2].Weight);
            Assert.AreEqual(2.421, target.Data[3].Value);
            Assert.AreEqual(3.141, target.Data[3].Weight);
        }

        [TestMethod]
        public void Paste_SuccessTest2()
        {
            MainViewModel target = new MainViewModel();


            string text = ""
             + "0.111\t2.222\n"
             + "A\tB\n"
             + "2.421\t3.141";

            Clipboard.SetText(text);
            target.Paste_Executed(null, new RoutedEventArgs());

            Assert.AreEqual(3, target.Data.Count);
            Assert.AreEqual(0, target.Data[0].Value);
            Assert.AreEqual(1, target.Data[0].Weight);
            Assert.AreEqual(0.111, target.Data[1].Value);
            Assert.AreEqual(2.222, target.Data[1].Weight);
            Assert.AreEqual(2.421, target.Data[2].Value);
            Assert.AreEqual(3.141, target.Data[2].Weight);
        }
    }
}

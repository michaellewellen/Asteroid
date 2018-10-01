using System;
using Asteroids;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
       

        [TestMethod]
        public void SetPictureTest()
        {
            Pictures TestProject1 = new Pictures(1, "Test");

            Assert.AreNotEqual(2, TestProject1.Id);
            Assert.AreEqual("Test", TestProject1.Type);

        }
    }
}

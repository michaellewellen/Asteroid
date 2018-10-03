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
            SpaceObject TestProject1 = new SpaceObject('G', 0, 0, 20, 2, 0);

            Assert.AreNotEqual(2, TestProject1.XCoordinate);
            Assert.AreEqual(0, TestProject1.Theta);

        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CjoSurveyApp;
using CjoSurveyAppCore;

namespace SurveyAppTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Arrange
            byte[] msg = { 0xB5, 0x62, 0x06, 0x08, 0x06, 0x00, 0xC8, 0x00, 0x01, 0x00, 0x01, 0x00, 0xDE, 0x6A };
            byte[] result = { 0xDE, 0x6A };
            //Act
            byte[] checksumOk = Checksum.VerifyChecksum(msg);
            //Assert
            Assert.AreEqual(result[0], checksumOk[0]);
            Assert.AreEqual(result[1], checksumOk[1]);
        }
    }
}

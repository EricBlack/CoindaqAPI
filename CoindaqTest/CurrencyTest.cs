using CoindaqAPI.Utils.FiatCurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoindaqTest
{
    [TestClass]
    public class CurrencyTest
    {
        public CurrencySet CurrencySet { get; set; }

        [TestInitialize]
        public void InitTest()
        {
            CurrencySet = new CurrencySet();
        }
        [TestMethod]
        public void TestRequest()
        {

        }

        [TestMethod]
        public void TestDeserializeObject()
        {
        }
    }
}

/**************************************************************
 *  Filename:      ValidateHelperTest
 *
 *  Description: ValidateHelperTest ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *  @Created:    2018/5/30/周三 18:44:38 
 **************************************************************/

using CoindaqAPI.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoindaqTest
{
    [TestClass]
    public class ValidateHelperTest
    {
        [TestMethod]
        public void TestPhone()
        {
            string phone = "13943060540";
            var result = ValidateHelper.IsPhoneFormat(phone);
            Assert.IsTrue(result);
        }
    }
}

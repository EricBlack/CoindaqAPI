/**************************************************************
 *  Filename:      CoinInfoTest
 *
 *  Description: CoinInfoTest ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *  @Created:    2018/6/6/周三 12:32:49 
 **************************************************************/

using CoindaqAPI.Utils.FiatCurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoindaqTest
{
    [TestClass]
    public class CoinInfoTest
    {
        public CoinInfoSet CoinSet { get; set; }

        [TestInitialize]
        public void Init()
        {
            CoinSet = new CoinInfoSet();
        }

        [TestMethod]
        public void ReadCoinInfo()
        {
            CoinSet.ConvertCoinSet();
            foreach(var coin in CoinSet.CoinSet)
            {
                Console.WriteLine($"{coin.Id},  {coin.Name}, {coin.Tag}");
                foreach(var info in coin.Convert)
                {
                    Console.Write($"{info} ");
                }
                Console.WriteLine();
            }
        }
        
    }
}

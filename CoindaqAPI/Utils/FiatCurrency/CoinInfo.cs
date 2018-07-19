/**************************************************************
 *  Filename:      CoinInfo
 *
 *  Description:  CoinInfo ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/6/6/周三 12:13:04 
 **************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoindaqAPI.Utils.FiatCurrency
{
    public class CoinInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public List<string> Convert { get; set; }

        public CoinInfo()
        {
            Convert = new List<string>();
        }
    }

    public class CoinInfoSet
    {
        public List<CoinInfo> CoinSet { get; set; }
        public XDocument Document { get; set; }
        public CoinInfoSet()
        {
            CoinSet = new List<CoinInfo>();
            Document = XDocument.Load(@"Config/coinlist.xml");
        }

        public void ConvertCoinSet()
        {
            //获取到XML的根元素进行操作
            XElement root = Document.Root;
            var coinList = root.Elements("Coin");
            foreach(var coin in coinList)
            {
                var coinInfo = new CoinInfo();
                coinInfo.Convert = new List<string>();

                //Id
                XElement Id = coin.Element("Id");
                coinInfo.Id = Id.Value.ToString();
                //Name
                XElement Name = coin.Element("Name");
                coinInfo.Name = Name.Value.ToString();
                //Tag
                XElement Tag = coin.Element("Tag");
                coinInfo.Tag = Tag.Value.ToString();
                //Convert
                XElement Convert = coin.Element("Convert");
                var infoList = Convert.Elements("Info");
                foreach(var info in infoList)
                {
                    coinInfo.Convert.Add(info.Value.ToString());
                }

                CoinSet.Add(coinInfo);
            }
        }
    }
}

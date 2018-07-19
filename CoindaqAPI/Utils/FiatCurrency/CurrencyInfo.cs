/**************************************************************
 *  Filename:      CurrencyInfo
 *
 *  Description:  CurrencyInfo ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/19/周六 11:46:30 
 **************************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.FiatCurrency
{
    public class CurrencyInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("price_usd")]
        public float PriceUsd { get; set; }

        [JsonProperty("price_btc")]
        public float PriceBtc { get; set; }

        [JsonProperty("24h_volume_usd")]
        public float VolumnUsd24h { get; set; }

        [JsonProperty("market_cap_usd")]
        public float MarketCapUsd { get; set; }

        [JsonProperty("available_supply")]
        public float AvailableSupply { get; set; }

        [JsonProperty("total_supply")]
        public float TotalSupply { get; set; }

        [JsonProperty("max_supply")]
        public float MaxSupply { get; set; }

        [JsonProperty("percent_change_1h")]
        public float PercentChange1h { get; set; }

        [JsonProperty("percent_change_24h")]
        public float PercentChange2h { get; set; }

        [JsonProperty("percent_change_7d")]
        public float PercentChange7d { get; set; }

        [JsonProperty("last_updated")]
        public int LastUpdated { get; set; }
    }
}

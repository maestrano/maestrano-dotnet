using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maestrano.Api;
using System.Collections.Specialized;
using RestSharp;
using Newtonsoft.Json;

namespace Maestrano.Account
{

    public class Bill
    {
        public string PresetName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("status")]
        public String Status { get; set; }

        [JsonProperty("units")]
        public decimal? Units { get; set; }

        [JsonProperty("period_started_at")]
        public DateTime? PeriodStartedAt { get; set; }

        [JsonProperty("period_ended_at")]
        public DateTime? PeriodEndedAt { get; set; }

        // Mandatory for creation

        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        [JsonProperty("price_cents")]
        public Int32 PriceCents { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("third_party")]
        public bool ThirdParty { get; set; }

        /// <summary>
        /// The Resource name
        /// </summary>
        /// <returns></returns>
        public static string IndexPath()
        {
            return "account/bills";
        }

        /// <summary>
        /// The Single Resource name
        /// </summary>
        /// <returns></returns>
        public static string ResourcePath()
        {
            return IndexPath() + "/{id}";
        }

        /// <summary>
        /// Scope REST calls to specific configuration presets
        /// </summary>
        /// <param name="presetName">name of preset to use</param>
        /// <returns></returns>
        public static BillRequestor With(string presetName = "maestrano")
        {
            return new BillRequestor(presetName);
        }

        public static List<Bill> All(NameValueCollection filters = null)
        {
            return (new BillRequestor()).All(filters);
        }

        public static Bill Retrieve(string billId)
        {
            return (new BillRequestor()).Retrieve(billId);
        }

        public static Bill Create(String groupId, Int32 priceCents, String description, String currency = "AUD", Decimal? units = null, DateTime? periodStartedAt = null, DateTime? periodEndedAt = null, bool thirdParty = false)
        {
            return (new BillRequestor()).Create(
                groupId: groupId, 
                priceCents: priceCents, 
                description:description, 
                currency: currency,
                units: units,
                periodStartedAt: periodStartedAt,
                periodEndedAt: periodEndedAt,
                thirdParty: thirdParty
            );
        }

        public Boolean Cancel()
        {
            if (Id != null)
            {
                Bill respBill = MnoClient.Delete<Bill>(ResourcePath(), Id);
                Status = respBill.Status;
                return (Status.Equals("cancelled"));
            }

            return false;
        }
    }
}
